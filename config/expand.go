package config

import (
	"os"
	"path/filepath"
	"strings"
)

func Expand(path string) string {
	if strings.HasPrefix(path, "~/") {
		homedir, err := os.UserHomeDir()
		if err != nil {
			panic(err)
		}

		path = filepath.Join(homedir, path[2:])
	}

	return os.ExpandEnv(path)
}
