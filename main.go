//go:generate protoc --go_out=. --go_opt=paths=source_relative --go-grpc_out=. --go-grpc_opt=paths=source_relative zproto/server.proto

package main

import (
	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/nadams/zander/internal/command"
)

type CLI struct {
	Server  command.Server           `cmd:"" help:"Start zander in server mode"`
	List    command.ListServersCmd   `cmd:"" help:"List configured doom servers"`
	Restart command.RestartServerCmd `cmd:"" help:"Restart a doom server"`
	Attach  command.AttachCmd        `cmd:"" help:"Attach to a running doom server"`

	Socket    string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Connects to a socket at the given path"`
	LogLevel  string `flag:"" enum:"fatal,error,warn,debug,info,trace" default:"warn" help:"Only show the given log severity or higher. (valid values: ${enum})"`
	LogFormat string `flag:"" enum:"text,json" default:"text" help:"Log output format. (valid values: ${enum})"`
}

func main() {
	var cli CLI
	ctx := command.Parse(&cli)

	configureLogger(&cli)

	ctx.FatalIfErrorf(ctx.Run(command.CmdCtx{
		Socket: cli.Socket,
	}))
}

func configureLogger(cli *CLI) {
	switch cli.LogFormat {
	case "json":
		log.SetFormatter(&log.JSONFormatter{})
	}

	switch cli.LogLevel {
	case "fatal":
		log.SetLevel(log.FatalLevel)
	case "error":
		log.SetLevel(log.ErrorLevel)
	case "warn":
		log.SetLevel(log.WarnLevel)
	case "debug":
		log.SetLevel(log.DebugLevel)
	case "info":
		log.SetLevel(log.InfoLevel)
	case "trace":
		log.SetLevel(log.TraceLevel)
	}
}
