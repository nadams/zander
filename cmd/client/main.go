package main

import (
	"github.com/alecthomas/kong"
	"gitlab.node-3.net/nadams/zander/cmd/client/cmds"
	"gitlab.node-3.net/nadams/zander/internal/command"
)

type CLI struct {
	Socket string         `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Connects to a socket at the given path"`
	Attach cmds.AttachCmd `cmd:"" help:"Attach to a running server"`
}

func main() {
	var cli CLI
	ctx := kong.Parse(&cli, kong.NamedMapper("pathenv", command.PathEnvDecoder()))

	ctx.FatalIfErrorf(ctx.Run(cli.Socket))
}
