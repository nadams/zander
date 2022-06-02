package command

import (
	"fmt"
	"net"
	"os"
	"os/signal"
	"path/filepath"
	"syscall"

	"github.com/adrg/xdg"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc"

	"gitlab.node-3.net/nadams/zander/config"
	"gitlab.node-3.net/nadams/zander/doom"
	"gitlab.node-3.net/nadams/zander/zproto"
	"gitlab.node-3.net/nadams/zander/zserver"
)

type Server struct {
	ConfigDir string `flag:"" short:"c" type:"path" description:"Directory containing zander configuration. Defaults to $XDG_CONFIG_HOME/zander"`

	ctx CmdCtx
}

func (s *Server) Run(cmdctx CmdCtx) error {
	s.ctx = cmdctx

	cfg, _, err := s.loadConfig()
	if err != nil {
		return err
	}

	manager, err := doom.Load(cfg)
	if err != nil {
		return err
	}

	if errs := manager.StartAll(); len(errs) > 0 {
		log.Error(errs)
	}

	go manager.Watch()

	return s.listenAndServe(manager)
}

func (s *Server) loadConfig() (config.Config, string, error) {
	configPath, err := xdg.ConfigFile("zander/zander.toml")
	if err != nil {
		return config.Config{}, "", fmt.Errorf("could not get config file path: %w", err)
	}

	cfg, err := config.FromDisk(configPath)
	if err != nil {
		return config.Config{}, "", err
	}

	return cfg, configPath, err
}

func (s *Server) listenAndServe(manager *doom.Manager) error {
	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

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

	go func() {
		<-sigs

		server.Stop()
	}()

	return server.Serve(l)
}
