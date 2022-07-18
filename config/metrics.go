package config

type Collector string

const (
	Prometheus Collector = "pometheus"
)

type Metrics struct {
	Enabled   bool      `toml:"enabled"`
	Collector Collector `toml:"collector"`
}
