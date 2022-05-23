package command

import (
	"bufio"
	"context"
	"fmt"
	"io"
	"os"
	"os/signal"
	"syscall"

	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		sigs := make(chan os.Signal, 1)
		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		go func() {
			<-sigs

			cancel()
		}()

		stream, err := client.Attach(ctx)
		if err != nil {
			return err
		}

		if err := stream.Send(&zproto.AttachIn{Id: a.ID}); err != nil {
			return err
		}

		go func() {
			scanner := bufio.NewScanner(os.Stdin)
			for scanner.Scan() {
				content := scanner.Bytes()

				if err := stream.Send(&zproto.AttachIn{
					Id:      a.ID,
					Content: content,
				}); err != nil {
					if err == io.EOF {
						return
					}

					log.Error(err)
					return
				}
			}
		}()

		for {
			in, err := stream.Recv()
			if err != nil {
				if err == io.EOF {
					break
				}

				if _, ok := status.FromError(err); ok {
					break
				}

				log.Errorf("unknown error from server: %v", err)
			}

			fmt.Print(string(in.Content))
		}

		return nil
	})
}
