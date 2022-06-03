package command

import (
	"context"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type RestartServerCmd struct {
	Ids []string `arg:"" required:"true" help:"Doom servers to restart"`
}

func (r *RestartServerCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.RestartServer(ctx, &zproto.RestartServerRequest{Ids: r.Ids})

		return err
	})
}
