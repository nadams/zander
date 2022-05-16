package main

import (
	"gitlab.node-3.net/nadams/zander/internal/command"
)

type CLI struct {
	Server command.Server    `cmd:""`
	Attach command.AttachCmd `cmd:""`
	List   command.ListCmd   `cmd:""`

	Socket string `flag:"" short:"s" type:"pathenv" default:"$XDG_CONFIG_HOME/zander/zander.sock" description:"Connects to a socket at the given path"`
}

func main() {
	var cli CLI
	ctx := command.Parse(&cli)

	ctx.FatalIfErrorf(ctx.Run(command.CmdCtx{
		Socket: cli.Socket,
	}))
}
