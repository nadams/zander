package command

import (
	"fmt"
	"net"
	"os"
	"os/signal"
	"path/filepath"
	"syscall"

	"github.com/adrg/xdg"
	grpc_middleware "github.com/grpc-ecosystem/go-grpc-middleware"
	grpc_logrus "github.com/grpc-ecosystem/go-grpc-middleware/logging/logrus"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc"

	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/doom"
	"gitlab.node-3.net/zander/zander/zproto"
	"gitlab.node-3.net/zander/zander/zserver"
)

type Server struct {
	ConfigDir string `flag:"" short:"c" type:"path" description:"Directory containing zander configuration. Defaults to $XDG_CONFIG_HOME/zander"`

	ctx CmdCtx
}

func (s *Server) Run(cmdctx CmdCtx) error {
	s.ctx = cmdctx

	cfg, cfgPath, err := loadConfig()
	if err != nil {
		return err
	}

	log.Infof("loaded config from %s", cfgPath)

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

	entry := log.NewEntry(log.StandardLogger())
	grpc_logrus.ReplaceGrpcLogger(entry)

	opts := []grpc.ServerOption{
		grpc.StreamInterceptor(
			grpc_middleware.ChainStreamServer(
				grpc_logrus.StreamServerInterceptor(entry),
			),
		),
		grpc.UnaryInterceptor(
			grpc_middleware.ChainUnaryServer(
				grpc_logrus.UnaryServerInterceptor(entry),
			),
		),
	}

	server := grpc.NewServer(opts...)
	zproto.RegisterZanderServer(server, zserver.New(manager))

	go func() {
		<-sigs

		server.Stop()
	}()

	return server.Serve(l)
}

func configPath() (string, error) {
	configPath, err := xdg.SearchConfigFile("zander/zander.toml")
	if err != nil {
		return "", fmt.Errorf("could not get config file path: %w", err)
	}

	return configPath, nil
}

func loadConfig() (config.Config, string, error) {
	configPath, err := configPath()
	if err != nil {
		return config.Config{}, "", err
	}

	cfg, err := config.FromDisk(configPath)
	if err != nil {
		return config.Config{}, "", err
	}

	if len(cfg.WADPaths) == 0 {
		cfg.WADPaths.FromEnv()
	}

	return cfg, configPath, err
}
