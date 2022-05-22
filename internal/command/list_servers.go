package command

import (
	"context"
	"encoding/csv"
	"encoding/json"
	"fmt"
	"os"
	"time"

	"github.com/jedib0t/go-pretty/table"
	"gitlab.node-3.net/nadams/zander/zproto"
	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
	"gopkg.in/yaml.v2"
)

type ListServersCmd struct {
	Output string `flag:"" short:"o" enum:"table,json,yaml,csv,raw" default:"table"`

	header []string
}

func (l *ListServersCmd) Run(cmdCtx CmdCtx) error {
	l.header = []string{"ID", "Name", "Status", "Started"}

	opts := []grpc.DialOption{
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	}

	conn, err := grpc.Dial(fmt.Sprintf("unix:%s", cmdCtx.Socket), opts...)
	if err != nil {
		return err
	}

	defer conn.Close()

	client := zproto.NewZanderClient(conn)
	ctx, cancel := context.WithTimeout(context.Background(), time.Second*15)
	defer cancel()

	resp, err := client.ListServers(ctx, &zproto.ListServersRequest{})
	if err != nil {
		return err
	}

	switch l.Output {
	case "table":
		tw := table.NewWriter()
		h := make(table.Row, 0, len(l.header))
		for _, x := range l.header {
			h = append(h, x)
		}
		tw.AppendHeader(h)
		for _, s := range resp.Servers {
			tw.AppendRow(table.Row{s.Id, s.Name, s.Status, s.StartedAt.AsTime().Format(time.ANSIC)})
		}
		fmt.Fprintln(os.Stdout, tw.Render())
	case "csv":
		w := csv.NewWriter(os.Stdout)
		w.Write(l.header)
		for _, s := range resp.Servers {
			w.Write([]string{s.Id, s.Name, s.Status, s.StartedAt.AsTime().Format(time.RFC3339)})
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
}
