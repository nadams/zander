package command

import (
	"context"

	"gitlab.node-3.net/zander/zander/zproto"
)

type StartServerCmd struct {
	Ids []string `arg:"" required:"true" help:"Doom servers to start" predictor:"server_list"`
}

func (r *StartServerCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.StartServer(ctx, &zproto.StartServerRequest{Ids: r.Ids})

		return err
	})
}
