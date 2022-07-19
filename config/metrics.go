package config

type Collector string

const (
	Prometheus Collector = "prometheus"
	StatsD     Collector = "statsd"
)

type Metrics struct {
	Collector  Collector        `toml:"collector,omitempty"`
	Prometheus PrometheusConfig `toml:"prometheus,omitempty"`
	StatsD     StatsDConfig     `toml:"statsd,omitempty"`
}

type PrometheusConfig struct {
	Port int    `toml:"port,omitempty"`
	Path string `toml:"path,omitempty"`
}

type StatsDConfig struct {
	Address string `toml:"address,omitempty"`
	Prefix  string `toml:"prefix,omitempty"`
}
