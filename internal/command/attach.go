package command

import (
	"bufio"
	"context"
	"fmt"
	"io"
	"log"
	"os"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	opts := []grpc.DialOption{
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	}

	conn, err := grpc.Dial(fmt.Sprintf("unix:%s", cmdCtx.Socket), opts...)
	if err != nil {
		return err
	}

	defer conn.Close()

	client := zproto.NewZanderClient(conn)
	//ctx, cancel := context.WithTimeout(context.Background(), time.Second*15)
	//defer cancel()
	ctx := context.Background()

	stream, err := client.Attach(ctx)
	if err != nil {
		return err
	}

	if err := stream.Send(&zproto.AttachIn{ServerId: a.ID}); err != nil {
		return err
	}

	go func() {
		scanner := bufio.NewScanner(os.Stdin)
		for scanner.Scan() {
			content := scanner.Bytes()

			if err := stream.Send(&zproto.AttachIn{
				ServerId: a.ID,
				Content:  content,
			}); err != nil {
				if err == io.EOF {
					return
				}

				log.Println(err)
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

			log.Printf("unknown error from server: %v", err)
		}

		fmt.Print(string(in.Content))
	}

	return nil
}
