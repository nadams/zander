package config

import (
	"os"
	"path/filepath"

	"github.com/pelletier/go-toml/v2"
)

type Server struct {
	ID           string   `toml:"id,omitempty"`
	Mode         string   `toml:"mode,omitempty"`
	Email        string   `toml:"email,omitempty"`
	Port         int      `toml:"port,omitempty"`
	Hostname     string   `toml:"hostname,omitempty"`
	Website      string   `toml:"website,omitempty"`
	IWAD         string   `toml:"iwad,omitempty"`
	PWADs        []string `toml:"pwads,omitempty"`
	Skill        int      `toml:"skill,omitempty"`
	MOTD         string   `toml:"motd,multiline,omitempty"`
	Maplist      []string `toml:"maplist,omitempty"`
	RCONPassword string   `toml:"rcon_password,omitempty"`
	RawParams    string   `toml:"rawparams,multiline,omitempty"`
}

func LoadServer(path string) (Server, error) {
	f, err := os.OpenFile(path, os.O_RDONLY, 0)
	if err != nil {
		return Server{}, nil
	}

	defer f.Close()

	name := filepath.Base(path)
	name = name[:len(name)-len(filepath.Ext(name))]

	s := Server{
		ID: name,
	}

	if err := toml.NewDecoder(f).Decode(&s); err != nil {
		return Server{}, err
	}

	return s, nil
}
