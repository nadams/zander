package doom

import (
	"errors"
	"fmt"
	"io/ioutil"
	"os"
	"path/filepath"
	"sort"
	"strings"
	"sync"
	"time"

	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
)

var (
	ErrServerNotFound = errors.New("server not found")
)

type Manager struct {
	m       sync.RWMutex
	servers map[ID]*Server
	metrics metrics.Metrics
}

type ManagerOpt func(m *Manager)

func WithMetrics(c metrics.Metrics) ManagerOpt {
	return func(m *Manager) {
		m.metrics = c
	}
}

type ID string

func NewManager(opts ...ManagerOpt) *Manager {
	m := &Manager{
		servers: map[ID]*Server{},
	}

	for _, opt := range opts {
		opt(m)
	}

	return m
}

func (m *Manager) Add(id ID, server *Server) ID {
	m.add(id, server)

	return id
}

func (m *Manager) StartAll() []error {
	m.m.RLock()
	defer m.m.RUnlock()

	var errs []error

	for _, server := range m.servers {
		if server.cfg.Disabled {
			continue
		}

		if err := server.Start(); err != nil {
			errs = append(errs, err)
			continue
		}
	}

	return errs
}

func (m *Manager) Start(id ID) error {
	server, found := m.servers[id]
	if found {
		return server.Start()
	}

	return ErrServerNotFound
}

func (m *Manager) Stop(id ID) error {
	server, found := m.servers[id]
	if found {
		return server.Stop()
	}

	return ErrServerNotFound
}

func (m *Manager) Restart(id ID) error {
	m.m.Lock()
	defer m.m.Unlock()

	if server, found := m.servers[id]; found {
		server.Stop()

		newServer, err := server.Copy()
		if err != nil {
			return err
		}

		m.remove(id)
		m.add(id, newServer)

		return m.Start(id)
	}

	return nil
}

func (m *Manager) List() []ServerInfo {
	m.m.RLock()
	defer m.m.RUnlock()

	out := make([]ServerInfo, 0, len(m.servers))

	for _, server := range m.servers {
		out = append(out, server.Info())
	}

	return out
}

func (m *Manager) Get(id ID) (*Server, bool) {
	server, found := m.servers[id]

	return server, found
}

func (m *Manager) Watch() {
	for range time.NewTicker(time.Second).C {
		for id, server := range m.servers {
			if server.cfg.RestartPolicy == config.OnFailure && server.Status() == Errored {
				log.Infof("found server to restart: %v", id)

				m.Restart(id)
			}
		}
	}
}

func (m *Manager) remove(id ID) {
	delete(m.servers, id)
}

func (m *Manager) add(id ID, server *Server) {
	m.servers[id] = server
}

func Load(cfg config.Config) (*Manager, error) {
	var met metrics.Metrics = &metrics.Noop{}

	if mcfg := cfg.Metrics; mcfg.Collector != "" {
		switch mcfg.Collector {
		case config.Prometheus:
			prom := &metrics.Prometheus{}
			go func() {
				if err := prom.Start(); err != nil {
					panic(err)
				}

				met = prom
			}()

		default:
			met = &metrics.Noop{}

			log.Warnf("invalid collector configured \"%s\", using no-op collector", mcfg.Collector)
		}
	}

	m := NewManager(WithMetrics(met))
	zandbinary := cfg.Expand(cfg.ServerBinaries.Zandronum)
	if !cfg.Exists(zandbinary) {
		return nil, fmt.Errorf("server binary %s not found", zandbinary)
	}

	dir := cfg.ExpandRel(cfg.ServerConfigDir)
	waddir := cfg.ExpandRel(cfg.WADDir)

	if _, err := os.Stat(dir); err != nil {
		if err := os.MkdirAll(dir, 0755); err != nil {
			return nil, err
		}
	}

	entries, err := ioutil.ReadDir(dir)
	if err != nil {
		return nil, err
	}

	sort.Slice(entries, func(i, j int) bool {
		return entries[i].Name() < entries[j].Name()
	})

	for _, entry := range entries {
		if !entry.IsDir() && strings.HasSuffix(entry.Name(), ".toml") {
			cfg, err := config.LoadServer(filepath.Join(dir, entry.Name()))
			if err != nil {
				return nil, err
			}

			server, err := NewServer(zandbinary, waddir, cfg)
			if err != nil {
				return nil, err
			}

			m.Add(ID(cfg.ID), server)
		}
	}

	return m, nil
}
