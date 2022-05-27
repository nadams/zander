package command

import (
	"fmt"
	"runtime/debug"
)

type VersionCmd struct{}

func (v *VersionCmd) Run(_ CmdCtx) error {
	info, ok := debug.ReadBuildInfo()
	if !ok {
		fmt.Println("version: unknown")
	}

	fmt.Printf("Revision: %s\nGo version: %s\n", info.Main.Version, info.GoVersion)

	return nil
}
