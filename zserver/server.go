package zserver

import (
	"context"

	"gitlab.node-3.net/nadams/zander/zandronum"
	"gitlab.node-3.net/nadams/zander/zproto"
	"google.golang.org/protobuf/types/known/timestamppb"
)

type ZanderServer struct {
	zproto.UnimplementedZanderServer

	manager *zandronum.Manager
}

func New(manager *zandronum.Manager) *ZanderServer {
	return &ZanderServer{
		manager: manager,
	}
}

func (z *ZanderServer) ListServers(ctx context.Context, cmd *zproto.ListServersRequest) (*zproto.ListServersResponse, error) {
	servers := z.manager.List()
	serversOut := make([]*zproto.Server, 0, len(servers))

	for _, s := range servers {
		serversOut = append(serversOut, &zproto.Server{
			Id:        s.ID,
			Name:      s.Name,
			Status:    s.Status,
			StartedAt: timestamppb.New(s.Started),
		})
	}

	return &zproto.ListServersResponse{
		Servers: serversOut,
	}, nil
}
