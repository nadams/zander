package config

import (
	"fmt"
	"os"

	"github.com/pelletier/go-toml/v2"
)

type Config struct {
	ServerConfigDir string         `toml:"server_config_dir,omitempty"`
	WADDir          string         `toml:"wad_dir,omitempty"`
	ServerBinaries  ServerBinaries `toml:"server_binaries,omitempty"`
}

func FromDisk(path string) (*Config, error) {
	f, err := os.OpenFile(path, os.O_RDONLY|os.O_CREATE, 0600)
	if err != nil {
		return nil, err
	}

	defer f.Close()

	c := DefaultConfig()

	if err := toml.NewDecoder(f).Decode(&c); err != nil {
		return nil, fmt.Errorf("could not decode configuration: %w", err)
	}

	return &c, nil
}

func DefaultConfig() Config {
	return Config{
		ServerConfigDir: "servers",
		WADDir:          "wads",
		ServerBinaries: ServerBinaries{
			Zandronum: "zandronum-server",
		},
	}
}

type ServerBinaries struct {
	Zandronum string `toml:"zandronum,omitempty"`
	Odamex    string `toml:"odamex,omitempty"`
	ZDaemon   string `toml:"zdaemon,omitempty"`
}

type ServerConfig struct {
	ID           string
	Mode         string
	Email        string
	Port         int
	Hostname     string
	Website      string
	IWAD         string
	PWADs        []string
	Skill        int
	MOTD         string
	Maplist      []string
	RCONPassword string
	RawParams    string
}
