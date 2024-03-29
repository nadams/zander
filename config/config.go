package config

import (
	"fmt"
	"os"
	"os/exec"
	"path/filepath"
	"strings"

	"github.com/pelletier/go-toml/v2"
)

type WADPaths []string

func (w WADPaths) Exists(wad string) bool {
	return false
}

func (w WADPaths) String() string {
	return strings.Join(w.Expanded(), string(os.PathListSeparator))
}

func (w WADPaths) Expanded() []string {
	paths := make([]string, 0, len(w))

	for _, p := range w {
		paths = append(paths, expanded(p))
	}

	return paths
}

func (w *WADPaths) FromEnv() {
	paths := strings.Split(os.Getenv("DOOMWADPATH"), string(os.PathListSeparator))
	if p := os.Getenv("DOOMWADDIR"); p != "" {
		paths = append(paths, p)
	}

	x := make(WADPaths, 0, len(paths))
	for _, p := range paths {
		x = append(x, p)
	}

	*w = x
}

type Config struct {
	ServerConfigDir string         `toml:"server_config_dir,omitempty"`
	WADPaths        WADPaths       `toml:"wad_paths,omitempty"`
	ServerBinaries  ServerBinaries `toml:"server_binaries,omitempty"`
	Metrics         Metrics        `toml:"metrics,omitempty"`

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

func (c Config) Exists(path string) bool {
	x, _ := exec.LookPath(path)

	return x != ""
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
		ServerBinaries: ServerBinaries{
			Zandronum: "zandronum-server",
			Odamex:    "odasrv",
		},

		dir: dir,
	}
}

type ServerBinaries struct {
	Zandronum string `toml:"zandronum,omitempty"`
	Odamex    string `toml:"odamex,omitempty"`
	ZDaemon   string `toml:"zdaemon,omitempty"`
}

func expanded(path string) string {
	if strings.HasPrefix(path, "~/") {
		homedir, err := os.UserHomeDir()
		if err != nil {
			panic(err)
		}

		path = filepath.Join(homedir, path[2:])
	}

	return os.ExpandEnv(path)
}
