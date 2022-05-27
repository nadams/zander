package command

import (
	"fmt"
	"runtime/debug"
	"strings"
)

type VersionCmd struct{}

func (v *VersionCmd) Run(_ CmdCtx) error {
	info, ok := debug.ReadBuildInfo()
	if !ok {
		fmt.Println("version: unknown")
	}

	var out strings.Builder
	out.WriteString(fmt.Sprintf("Go version: %s\n", info.GoVersion))

	for _, setting := range info.Settings {
		if setting.Value != "" {
			out.WriteString(setting.Key)
			out.WriteString(": ")
			out.WriteString(setting.Value)
			out.WriteString("\n")
		}
	}

	fmt.Print(out.String())

	return nil
}
