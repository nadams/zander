package metrics

import (
	"fmt"
	"net"
	"net/http"
	"strings"

	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/promauto"
	"github.com/prometheus/client_golang/prometheus/promhttp"
	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/util"
)

const (
	LabelServerID = "server_id"
	LabelEngine   = "engine"
)

var _ Metrics = (*Prometheus)(nil)

type Prometheus struct {
	cfg config.PrometheusConfig

	playerCountTotal *prometheus.GaugeVec
}

func NewPrometheus(cfg config.PrometheusConfig) *Prometheus {
	return &Prometheus{
		cfg: cfg,
		playerCountTotal: promauto.NewGaugeVec(prometheus.GaugeOpts{
			Name: "zander_player_count_total",
			Help: "Total number of players in a given server",
		}, []string{"server_id", "engine"}),
	}
}

func (p *Prometheus) IncPlayerCount(serverID, engine string) {
	p.playerCountTotal.With(prometheus.Labels{
		LabelServerID: serverID,
		LabelEngine:   engine,
	}).Inc()
}

func (p *Prometheus) DecPlayerCount(serverID, engine string) {
	p.playerCountTotal.With(prometheus.Labels{
		LabelServerID: serverID,
		LabelEngine:   engine,
	}).Dec()
}

func (p *Prometheus) SetPlayerCount(serverID, engine string, count uint) {
	p.playerCountTotal.With(prometheus.Labels{
		LabelServerID: serverID,
		LabelEngine:   engine,
	}).Set(float64(count))
}

func (p *Prometheus) Start() error {
	port := 2112
	switch {
	case p.cfg.Port <= 0:
		newPort, err := util.FreeTCPPortFrom(port)
		if err != nil {
			return fmt.Errorf("could not find port for prometheus: %w", err)
		}

		port = newPort
	default:
		port = p.cfg.Port
	}

	path := "/metrics"
	if p.cfg.Path != "" {
		path = p.cfg.Path
	}

	if !strings.HasPrefix(path, "/") {
		path = "/" + path
	}

	var address string

	if net.ParseIP(p.cfg.Address) != nil {
		address = p.cfg.Address

		if strings.Contains(address, ":") && !strings.HasPrefix(address, "[") && !strings.HasSuffix(address, "]") {
			address = fmt.Sprintf("[%s]", address)
		}
	}

	log.Infof("metrics being served at %s:%d%s", address, port, path)

	http.Handle(path, promhttp.Handler())

	return http.ListenAndServe(fmt.Sprintf("%s:%d", address, port), nil)
}
