package completions

import (
	"context"
	"sort"
	"time"

	"github.com/posener/complete"
	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/internal/command"
	"gitlab.node-3.net/zander/zander/zproto"
)

func ServersPredictor(addr string) complete.Predictor {
	return completeServers(addr)
}

func completeServers(addr string) complete.PredictFunc {
	return func(args complete.Args) []string {
		var ids []string

		if err := command.WithConnTimeout(addr, time.Second*2, func(ctx context.Context, client zproto.ZanderClient) error {
			resp, err := client.ListServers(ctx, &zproto.ListServersRequest{})
			if err != nil {
				return err
			}

			for _, server := range resp.Servers {
				ids = append(ids, server.Id)
			}

			sort.Strings(ids)

			return nil
		}); err != nil {
			log.Error(err)
		}

		return ids
	}
}
