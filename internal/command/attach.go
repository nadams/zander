package command

import (
	"bufio"
	"context"
	"fmt"
	"io"
	"os"

	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		//ctx, cancel := context.WithTimeout(context.Background(), time.Second*15)
		//defer cancel()
		ctx := context.Background()

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

				if err, ok := status.FromError(err); ok {
					if err.Message() == "error reading from server: EOF" {
						break
					}
				}

				log.Errorf("unknown error from server: %v", err)
			}

			fmt.Print(string(in.Content))
		}

		return nil
	})
}
