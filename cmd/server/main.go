package main

import (
	"fmt"
	"log"
	"net"
	"os"
	"path/filepath"

	"github.com/alecthomas/kong"
	"github.com/google/uuid"
	"gitlab.node-3.net/nadams/zander/internal/command"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type CLI struct {
	Socket          string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Listen on a socket at the given path"`
	ZandronumServer string `flag:"" short:"p" type:"pathenv" default:"zandronum-server" description:"The path to zandronum-server, may be just the name if it's in $PATH"`
}

func main() {
	var cli CLI
	ctx := kong.Parse(&cli, kong.NamedMapper("pathenv", command.PathEnvDecoder()))

	switch ctx.Command() {
	default:
		if err := ListenAndServe(cli); err != nil {
			fmt.Println(err)
			os.Exit(1)
		}
	}
}

func ListenAndServe(cli CLI) error {
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

	server := zandronum.NewServer(cli.ZandronumServer, nil)
	if err := server.Start(); err != nil {
		log.Println(err)
	}

	log.Println("listening for connections")
	for {
		conn, err := l.Accept()
		if err != nil {
			log.Println(err)
		}

		log.Println("client connected")

		go connect(conn, server)
	}
}

func connect(conn net.Conn, server *zandronum.Server) {
	id := uuid.New().String()

	if err := server.Connect(id, conn, conn); err != nil {
		log.Println(err)
	}

	server.Disconnect(id)
}
