package command

import (
	"fmt"
	"net"
	"os"
	"path/filepath"

	"google.golang.org/grpc"

	"gitlab.node-3.net/nadams/zander/zandronum"
	"gitlab.node-3.net/nadams/zander/zproto"
	"gitlab.node-3.net/nadams/zander/zserver"
)

type Server struct {
	Config string `flag:"" short:"c" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/config.json" description:"Path to config file"`

	ctx CmdCtx
}

func (s *Server) Run(cmdctx CmdCtx) error {
	s.ctx = cmdctx
	server := zandronum.NewServer("/usr/bin/zandronum-server", nil)

	manager := zandronum.NewManager()
	id := manager.Add(server)
	manager.Start(id)

	return s.listenAndServe(manager)
}

func (s *Server) listenAndServe(manager *zandronum.Manager) error {
	removeSocket := func() error {
		if err := os.RemoveAll(s.ctx.Socket); err != nil {
			return fmt.Errorf("could not remove zander socket: %w", err)
		}

		return nil
	}

	if err := removeSocket(); err != nil {
		return err
	}

	if err := os.MkdirAll(filepath.Dir(s.ctx.Socket), 0755); err != nil {
		return fmt.Errorf("could not create path for socket: %w", err)
	}

	l, err := net.Listen("unix", s.ctx.Socket)
	if err != nil {
		return fmt.Errorf("could not create socket: %w", err)
	}

	defer l.Close()

	defer removeSocket()

	var opts []grpc.ServerOption

	server := grpc.NewServer(opts...)
	zproto.RegisterZanderServer(server, zserver.New(manager))
	server.Serve(l)

	return nil
}
