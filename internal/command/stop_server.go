package command

import (
	"context"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type StopServerCmd struct {
	ID string `arg:"" required:"true"`
}

func (r *StopServerCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.StopServer(ctx, &zproto.StopServerRequest{
			Id: r.ID,
		})

		return err
	})
}
