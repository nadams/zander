package command

import (
	"bytes"
	"context"
	"io"
	"os"
	"os/signal"
	"syscall"

	"gitlab.node-3.net/zander/zander/zproto"
)

type LogsCmd struct {
	ID    string `arg:"" required:"true" help:"ID of doom server to print logs from" predictor:"server_list"`
	Lines int    `required:"true" short:"n" default:"-1" help:"The number of recent lines to print"`
}

func (t *LogsCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()

		sigs := make(chan os.Signal, 1)

		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		go func() {
			<-sigs

			cancel()
		}()

		out, err := client.Logs(ctx, &zproto.LogsIn{Id: t.ID, Num: int32(t.Lines)})
		if err != nil {
			return err
		}

		_, err = io.Copy(os.Stdout, bytes.NewReader(out.Content))

		return err
	})
}
