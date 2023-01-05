package command

import (
	"fmt"
	"strings"
)

type VersionCmd struct{}

func (v *VersionCmd) Run(ctx CmdCtx) error {
	out := &strings.Builder{}
	v.writeProp(out, "Version", ctx.Version)
	v.writeProp(out, "Go Version", ctx.GoVersion)
	v.writeProp(out, "Commit", ctx.Commit)
	v.writeProp(out, "Date", ctx.Date)

	fmt.Print(out.String())

	return nil
}

func (v *VersionCmd) writeProp(out *strings.Builder, name, value string) {
	if value == "" {
		return
	}

	out.WriteString(name)
	out.WriteString(": ")
	out.WriteString(value)
	out.WriteString("\n")
}
