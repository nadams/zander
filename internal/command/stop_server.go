package command

import (
	"context"

	"gitlab.node-3.net/zander/zander/zproto"
)

type StopServerCmd struct {
	Ids []string `arg:"" required:"true" help:"Doom servers to stop" predictor:"server_list"`
}

func (r *StopServerCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.StopServer(ctx, &zproto.StopServerRequest{Ids: r.Ids})

		return err
	})
}
