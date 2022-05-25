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
	"github.com/rivo/tview"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true" help:"ID of doom server to attach to"`

	Output      string `flag:"" enum:"raw,default" default:"default" help:"Output mode. (valid values: ${enum})"`
	ScrollLines int    `flag:"" default:"5" help:"How many lines to scroll when pgup and pgdn are pressed. Only valid in default output mode"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
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
	app := tview.NewApplication()

	output := tview.NewTextView()
	output.SetBorder(false)
	output.SetBackgroundColor(tcell.ColorDefault)
	output.SetScrollable(true)

	app.SetInputCapture(func(event *tcell.EventKey) *tcell.EventKey {
		row, _ := output.GetScrollOffset()

		switch event.Key() {
		case tcell.KeyPgUp:
			if row > 0 {
				to := row - a.ScrollLines
				if to < 0 {
					to = 0
				}

				output.ScrollTo(to, 0)
			}
		case tcell.KeyPgDn:
			maxRows := output.GetOriginalLineCount()
			to := row + a.ScrollLines

			if to > maxRows {
				to = maxRows
			}

			output.ScrollTo(to, 0)
		}

		return event
	})

	go func() {
		for content := range in {
			if content != "" {
				fmt.Fprint(output, string(content))
				app.Draw()
				app.QueueUpdate(func() {
					output.ScrollToEnd()
				})
			}
		}

		app.Stop()
	}()

	input := tview.NewInputField()
	input.SetBorder(false)
	input.SetFieldWidth(0)
	input.SetBackgroundColor(tcell.ColorDefault)
	input.SetFieldBackgroundColor(tcell.ColorDefault)
	input.SetDoneFunc(func(key tcell.Key) {
		orig := input.GetText()
		intercept := strings.ToLower(strings.TrimSpace(orig))

		switch intercept {
		case "help":
			fmt.Fprintln(output, "help requested")
		default:
			out <- input.GetText()
		}

		input.SetText("")
	})

	layout := tview.NewFlex()
	layout.SetDirection(tview.FlexRow)
	layout.AddItem(output, 0, 1, false)
	layout.AddItem(hr(), 1, 0, false)
	layout.AddItem(input, 1, 0, true)

	return app.SetRoot(layout, true).Run()
}

func hr() *tview.Box {
	return tview.NewBox().
		SetBackgroundColor(tcell.ColorDefault).
		SetDrawFunc(func(screen tcell.Screen, x, y, width, height int) (int, int, int, int) {
			for cx := x; cx < x+width; cx++ {
				screen.SetContent(cx, y, tview.BoxDrawingsLightHorizontal, nil, tcell.StyleDefault.Foreground(tcell.ColorWhite))
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

type servercmd struct {
	Cmd         string
	Example     string
	Description string
}

func (a *AttachCmd) availableCommands() []servercmd {
	return []servercmd{
		{
			Cmd:     "AddBan",
			Example: "AddBan 192.168.2.* 2min \"Test Ban\"",
			Description: `
			Issues a ban on an individual client or clients. Syntax for AddBan is: AddBan <IPv4> ["Reason"]. Additionally, range bans are acceptable by using wildcards, such as ( * ). However, when the client or clients are blacklisted, they will not be able to communicate with that server until the client or clients are whitelisted or expunged from the blacklist.

			Time
				When approaching this argument, the time can be inputted as 6days or even 1345years. The allowed time/date formats are: minutes, hours, days, months, and years.
				Perm (or permanent ban) is what it is, permanent. This can also be used as the time argument.

				Note: IPv4 is only supported; IPv6 is unsupported at the mean time.`,
		},
	}
}
