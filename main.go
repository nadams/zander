package main

import (
	"gitlab.node-3.net/nadams/zander/internal/command"
)

type CLI struct {
	Server command.Server    `cmd:"" help:"Start zander in server mode"`
	Attach command.AttachCmd `cmd:"" help:"Attach to a running doom server"`
	List   command.ListCmd   `cmd:"" help:"List configured doom servers"`

	Socket string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Connects to a socket at the given path"`
}

func main() {
	var cli CLI
	ctx := command.Parse(&cli)

	ctx.FatalIfErrorf(ctx.Run(command.CmdCtx{
		Socket: cli.Socket,
	}))
}
