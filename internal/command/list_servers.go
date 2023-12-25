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
	"github.com/jedib0t/go-pretty/v6/table"
	"github.com/jedib0t/go-pretty/v6/text"
	"gitlab.node-3.net/zander/zander/doom"
	"gitlab.node-3.net/zander/zander/zproto"
	"gopkg.in/yaml.v2"
)

var columns = map[string]string{
	"id":         "ID",
	"name":       "Name",
	"engine":     "Engine",
	"port":       "Port",
	"mode":       "Mode",
	"status":     "Status",
	"iwad":       "IWAD",
	"pwads":      "PWADs",
	"players":    "Players",
	"started_at": "Started At",
	"stopped_at": "Stopped At",
}

var tableProps = map[string]func(s *zproto.Server) string{
	"id":      func(s *zproto.Server) string { return s.Id },
	"name":    func(s *zproto.Server) string { return s.Name },
	"engine":  func(s *zproto.Server) string { return s.Engine },
	"port":    func(s *zproto.Server) string { return fmt.Sprintf("%d", s.Port) },
	"mode":    func(s *zproto.Server) string { return s.Mode },
	"iwad":    func(s *zproto.Server) string { return s.Iwad },
	"pwads":   func(s *zproto.Server) string { return strings.Join(s.Pwads, "\n") },
	"players": func(s *zproto.Server) string { return fmt.Sprintf("%d", s.Players) },
	"status": func(s *zproto.Server) string {
		status := s.Status

		if text.ANSICodesSupported {
			switch doom.ServerStatus(s.Status) {
			case doom.Running:
				status = text.FgGreen.Sprint(status)
			case doom.Stopped:
				status = text.FgBlue.Sprint(status)
			case doom.Errored:
				status = text.FgRed.Sprint(status)
			case doom.NotStarted:
				status = text.FgYellow.Sprint(status)
			case doom.Disabled:
				status = text.FgWhite.Sprint(status)
			}
		}

		t := s.StartedAt.AsTime()
		updown := "started"

		if s.StoppedAt.GetSeconds() > 0 {
			updown = "stopped"
			t = s.StoppedAt.AsTime()
		}

		nicetime := humanize.Time(t)
		if t.Equal(time.Unix(0, 0)) {
			return status
		}

		return fmt.Sprintf("%s\n%s %s", status, updown, nicetime)
	},
	"started_at": func(s *zproto.Server) string {
		if s.StartedAt.IsValid() && !s.StartedAt.AsTime().IsZero() {
			return s.StartedAt.AsTime().Format(time.RFC3339)
		}

		return ""
	},
	"stopped_at": func(s *zproto.Server) string {
		if s.StoppedAt.IsValid() && !s.StoppedAt.AsTime().IsZero() {
			return s.StoppedAt.AsTime().Format(time.RFC3339)
		}

		return ""
	},
}

type ListServersCmd struct {
	Output  string   `flag:"" short:"o" env:"ZANDER_LIST_OUTPUT" enum:"table,json,yaml,csv,raw" default:"table" help:"Output format. valid values: (${enum})"`
	Columns []string `flag:"" short:"c" env:"ZANDER_LIST_COLUMNS" enum:"id,name,engine,port,mode,status,iwad,pwads,players,started_at,stopped_at" default:"id,engine,name,port,mode,status,iwad,pwads,players" sep:"," help:"Which columns to show in table output and csv format. valid values: (${enum})"`

	header []string
}

func (l *ListServersCmd) Run(cmdCtx CmdCtx) error {
	l.header = make([]string, 0, len(l.Columns))
	for _, col := range l.Columns {
		l.header = append(l.header, columns[col])
	}

	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		resp, err := client.ListServers(ctx, &zproto.ListServersRequest{})
		if err != nil {
			return err
		}

		for _, server := range resp.Servers {
			if server.StartedAt.IsValid() && server.StartedAt.AsTime().IsZero() {
				server.StartedAt = nil
			}

			if server.StoppedAt.IsValid() && server.StoppedAt.AsTime().IsZero() {
				server.StoppedAt = nil
			}
		}

		sort.Slice(resp.Servers, func(i, j int) bool {
			if resp.Servers[i].Name != resp.Servers[j].Name {
				return resp.Servers[i].Name < resp.Servers[j].Name
			}

			return resp.Servers[i].Id < resp.Servers[j].Id
		})

		switch l.Output {
		case "table":
			tw := table.NewWriter()
			tw.SetColumnConfigs([]table.ColumnConfig{
				{Name: columns["name"], WidthMax: 30},
			})

			h := make(table.Row, 0, len(l.header))
			for _, x := range l.header {
				h = append(h, x)
			}
			tw.AppendHeader(h)

			for _, s := range resp.Servers {
				var row table.Row

				for _, col := range l.Columns {
					f, ok := tableProps[col]
					if !ok {
						row = append(row, "")
						continue
					}

					row = append(row, f(s))
				}

				tw.AppendRow(row)
			}

			fmt.Fprintln(os.Stdout, tw.Render())
		case "csv":
			w := csv.NewWriter(os.Stdout)
			w.Write(l.header)

			tableProps["pwads"] = func(s *zproto.Server) string {
				return strings.Join(s.Pwads, ";")
			}

			tableProps["status"] = func(s *zproto.Server) string {
				return s.Status
			}

			for _, s := range resp.Servers {
				var row []string

				for _, col := range l.Columns {
					f, ok := tableProps[col]
					if !ok {
						row = append(row, "")
						continue
					}

					row = append(row, f(s))
				}

				w.Write(row)
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
