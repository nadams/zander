//go:generate protoc --go_out=. --go_opt=paths=source_relative --go-grpc_out=. --go-grpc_opt=paths=source_relative zproto/server.proto

package main

import (
	"github.com/adrg/xdg"
	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/nadams/zander/internal/command"
)

var (
	version string
	commit  string
	date    string
)

type CLI struct {
	Server  command.Server           `cmd:"" help:"Start zander in server mode"`
	List    command.ListServersCmd   `cmd:"" help:"List configured doom servers"`
	Restart command.RestartServerCmd `cmd:"" help:"Restart a doom server"`
	Stop    command.StopServerCmd    `cmd:"" help:"Stops a doom server"`
	Attach  command.AttachCmd        `cmd:"" help:"Attach to a running doom server"`
	Version command.VersionCmd       `cmd:"" help:"Print zander version information"`

	Socket    string `flag:"" short:"s" type:"pathenv" env:"ZANDER_SOCKET" help:"Uses the given socket path for client/server communication. If no value is given, then it defaults to $XDG_RUNTIME_DIR/zander.sock"`
	LogLevel  string `flag:"" enum:"fatal,error,warn,debug,info,trace" default:"warn" env:"ZANDER_LOG_LEVEL" help:"Only show the given log severity or higher. (valid values: ${enum})"`
	LogFormat string `flag:"" enum:"text,json" default:"text" env:"ZANDER_LOG_FORMAT" help:"Log output format. (valid values: ${enum})"`
}

func (c CLI) ctx() command.CmdCtx {
	if commit == "" {
		commit = "devel"
	}

	return command.CmdCtx{
		Socket:  c.Socket,
		Commit:  commit,
		Version: version,
		Date:    date,
	}
}

func main() {
	var cli CLI
	ctx := command.Parse(&cli)

	configureLogger(&cli)

	if len(cli.Socket) == 0 {
		p, err := xdg.RuntimeFile("zander.sock")
		if err != nil {
			log.Fatal(err)
		}

		cli.Socket = p
	}

	ctx.FatalIfErrorf(ctx.Run(cli.ctx()))
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
