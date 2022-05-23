package zserver

import (
	"context"
	"io"
	"log"

	"github.com/google/uuid"
	"google.golang.org/grpc"
	"google.golang.org/grpc/codes"
	"google.golang.org/protobuf/types/known/timestamppb"

	"gitlab.node-3.net/nadams/zander/zandronum"
	"gitlab.node-3.net/nadams/zander/zproto"
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

func (z *ZanderServer) Attach(stream zproto.Zander_AttachServer) error {
	initial, err := stream.Recv()
	if err != nil {
		return err
	}

	srv, found := z.manager.Get(zandronum.ID(initial.ServerId))
	if !found {
		return grpc.Errorf(codes.NotFound, "server with id '%v' not found", initial.ServerId)
	}

	wait := make(chan struct{})
	send := make(chan []byte)
	recv := make(chan []byte)

	go func() {
		for {
			in, err := stream.Recv()
			if err != nil {
				if err == io.EOF {
					log.Println("close channel")
					close(wait)
					return
				}

				log.Printf("error when reading client: %v", err)
				return
			}

			log.Println("got data")

			recv <- in.Content
		}
	}()

	go func() {
		for data := range send {
			if err := stream.Send(&zproto.AttachOut{
				Content: data,
			}); err != nil {
				log.Println(err)
				return
			}
		}

		log.Println("done sending data")
	}()

	id := uuid.New().String()
	if err := srv.Connect(id, send, recv); err != nil {
		return err
	}

	close(wait)

	defer srv.Disconnect(id)

	<-wait

	return nil
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
