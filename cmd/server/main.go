package main

import (
	"fmt"
	"log"
	"net"
	"os"
	"path/filepath"

	"github.com/alecthomas/kong"
	"gitlab.node-3.net/nadams/zander/config"
	"gitlab.node-3.net/nadams/zander/internal/command"
	"gitlab.node-3.net/nadams/zander/internal/handler"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type CLI struct {
	Socket string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Listen on a socket at the given path"`
	Config string `flag:"" short:"c" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/config.json" description:"Path to config file"`
}

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
		if err := ListenAndServe(cli, cfg); err != nil {
			fmt.Println(err)
			os.Exit(1)
		}
	}
}

func ListenAndServe(cli CLI, cfg *config.Config) error {
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

	server := zandronum.NewServer("/usr/share/zandronum/zandronum-server", nil)
	if err := server.Start(); err != nil {
		log.Println(err)
	}

	for {
		conn, err := l.Accept()
		if err != nil {
			log.Println(err)
		}

		go handler.HandleConnection(conn, server)
	}
}
