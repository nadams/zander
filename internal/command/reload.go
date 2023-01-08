package command

import (
	"context"

	"gitlab.node-3.net/zander/zander/zproto"
)

type ReloadCmd struct{}

func (r *ReloadCmd) Run(cmdCtx CmdCtx) error {
	return WithConnTimeout(cmdCtx.Socket, DefaultTimeout, func(ctx context.Context, client zproto.ZanderClient) error {
		_, err := client.Reload(ctx, &zproto.ReloadIn{})
		return err
	})
}
