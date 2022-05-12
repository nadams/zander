package main

import (
	"fmt"
	"log"
	"net"
	"os"
	"os/signal"
	"path/filepath"
	"syscall"

	"github.com/alecthomas/kong"
	"gitlab.node-3.net/nadams/zander/config"
	"gitlab.node-3.net/nadams/zander/internal/command"
	"gitlab.node-3.net/nadams/zander/internal/handler"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type CLI struct {
	Socket string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Listen on a socket at the given path"`
	Config string `flag:"" short:"c" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/config.json" description:"Path to config file"`
}

var quit = make(chan struct{}, 1)

func main() {
	var cli CLI
	ctx := kong.Parse(&cli, kong.NamedMapper("pathenv", command.PathEnvDecoder()))

	cfg := config.New()
	if err := cfg.LoadFromDisk(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	if err := cfg.Validate(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	switch ctx.Command() {
	default:
		if err := Start(cli, cfg); err != nil {
			fmt.Println(err)
			os.Exit(1)
		}
	}
}

func Start(cli CLI, cfg *config.Config) error {
	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

	go func() {
		<-sigs

		quit <- struct{}{}
	}()

	server := zandronum.NewServer("/usr/share/zandronum/zandronum-server", nil)

	manager := zandronum.NewManager()
	id := manager.Add(server)
	manager.Start(id)

	registerHandlers(manager)

	return ListenAndServe(cli, cfg, manager)
}

func ListenAndServe(cli CLI, cfg *config.Config, manager *zandronum.Manager) error {
	removeSocket := func() error {
		if err := os.RemoveAll(cli.Socket); err != nil {
			return fmt.Errorf("could not remove zander socket: %w", err)
		}

		return nil
	}

	if err := removeSocket(); err != nil {
		return err
	}

	if err := os.MkdirAll(filepath.Dir(cli.Socket), 0755); err != nil {
		return fmt.Errorf("could not create path for socket: %w", err)
	}

	l, err := net.Listen("unix", cli.Socket)
	if err != nil {
		return fmt.Errorf("could not create socket: %w", err)
	}

	defer l.Close()

	defer removeSocket()

	go func() {
		<-quit
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

		log.Println("handling conn")
		go handler.Handle(conn)
	}

	return nil
}

func registerHandlers(manager *zandronum.Manager) {
	handler.Register(message.CMD_LIST_SERVERS, handler.ListServers(manager))
	handler.Register(message.CMD_ATTACH, handler.Attach(manager))
}
