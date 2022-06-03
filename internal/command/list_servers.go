package command

import (
	"context"
	"encoding/csv"
	"encoding/json"
	"fmt"
	"os"
	"sort"
	"strings"
	"time"

	"github.com/dustin/go-humanize"
	"github.com/jedib0t/go-pretty/table"
	"github.com/jedib0t/go-pretty/text"
	"gitlab.node-3.net/nadams/zander/doom"
	"gitlab.node-3.net/nadams/zander/zproto"
	"gopkg.in/yaml.v2"
)

type ListServersCmd struct {
	Output string `flag:"" short:"o" enum:"table,json,yaml,csv,raw" default:"table" help:"Output format. valid values: (valid values: ${enum})"`

	header []string
}

func (l *ListServersCmd) Run(cmdCtx CmdCtx) error {
	l.header = []string{"ID", "Name", "Port", "Mode", "Status", "IWAD", "PWADs"}

	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		resp, err := client.ListServers(ctx, &zproto.ListServersRequest{})
		if err != nil {
			return err
		}

		sort.Slice(resp.Servers, func(i, j int) bool {
			return resp.Servers[i].Name < resp.Servers[j].Name
		})

		switch l.Output {
		case "table":
			tw := table.NewWriter()
			tw.SetColumnConfigs([]table.ColumnConfig{
				{Name: "Name", WidthMax: 30},
			})
			h := make(table.Row, 0, len(l.header))
			for _, x := range l.header {
				h = append(h, x)
			}
			tw.AppendHeader(h)
			for _, s := range resp.Servers {
				status := s.Status
				switch doom.ServerStatus(status) {
				case doom.Running:
					status = text.FgGreen.Sprint(status)
				case doom.Stopped:
					status = text.FgBlue.Sprint(status)
				case doom.Errored:
					status = text.FgRed.Sprint(status)
				case doom.NotStarted:
					status = text.FgYellow.Sprint(status)
				}

				t := s.StartedAt.AsTime()
				updown := "started"

				if s.StoppedAt.GetSeconds() > 0 {
					updown = "stopped"
					t = s.StoppedAt.AsTime()
				}

				nicetime := humanize.Time(t)
				status = fmt.Sprintf("%s\n%s %s", status, updown, nicetime)

				pwads := strings.Join(s.Pwads, "\n")
				tw.AppendRow(table.Row{s.Id, s.Name, s.Port, s.Mode, status, s.Iwad, pwads})
			}
			fmt.Fprintln(os.Stdout, tw.Render())
		case "csv":
			w := csv.NewWriter(os.Stdout)
			w.Write(l.header)
			for _, s := range resp.Servers {
				pwads := strings.Join(s.Pwads, ";")
				w.Write([]string{s.Id, s.Name, fmt.Sprintf("%d", s.Port), s.Mode, s.Status, s.Iwad, pwads, s.StartedAt.AsTime().Format(time.ANSIC)})
			}
			w.Flush()
		case "json":
			enc := json.NewEncoder(os.Stdout)
			enc.SetEscapeHTML(false)
			enc.SetIndent("", "  ")

			enc.Encode(resp)
		case "yaml":
			yaml.NewEncoder(os.Stdout).Encode(resp)
		default:
			fmt.Fprintf(os.Stdout, "%+v\n", resp)
		}

		return nil
	})
}
