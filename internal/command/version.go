package command

import (
	"io"
	"os"
)

type VersionCmd struct{}

func (v *VersionCmd) Run(ctx CmdCtx) error {
	out := os.Stdout

	v.writeProp(out, "Version", ctx.Version)
	v.writeProp(out, "Go Version", ctx.GoVersion)
	v.writeProp(out, "Commit", ctx.Commit)
	v.writeProp(out, "Date", ctx.Date)

	return nil
}

func (v *VersionCmd) writeProp(out io.StringWriter, name, value string) {
	if value == "" {
		return
	}

	out.WriteString(name)
	out.WriteString(": ")
	out.WriteString(value)
	out.WriteString("\n")
}
