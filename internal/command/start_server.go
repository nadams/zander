package command

import (
	"context"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type StartServerCmd struct {
	ID string `arg:"" required:"true"`
}

func (r *StartServerCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.StartServer(ctx, &zproto.StartServerRequest{
			Id: r.ID,
		})

		return err
	})
}
