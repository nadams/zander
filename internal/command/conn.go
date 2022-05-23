package command

import (
	"context"
	"fmt"
	"time"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"

	"gitlab.node-3.net/nadams/zander/zproto"
)

const DefaultTimeout = time.Second * 15

func WithConn(addr string, fn func(client zproto.ZanderClient) error) error {
	opts := []grpc.DialOption{
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	}

	conn, err := grpc.Dial(fmt.Sprintf("unix:%s", addr), opts...)
	if err != nil {
		return err
	}

	defer conn.Close()

	client := zproto.NewZanderClient(conn)

	return fn(client)
}

func WithConnTimeout(addr string, timeout time.Duration, fn func(ctx context.Context, client zproto.ZanderClient) error) error {
	opts := []grpc.DialOption{
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	}

	conn, err := grpc.Dial(fmt.Sprintf("unix:%s", addr), opts...)
	if err != nil {
		return err
	}

	defer conn.Close()

	client := zproto.NewZanderClient(conn)

	ctx, cancel := context.WithTimeout(context.Background(), time.Second*15)
	defer cancel()

	return fn(ctx, client)
}
