package config

import (
	"encoding/json"
	"fmt"
	"io"
	"os"

	"github.com/adrg/xdg"
)

type Config struct {
	Version int      `json:"version"`
	Servers []Server `json:"servers"`
	WADDir  string   `json:"waddir"`
}

func New() *Config {
	return &Config{
		Version: 1,
		WADDir:  "$DOOMWADDIR",
	}
}

func (c *Config) Validate() error {
	return nil
}

func (c *Config) LoadFromDisk() error {
	path, err := xdg.ConfigFile("zander/config.json")
	if err != nil {
		return fmt.Errorf("could not find config file: %w", err)
	}

	if _, err := os.Stat(path); err != nil {
		if err := c.create(path); err != nil {
			return err
		}
	}

	return c.load(path)
}

func (c *Config) load(path string) error {
	f, err := os.Open(path)
	if err != nil {
		return fmt.Errorf("could not open config file: %w", err)
	}

	defer f.Close()

	if err := json.NewDecoder(f).Decode(c); err != nil {
		return fmt.Errorf("could not load config file: %w", err)
	}

	return nil
}

func (c *Config) create(path string) error {
	f, err := os.OpenFile(path, os.O_CREATE|os.O_RDWR, 0600)
	if err != nil {
		return fmt.Errorf("could not create config file: %w", err)
	}

	defer f.Close()

	return c.newEncoder(f).Encode(c)
}

func (c *Config) newEncoder(w io.Writer) *json.Encoder {
	enc := json.NewEncoder(w)
	enc.SetEscapeHTML(false)
	enc.SetIndent("", "  ")

	return enc
}
