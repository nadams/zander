package config

import (
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/pelletier/go-toml/v2"
)

type Config struct {
	ServerConfigDir string         `toml:"server_config_dir,omitempty"`
	WADDir          string         `toml:"wad_dir,omitempty"`
	ServerBinaries  ServerBinaries `toml:"server_binaries,omitempty"`

	dir string
}

func (c Config) Expand(path string) string {
	if strings.HasPrefix(path, "~/") {
		homedir, err := os.UserHomeDir()
		if err != nil {
			panic(err)
		}

		path = filepath.Join(homedir, path[2:])
	}

	return os.ExpandEnv(path)
}

func (c Config) ExpandRel(path string) string {
	path = c.Expand(path)

	if filepath.IsAbs(path) {
		return path
	}

	return filepath.Join(c.dir, path)
}

func FromDisk(path string) (Config, error) {
	f, err := os.OpenFile(path, os.O_RDONLY|os.O_CREATE, 0600)
	if err != nil {
		return Config{}, err
	}

	dir := filepath.Dir(path)

	defer f.Close()

	c := DefaultConfig(dir)

	if err := toml.NewDecoder(f).Decode(&c); err != nil {
		return Config{}, fmt.Errorf("could not decode configuration: %w", err)
	}

	return c, nil
}

func DefaultConfig(dir string) Config {
	return Config{
		ServerConfigDir: "servers",
		WADDir:          "wads",
		ServerBinaries: ServerBinaries{
			Zandronum: "zandronum-server",
		},

		dir: dir,
	}
}

type ServerBinaries struct {
	Zandronum string `toml:"zandronum,omitempty"`
	Odamex    string `toml:"odamex,omitempty"`
	ZDaemon   string `toml:"zdaemon,omitempty"`
}
