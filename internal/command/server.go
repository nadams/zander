package command

import (
	"fmt"
	"log"
	"net"
	"os"
	"os/signal"
	"path/filepath"
	"syscall"

	"gitlab.node-3.net/nadams/zander/config"
	"gitlab.node-3.net/nadams/zander/internal/handler"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type Server struct {
	Config string `flag:"" short:"c" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/config.json" description:"Path to config file"`

	ctx  CmdCtx
	quit chan struct{}
}

func (s *Server) Run(cmdctx CmdCtx) error {
	s.ctx = cmdctx
	s.quit = make(chan struct{}, 1)

	cfg := config.New()
	if err := cfg.LoadFromDisk(); err != nil {
		return err
	}

	if err := cfg.Validate(); err != nil {
		return err
	}

	return s.run(cfg)
}

func (s *Server) run(cfg *config.Config) error {
	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

	go func() {
		<-sigs

		s.quit <- struct{}{}
	}()

	server := zandronum.NewServer("zandronum-server", nil)

	manager := zandronum.NewManager()
	id := manager.Add(server)
	manager.Start(id)

	s.registerHandlers(manager)

	return s.listenAndServe(cfg, manager)
}

func (s *Server) listenAndServe(cfg *config.Config, manager *zandronum.Manager) error {
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

	go func() {
		<-s.quit
		l.Close()
	}()

	for {
		conn, err := l.Accept()
		if err != nil {
			if _, ok := err.(*net.OpError); ok {
				break
			}

			log.Println(err)
			continue
		}

		go handler.Handle(conn)
	}

	return nil
}

func (s *Server) registerHandlers(manager *zandronum.Manager) {
	handler.Register(message.CMD_LIST_SERVERS, handler.ListServers(manager))
	handler.Register(message.CMD_ATTACH, handler.Attach(manager))
}
