package command

import (
	"encoding/csv"
	"encoding/json"
	"fmt"
	"os"
	"time"

	"github.com/jedib0t/go-pretty/v6/table"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
	"gopkg.in/yaml.v2"
)

type ListCmd struct {
	Output string `flag:"" short:"o" enum:"table,json,yaml,csv,raw" default:"table"`

	header []string
}

func (l *ListCmd) Run(ctx CmdCtx) error {
	l.header = []string{"ID", "Name", "Status", "Started"}

	client := zandronum.NewClient(ctx.Socket)
	if err := client.Open(); err != nil {
		return err
	}

	defer client.Close()

	client.Send() <- message.Message{BodyType: message.CMD_LIST_SERVERS}

	msg := <-client.Recv()
	if msg.BodyType != message.SERVER_LIST {
		return fmt.Errorf("invalid response from server: %v", msg.BodyType)
	}

	var body message.ServerList

	if err := json.Unmarshal(msg.Body, &body); err != nil {
		return fmt.Errorf("response from server not understood: %w", err)
	}

	switch l.Output {
	case "table":
		tw := table.NewWriter()
		h := make(table.Row, 0, len(l.header))
		for _, x := range l.header {
			h = append(h, x)
		}
		tw.AppendHeader(h)
		for _, s := range body.Servers {
			tw.AppendRow(table.Row{s.ID, s.Name, s.Status, s.Started.Format(time.ANSIC)})
		}
		fmt.Fprintln(os.Stdout, tw.Render())
	case "csv":
		w := csv.NewWriter(os.Stdout)
		w.Write(l.header)
		for _, s := range body.Servers {
			w.Write([]string{s.ID, s.Name, s.Status, s.Started.Format(time.RFC3339)})
		}
		w.Flush()
	case "json":
		enc := json.NewEncoder(os.Stdout)
		enc.SetEscapeHTML(false)
		enc.SetIndent("", "  ")

		enc.Encode(body)
	case "yaml":
		yaml.NewEncoder(os.Stdout).Encode(body)
	default:
		fmt.Fprintf(os.Stdout, "%+v\n", body)
	}

	return nil
}
