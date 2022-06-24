package command

import (
	"context"
	"fmt"
	"io"
	"os"
	"os/signal"
	"syscall"

	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/zander/zander/zproto"
)

type TailCmd struct {
	ID string `arg:"" required:"true" help:"ID of doom server to attach to" predictor:"server_list"`
}

func (t *TailCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()

		sigs := make(chan os.Signal, 1)

		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		stream, err := client.Tail(ctx, &zproto.TailIn{Id: t.ID})
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
