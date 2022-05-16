package command

import (
	"encoding/csv"
	"encoding/json"
	"fmt"
	"log"
	"os"
	"time"

	"github.com/jedib0t/go-pretty/table"
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

	for msg := range client.Recv() {
		if msg.BodyType == message.SERVER_LIST {
			var body message.ServerList

			if err := json.Unmarshal(msg.Body, &body); err != nil {
				log.Println(err)
				break
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
			break
		}
	}

	return nil
}
