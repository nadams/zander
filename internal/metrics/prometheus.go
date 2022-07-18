package metrics

import (
	"fmt"
	"net/http"
	"strings"

	"github.com/prometheus/client_golang/prometheus/promhttp"
	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
)

var _ Metrics = (*Prometheus)(nil)

type Prometheus struct {
	cfg config.PrometheusConfig
}

func (p *Prometheus) Start() error {
	port := 2112
	if p.cfg.Port > 0 {
		port = p.cfg.Port
	}

	path := "/metrics"
	if p.cfg.Path != "" {
		path = p.cfg.Path
	}

	if !strings.HasPrefix(path, "/") {
		path = "/" + path
	}

	log.Infof("metrics being served at :%d%s", port, path)

	http.Handle(path, promhttp.Handler())

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
