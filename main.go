//go:generate protoc --go_out=. --go_opt=paths=source_relative --go-grpc_out=. --go-grpc_opt=paths=source_relative zproto/server.proto

package main

import (
	"os"

	"github.com/adrg/xdg"
	"github.com/alecthomas/kong"
	log "github.com/sirupsen/logrus"
	"github.com/willabides/kongplete"

	"gitlab.node-3.net/zander/zander/internal/command"
	"gitlab.node-3.net/zander/zander/internal/command/completions"
)

var (
	version   string
	commit    string
	date      string
	goversion string
)

type CLI struct {
	Server  command.Server           `cmd:"" help:"Start zander in server mode"`
	List    command.ListServersCmd   `cmd:"" help:"List configured doom servers" aliases:"ls,ps"`
	Start   command.StartServerCmd   `cmd:"" help:"Starts a doom server"`
	Stop    command.StopServerCmd    `cmd:"" help:"Stops a doom server"`
	Restart command.RestartServerCmd `cmd:"" help:"Restart a doom server"`
	Attach  command.AttachCmd        `cmd:"" help:"Attach to a running doom server"`
	Logs    command.LogsCmd          `cmd:"" help:"View log of a doom server"`
	Version command.VersionCmd       `cmd:"" help:"Print zander version information"`

	Socket    string `flag:"" short:"s" type:"xdgruntimefile" default:"zander.sock" env:"ZANDER_SOCKET" help:"Uses the given socket path for client/server communication. If no value is given, then it defaults to $XDG_RUNTIME_DIR/zander.sock"`
	LogLevel  string `flag:"" enum:"fatal,error,warn,debug,info,trace" default:"warn" env:"ZANDER_LOG_LEVEL" help:"Only show the given log severity or higher. (valid values: ${enum})"`
	LogFormat string `flag:"" enum:"text,json" default:"text" env:"ZANDER_LOG_FORMAT" help:"Log output format. (valid values: ${enum})"`

	InstallCompletions kongplete.InstallCompletions `cmd:"" help:"install shell completions"`
}

func (c CLI) ctx() command.CmdCtx {
	if commit == "" {
		commit = "devel"
	}

	return command.CmdCtx{
		Socket:    c.Socket,
		Commit:    commit,
		Version:   version,
		GoVersion: goversion,
		Date:      date,
	}
}

func main() {
	var cli CLI
	ctx := parse(&cli)

	configureLogger(&cli)

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

func parse(a *CLI, options ...kong.Option) *kong.Context {
	parser := kong.Must(a,
		kong.Name("zander"),
		kong.Description("Doom server manager"),
		kong.NamedMapper("pathenv", command.PathEnvDecoder()),
		kong.NamedMapper("xdgruntimefile", command.XDGRuntimeFile()),
		kong.UsageOnError(),
	)

	if len(a.Socket) == 0 {
		p, err := xdg.RuntimeFile("zander.sock")
		if err != nil {
			log.Fatal(err)
		}

		a.Socket = p
	}

	kongplete.Complete(parser,
		kongplete.WithPredictor("server_list", completions.ServersPredictor(a.Socket)),
	)

	ctx, err := parser.Parse(os.Args[1:])
	parser.FatalIfErrorf(err)

	return ctx
}
