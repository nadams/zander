package config

type Collector string

type Metrics struct {
	Prometheus *PrometheusConfig `toml:"prometheus,omitempty"`
	StatsD     *StatsDConfig     `toml:"statsd,omitempty"`
}

type PrometheusConfig struct {
	Enabled bool   `toml:"enabled"`
	Port    int    `toml:"port,omitempty"`
	Path    string `toml:"path,omitempty"`
}

type StatsDConfig struct {
	Enabled bool   `toml:"enabled"`
	Address string `toml:"address,omitempty"`
	Prefix  string `toml:"prefix,omitempty"`
}
