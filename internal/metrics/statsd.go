package metrics

import (
	"github.com/cactus/go-statsd-client/v5/statsd"

	"gitlab.node-3.net/zander/zander/config"
)

const (
	statsdPlayerCountTotal = "zander.player_count.total"
)

var _ Metrics = (*StatsD)(nil)

type StatsD struct {
	cfg    config.StatsDConfig
	client statsd.Statter
}

func NewStatsD(cfg config.StatsDConfig) *StatsD {
	return &StatsD{
		cfg: cfg,
	}
}

func (s *StatsD) Open() error {
	client, err := statsd.NewClientWithConfig(&statsd.ClientConfig{
		Address: s.cfg.Address,
		Prefix:  s.cfg.Prefix,
	})

	if err != nil {
		return err
	}

	s.client = client

	return nil
}

func (s *StatsD) Close() error {
	if s.client != nil {
		return s.client.Close()
	}

	return nil
}

func (s *StatsD) IncPlayerCount(serverID string) {
	s.client.Inc(statsdPlayerCountTotal, 1, 1.0, statsd.Tag{"server_id", serverID})
}

func (s *StatsD) DecPlayerCount(serverID string) {
	s.client.Dec(statsdPlayerCountTotal, 1, 1.0, statsd.Tag{"server_id", serverID})
}

func (s *StatsD) SetPlayerCount(serverID string, count uint) {
	s.client.SetInt(statsdPlayerCountTotal, int64(count), 1.0, statsd.Tag{"server_id", serverID})
}
