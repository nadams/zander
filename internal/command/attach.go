package command

import (
	"bufio"
	"context"
	"fmt"
	"io"
	"os"
	"os/signal"
	"strings"
	"syscall"

	"github.com/gdamore/tcell/v2"
	"github.com/jedib0t/go-pretty/table"
	"github.com/rivo/tview"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/zander/zander/internal/history"
	"gitlab.node-3.net/zander/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true" help:"ID of doom server to attach to" predictor:"server_list"`

	Output                    string `flag:"" enum:"raw,default" default:"default" help:"Output mode. (valid values: ${enum})"`
	ScrollLines               int    `flag:"" default:"5" help:"How many lines to scroll when pgup and pgdn are pressed. Only valid in default output mode"`
	CmdHistoryLen             int    `flag:"" default:"500" help:"How many entered commands to remember"`
	DeDuplicatedHistoryAppend bool   `flag:"" negatable:"" default:"true" help:"Do not duplicate history entries if the previous and submitted commands are the same"`

	stickToBottom bool
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	a.stickToBottom = true

	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()

		in := make(chan string)
		out := make(chan string)
		sigs := make(chan os.Signal, 1)

		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		go func() {
			<-sigs

			close(in)
			close(out)
		}()

		stream, err := client.Attach(ctx)
		if err != nil {
			return err
		}

		if err := stream.Send(&zproto.AttachIn{Id: a.ID}); err != nil {
			return err
		}

		go func() {
			for cmd := range out {
				if err := stream.Send(&zproto.AttachIn{
					Id:      a.ID,
					Content: []byte(cmd),
				}); err != nil {
					if err == io.EOF {
						return
					}

					log.Error(err)
					return
				}
			}
		}()

		go func() {
			for {
				msg, err := stream.Recv()
				if err != nil {
					if err == io.EOF {
						close(in)
						break
					}

					if _, ok := status.FromError(err); ok {
						close(in)
						break
					}

					log.Errorf("unknown error from server: %v", err)
				}

				in <- string(msg.Content)
			}
		}()

		switch a.Output {
		case "raw":
			return a.setupRawOutput(cancel, in, out)
		default:
			return a.setupDefaultOutput(cancel, in, out)
		}
	})
}

func (a *AttachCmd) setupDefaultOutput(cancel func(), in <-chan string, out chan<- string) error {
	cmdHistory := history.NewCmdHistory(
		history.WithMaxHistory(a.CmdHistoryLen),
		history.WithDeDuplicatedAppend(a.DeDuplicatedHistoryAppend),
	)

	cmdptr := cmdHistory.Ptr()
	app := tview.NewApplication()

	output := tview.NewTextView()
	output.SetBorder(false)
	output.SetBackgroundColor(tcell.ColorDefault)
	output.SetScrollable(true)
	output.SetWrap(true)
	output.SetChangedFunc(func() {
		app.Draw()
	})

	go func() {
		for content := range in {
			fmt.Fprintln(output, string(content))

			if a.stickToBottom {
				output.ScrollToEnd()
			}
		}

		app.Stop()
	}()

	input := tview.NewInputField()
	input.SetBorder(false)
	input.SetFieldWidth(0)
	input.SetBackgroundColor(tcell.ColorDefault)
	input.SetFieldBackgroundColor(tcell.ColorDefault)
	input.SetInputCapture(func(event *tcell.EventKey) *tcell.EventKey {
		switch event.Key() {
		case tcell.KeyUp:
			input.SetText(cmdptr.Prev())

			return nil
		case tcell.KeyDown:
			input.SetText(cmdptr.Next())

			return nil
		}

		return event
	})
	input.SetDoneFunc(func(key tcell.Key) {
		if orig := input.GetText(); len(orig) > 0 {
			intercept := strings.ToLower(strings.TrimSpace(orig))

			cmdHistory.Append(orig)
			cmdptr.Reset()

			switch intercept {
			case "help", "?", "h":
				fmt.Fprintln(output, a.renderHelp(output))
			default:
				out <- input.GetText()
			}

			input.SetText("")
			output.ScrollToEnd()
		}
	})

	app.SetInputCapture(func(event *tcell.EventKey) *tcell.EventKey {
		switch event.Key() {
		case tcell.KeyPgUp:
			if event.Modifiers()&tcell.ModCtrl == tcell.ModCtrl {
				output.ScrollToBeginning()
				a.stickToBottom = false
				return event
			}
			row, _ := output.GetScrollOffset()
			if row > 0 {
				to := row - a.ScrollLines
				if to < 0 {
					to = 0
				}

				a.stickToBottom = false
				output.ScrollTo(to, 0)
			}
		case tcell.KeyPgDn:
			if event.Modifiers()&tcell.ModCtrl == tcell.ModCtrl {
				output.ScrollToEnd()
				a.stickToBottom = true
				return event
			}

			row, _ := output.GetScrollOffset()
			maxRows := output.GetOriginalLineCount()
			_, _, _, height := output.GetInnerRect()
			to := row + a.ScrollLines

			if height+to >= maxRows {
				to = maxRows
				a.stickToBottom = true
			}

			output.ScrollTo(to, 0)
		}

		return event
	})

	layout := tview.NewFlex()
	layout.SetDirection(tview.FlexRow)
	layout.AddItem(output, 0, 1, false)
	layout.AddItem(hr(func() bool { return a.stickToBottom }), 1, 0, false)
	layout.AddItem(input, 1, 0, true)

	return app.SetRoot(layout, true).Run()
}

func hr(stick func() bool) *tview.Box {
	return tview.NewBox().
		SetBackgroundColor(tcell.ColorDefault).
		SetDrawFunc(func(screen tcell.Screen, x, y, width, height int) (int, int, int, int) {
			r := tview.BoxDrawingsLightHorizontal
			if !stick() {
				r = 'v'
			}

			for cx := x; cx < x+width; cx++ {
				screen.SetContent(cx, y, r, nil, tcell.StyleDefault.Foreground(tcell.ColorWhite))
			}

			return x, y, width, height
		})
}

func (a *AttachCmd) setupRawOutput(cancel func(), in <-chan string, out chan<- string) error {
	defer cancel()

	go func() {
		scanner := bufio.NewScanner(os.Stdin)

		for scanner.Scan() {
			out <- scanner.Text()
		}
	}()

	for msg := range in {
		fmt.Print(msg)
	}

	return nil
}

func (a *AttachCmd) renderHelp(out *tview.TextView) string {
	header := table.Row{"Cmd", "Example", "Description"}
	_, _, wi, _ := out.GetInnerRect()
	wi -= len(header)*len(header) + 1
	w := float64(wi)

	style := table.StyleLight
	style.Options.SeparateRows = true

	tw := table.NewWriter()
	tw.SetStyle(style)
	tw.AppendHeader(header)
	tw.SetColumnConfigs([]table.ColumnConfig{
		{Name: "Cmd", WidthMax: int(w * 0.2)},
		{Name: "Example", WidthMax: int(w * 0.3)},
		{Name: "Description", WidthMax: int(w * 0.5)},
	})

	for _, cmd := range a.serverCommands() {
		tw.AppendRow(table.Row{cmd.Cmd, cmd.Example, strings.TrimSpace(cmd.Description)})
	}

	return tw.Render()
}

type servercmd struct {
	Cmd         string
	Example     string
	Description string
}

func (a *AttachCmd) serverCommands() []servercmd {
	return []servercmd{
		{
			Cmd:     "AddBan",
			Example: "AddBan 192.168.2.* 2min \"Test Ban\"",
			Description: `Issues a ban on an individual client or clients. Syntax for AddBan is: AddBan <IPv4> ["Reason"]. Additionally, range bans are acceptable by using wildcards, such as ( * ). However, when the client or clients are blacklisted, they will not be able to communicate with that server until the client or clients are whitelisted or expunged from the blacklist.

Time
	When approaching this argument, the time can be inputted as 6days or even 1345years. The allowed time/date formats are: minutes, hours, days, months, and years.
	Perm (or permanent ban) is what it is, permanent. This can also be used as the time argument.

	Note: IPv4 is only supported; IPv6 is unsupported at the mean time.`,
		},
		{
			Cmd:         "AddBanExemption",
			Example:     "AddBanExemption 192.168.2.50",
			Description: "",
		},
		{
			Cmd:         "AddBot",
			Example:     "AddBot \"Strife Guy\"",
			Description: "",
		},
		{
			Cmd:         "AddMap",
			Example:     "AddMap MAP02\nAddMap MAP02 2 10",
			Description: "",
		},
		{
			Cmd:         "Ban",
			Example:     "Ban JoeDirt Perm \"Test\"",
			Description: "",
		},
		{
			Cmd:         "Ban_IDx",
			Example:     "Ban_IDx 5 \"45 Months\" \"test\"",
			Description: "",
		},
		{
			Cmd:         "CancelVote",
			Description: "Forces the vote to be terminated.",
		},
		{
			Cmd:         "ChangeMap",
			Example:     "ChangeMap MAP31\nChangeMap K1E1",
			Description: "",
		},
		{
			Cmd:         "ClearBans",
			Description: "Expunges all of the clients IP's from the blacklist.",
		},
		{
			Cmd:         "ClearMapList",
			Description: "Thrashes all maps from the server's MapList.",
		},
		{
			Cmd:         "CMDList",
			Description: "Displays all available Commands on the console.",
		},
		{
			Cmd:         "DelBan",
			Example:     "DelBan 127.0.0.1",
			Description: "Expunges the specified IP from the blacklist.",
		},
		{
			Cmd:         "DelBanExemption",
			Example:     "DelBanExemption 192.168.0.50",
			Description: "Expunges the specified IP from the whitelist.",
		},
		{
			Cmd:         "DelMap",
			Description: "Removes a map by lump name from rotation.",
		},
		{
			Cmd:         "DelMap_IDx",
			Description: "Removes a map by number from rotation.",
		},
		{
			Cmd:         "DumpTrafficMeasure",
			Description: "Displays how much of the bandwidth is used for both ACS scripts and Actor classes. However, the server CVar [yellow]SV_MeasureOutBoundTraffic[white] must be true for this to work. See Measuring outbound traffic for a tutorial.",
		},
		{
			Cmd:         "Exit",
			Description: "Terminates the current Zandronum session.",
		},
		{
			Cmd:         "GameVersion",
			Description: "Displays the current executable version and revision.",
		},
		{
			Cmd:         "GetIP",
			Example:     "GetIP \"Player\"",
			Description: "When used within the server console, this CCMD displays the targeted clients IP address.",
		},
		{
			Cmd:         "GetIP_IDx",
			Example:     "GetIP_IDx 6",
			Description: "When used within the server console, this CCMD displays the targeted clients IP address by use of the Player index number list. The player index number list can be found in the 'PlayerInfo' CCMD.",
		},
		{
			Cmd:         "History",
			Description: "When used, this displays all of the commands that were inputted into the console.",
		},
		{
			Cmd:         "Ignore",
			Example:     "Ignore \"Mr. X\"",
			Description: "Ignores a specified client. It is plausible to specify a time in which the client is ignored, such as Ignore \"Mr. X\" 5. The time is measured in minutes.",
		},
		{
			Cmd:         "Ignore_IDx",
			Example:     "Ignore_IDx 4",
			Description: "Ignores a specific client with the use of Player Index number list. It is plausible to specify a time in which the client is ignored, such as Ignore_IDx 4 5. The player index number list can be found in the 'PlayerInfo' CCMD.",
		},
		{
			Cmd:         "InsertMap <Lump name> <Position> [MinPlayers] [MaxPlayers]",
			Example:     "InsertMap MAP32 5",
			Description: "Inserts a map to the map rotation list, after [Position]. Use CCMD maplist to list the rotation with index numbers. InsertMap also supports optional minimum and maximum player limits.",
		},
		{
			Cmd:         "IP",
			Example:     "IP",
			Description: "Displays the clients IP on the console.",
		},
		{
			Cmd:         "Kick",
			Example:     "Kick \"Player Name Here\"",
			Description: "Forces the client to disconnect from the server. When using this CCMD use quotations on the players name in order to be sure that the correct player is kicked.",
		},
		{
			Cmd:         "Kick_IDx",
			Example:     "Kick_IDx 8",
			Description: "Forces the client to disconnect from the server, while utilizing the Player Index list.",
		},
		{
			Cmd:         "Kick_IP",
			Example:     "Kick_IP 127.0.0.1 \"Reason\"",
			Description: "Forces the client(s) with the exact IP address to be disconnected from the server. However, the clients can reconnect to the server.",
		},
		{
			Cmd:         "KickFromGame",
			Example:     "KickFromGame \"Player 123\"",
			Description: "Forces the client from the server to spectate the game. However, the client can rejoin the game when plausible.",
		},
		{
			Cmd:         "KickFromGame_IDx",
			Example:     "KickFromGame_IDx 13",
			Description: "Forces the client from the server to spectate the game by using the Player list index. However, the client can rejoin the game when plausible. The player index number list can be found in the 'PlayerInfo' CCMD.",
		},
		{
			Cmd:         "ListBots",
			Description: "Displays all available bots.",
		},
		{
			Cmd:         "LogFile",
			Example:     "LogFile \"C:\\Program Files(x86)\\Zandronum\\Log History\\My First Log File.txt\"",
			Description: "When used, this CCMD will log every messages presented within the console.",
		},
		{
			Cmd:         "Map",
			Example:     "Map MAP23\nMap WTF02",
			Description: "",
		},
		{
			Cmd:         "MapList",
			Description: "Displays the servers MapList.",
		},
		{
			Cmd:         "NextMap",
			Description: "When used, allows the server to jump into the next map. Adjacent to 'ChangeMap' and 'Map' CCMDs, but 'NextMap' does not require map names. NextMap will only advance to the next map that is specified in either [yellow]MAPINFO[white] or the MapRotation list.",
		},
		{
			Cmd:         "NextSecret",
			Description: "When used, this allows the server to jump into the next secret map.",
		},
		{
			Cmd:         "Pings",
			Description: "Displays all of the connected clients pings.",
		},
		{
			Cmd:         "PlayerInfo",
			Description: "When entered by clients displays a Player index number list and their pings. However, when entered within the server console, this will display not only the Player index number list but their IP's.",
		},
		{
			Cmd:         "PrintJoinQueue",
			Description: "This allows the server to list the full join queue.",
		},
		{
			Cmd:         "Puke",
			Example:     "Puke \"SCRIPT #\"",
			Description: "When used, allows the use to force a script to execute within the PWAD/PKx files. This is usually used for project developers.",
		},
		{
			Cmd:         "Quit",
			Description: "Terminates the current Zandronum session.",
		},
		{
			Cmd:         "ReloadBans",
			Description: "When used, this enforces the server to reload the blacklist. This is useful when the blacklist file has been modified and the server did not read the changes before the server initially started.",
		},
		{
			Cmd:         "RemoveBot",
			Example:     "RemoveBot \"Strife Guy\"",
			Description: "Removes either a random bot (RemoveBot) or removes a specific bot from the game (RemoveBot \"BotName\").",
		},
		{
			Cmd:         "RemoveBots",
			Description: "Remove [yellow]all[white] bots from the game.",
		},
		{
			Cmd:         "Say",
			Example:     "Say \"Hello World!\"",
			Description: "Allows the players/server to communicate with others in the game, by use of the console.",
		},
		{
			Cmd:         "SayTo",
			Example:     "SayTo \"Player 123\" \"Hello!\"",
			Description: "Allows the player/server to communicate privately with each other privately which other players cannot see, using the name of the player who will receive the message. If you want to send a private message to the server, use \"Server\" as the name instead.",
		},
		{
			Cmd:         "SayTo_IDx",
			Example:     "SayTo_IDx 2 \"Hello!\"",
			Description: "Allows the player/server to communicate privately with each other which other players cannot see, using the index of the player who will receive the message. If you want to send a private message to the server, use -1 for the index instead.",
		},
		{
			Cmd:         "Unignore",
			Example:     "Unignore \"Mr. X\"",
			Description: "Unignores a specified client.",
		},
		{
			Cmd:         "Unignore_IDx",
			Example:     "Unignore_IDx 4",
			Description: "Unignores a specific client with the use of Player Index number list. The player index number list can be found in the 'PlayerInfo' CCMD.",
		},
		{
			Cmd:         "Version_Info",
			Description: "Displays information the Mercurial changeset that the binary build is based on.",
		},
		{
			Cmd:         "ViewBanList",
			Description: "Displays the blacklist within the servers console.",
		},
		{
			Cmd:         "ViewBanExemptionList",
			Description: "Displays the whitelist within the servers console.",
		},
		{
			Cmd:         "WADS",
			Description: "Displays all of the loaded WADs used in the current session.",
		},
	}
}
