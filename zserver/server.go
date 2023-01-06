package zserver

import (
	"bytes"
	"context"
	"io"
	"strings"

	"github.com/google/uuid"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
	"google.golang.org/protobuf/types/known/timestamppb"

	"gitlab.node-3.net/zander/zander/doom"
	"gitlab.node-3.net/zander/zander/zproto"
)

type ZanderServer struct {
	zproto.UnimplementedZanderServer

	manager *doom.Manager
}

func New(manager *doom.Manager) *ZanderServer {
	return &ZanderServer{
		manager: manager,
	}
}

func (z *ZanderServer) StartServer(ctx context.Context, in *zproto.StartServerRequest) (*zproto.StartServerResponse, error) {
	var failure []string
	var success []string

	for _, id := range in.Ids {
		if err := z.manager.Start(doom.ID(id)); err != nil {
			failure = append(failure, id)
		} else {
			success = append(success, id)
		}
	}

	return &zproto.StartServerResponse{
		Success: success,
		Failure: failure,
	}, nil
}

func (z *ZanderServer) StopServer(ctx context.Context, in *zproto.StopServerRequest) (*zproto.StopServerResponse, error) {
	var failure []string
	var success []string

	for _, id := range in.Ids {
		if err := z.manager.Stop(doom.ID(id)); err != nil {
			failure = append(failure, id)
		} else {
			success = append(success, id)
		}
	}

	return &zproto.StopServerResponse{
		Success: success,
		Failure: failure,
	}, nil
}

func (z *ZanderServer) RestartServer(ctx context.Context, in *zproto.RestartServerRequest) (*zproto.RestartServerResponse, error) {
	var failure []string
	var success []string

	for _, id := range in.Ids {
		if err := z.manager.Restart(doom.ID(id)); err != nil {
			failure = append(failure, id)
		} else {
			success = append(success, id)
		}
	}

	return &zproto.RestartServerResponse{
		Success: success,
		Failure: failure,
	}, nil
}

func (z *ZanderServer) Attach(stream zproto.Zander_AttachServer) error {
	initial, err := stream.Recv()
	if err != nil {
		return err
	}

	srv, found := z.manager.Get(doom.ID(initial.Id))
	if !found {
		return grpc.Errorf(codes.NotFound, "server with id '%v' not found", initial.Id)
	}

	wait := make(chan struct{}, 1)
	send := make(chan []byte)
	recv := make(chan []byte)

	go func() {
		for {
			in, err := stream.Recv()
			if err != nil {
				defer func() {
					wait <- struct{}{}
				}()

				if err == io.EOF {
					return
				} else if st, ok := status.FromError(err); ok && st.Code() == codes.Canceled {
					return
				}

				log.Errorf("error when reading client: %v", err)
				return
			}

			if in.Content != nil && strings.TrimSpace(string(in.Content)) == "restart" {
				continue
			}

			recv <- in.Content
		}
	}()

	go func() {
		for data := range send {
			if err := stream.Send(&zproto.AttachOut{
				Content: data,
			}); err != nil {
				log.Error(err)
				return
			}
		}
	}()

	id := uuid.New().String()
	go func() {
		defer func() {
			wait <- struct{}{}
		}()

		opts := doom.ConnectOpts{ID: id}

		if err := srv.Connect(opts, send, recv); err != nil {
			log.Error(err)
			return
		}
	}()

	<-wait

	srv.Disconnect(id)

	return nil
}

func (z *ZanderServer) Tail(in *zproto.TailIn, stream zproto.Zander_TailServer) error {
	srv, found := z.manager.Get(doom.ID(in.Id))
	if !found {
		return grpc.Errorf(codes.NotFound, "server with id '%v' not found", in.Id)
	}

	send := make(chan []byte)
	recv := make(chan []byte)

	go func() {
		for data := range send {
			if err := stream.Send(&zproto.TailOut{Content: data}); err != nil {
				log.Error(err)
				return
			}
		}
	}()

	id := uuid.New().String()
	go func() {
		opts := doom.ConnectOpts{
			ID:    id,
			Lines: int(in.Num),
		}

		if err := srv.Connect(opts, send, recv); err != nil {
			log.Error(err)
			return
		}
	}()

	<-stream.Context().Done()

	close(recv)
	close(send)

	srv.Disconnect(id)

	return nil
}

func (z *ZanderServer) Logs(ctx context.Context, in *zproto.LogsIn) (*zproto.LogsOut, error) {
	srv, found := z.manager.Get(doom.ID(in.Id))
	if !found {
		return nil, grpc.Errorf(codes.NotFound, "server with id '%v' not found", in.Id)
	}

	var b bytes.Buffer

	for _, line := range srv.Logs(int(in.Num)) {
		b.WriteString(line)
		b.WriteRune('\n')
	}

	return &zproto.LogsOut{
		Content: b.Bytes(),
	}, nil
}

func (z *ZanderServer) ListServers(ctx context.Context, cmd *zproto.ListServersRequest) (*zproto.ListServersResponse, error) {
	servers := z.manager.List()
	serversOut := make([]*zproto.Server, 0, len(servers))

	for _, s := range servers {
		serversOut = append(serversOut, &zproto.Server{
			Id:        s.ID,
			Name:      s.Name,
			Engine:    string(s.Engine),
			Mode:      s.Mode,
			Status:    s.Status,
			Port:      int32(s.Port),
			Iwad:      s.IWAD,
			Pwads:     s.PWADs,
			StartedAt: timestamppb.New(s.Started),
			StoppedAt: timestamppb.New(s.Stopped),
		})
	}

	return &zproto.ListServersResponse{
		Servers: serversOut,
	}, nil
}
