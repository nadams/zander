package command

import (
	"bytes"
	"context"
	"fmt"
	"io"
	"os"
	"os/signal"
	"syscall"

	log "github.com/sirupsen/logrus"
	"gitlab.node-3.net/zander/zander/zproto"
	"google.golang.org/grpc/status"
)

type LogsCmd struct {
	ID    string `arg:"" required:"true" help:"ID of doom server to print logs from" predictor:"server_list"`
	Lines int    `short:"n" default:"-1" help:"The number of recent lines to print"`
	Tail  bool   `short:"t" help:"Run in tail mode"`
}

func (t *LogsCmd) Run(cmdCtx CmdCtx) error {
	if t.Tail {
		return t.tail(cmdCtx)
	}

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

func (t *LogsCmd) tail(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()

		sigs := make(chan os.Signal, 1)

		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		stream, err := client.Tail(ctx, &zproto.TailIn{Id: t.ID, Num: int32(t.Lines)})
		if err != nil {
			return err
		}

		go func() {
			<-sigs

			fmt.Println()

			stream.CloseSend()

			cancel()
		}()

		for {
			msg, err := stream.Recv()
			if err != nil {
				if err == io.EOF {
					break
				}

				if _, ok := status.FromError(err); ok {
					break
				}

				log.Errorf("unknown error from server: %v", err)
			}

			fmt.Println(string(msg.Content))
		}

		return nil
	})
}
