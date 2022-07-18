package config

type Collector string

const (
	Prometheus Collector = "prometheus"
)

type Metrics struct {
	Collector  Collector        `toml:"collector"`
	Prometheus PrometheusConfig `toml:"prometheus"`
}

type PrometheusConfig struct {
	Port int    `toml:"port"`
	Path string `toml:"path"`
}
