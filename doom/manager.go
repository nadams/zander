package doom

import (
	"errors"
	"fmt"
	"os"
	"path/filepath"
	"sort"
	"strings"
	"sync"
	"time"

	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
	"gitlab.node-3.net/zander/zander/internal/util"
)

var (
	ErrServerNotFound = errors.New("server not found")
)

var binaryPaths = map[config.Engine]string{}

type Manager struct {
	m             sync.RWMutex
	servers       map[ID]Server
	metrics       metrics.Metrics
	memoryMetrics *metrics.Memory
}

type ManagerOpt func(m *Manager)

func WithMetrics(c metrics.Metrics) ManagerOpt {
	return func(m *Manager) {
		m.metrics = c
	}
}

type ID string

func NewManager(memoryMetrics *metrics.Memory, opts ...ManagerOpt) *Manager {
	m := &Manager{
		servers:       map[ID]Server{},
		memoryMetrics: memoryMetrics,
	}

	for _, opt := range opts {
		opt(m)
	}

	return m
}

func (m *Manager) Add(id ID, server Server) ID {
	m.add(id, server)

	return id
}

func (m *Manager) StartAll() []error {
	m.m.RLock()
	defer m.m.RUnlock()

	var errs []error

	for id, server := range m.servers {
		if err := server.Start(); err != nil {
			log.Errorf("error starting server: %s", err.Error())
			errs = append(errs, err)
			continue
		}

		log.Infof("started server %s", id)
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

func (m *Manager) Get(id ID) (Server, bool) {
	server, found := m.servers[id]

	return server, found
}

func (m *Manager) Watch() {
	for range time.NewTicker(time.Second).C {
		for id, server := range m.servers {
			if server.Config().RestartPolicy == config.OnFailure && server.Info().Status == Errored {
				log.Infof("found server to restart: %v", id)

				m.Restart(id)
			}
		}
	}
}

func (m *Manager) Metrics() *metrics.Memory {
	return m.memoryMetrics
}

func (m *Manager) remove(id ID) {
	delete(m.servers, id)
}

func (m *Manager) add(id ID, server Server) {
	m.servers[id] = server
}

func Load(cfg config.Config) (*Manager, error) {
	mem := metrics.NewMemory()
	coll := []metrics.Metrics{mem}

	mcfg := cfg.Metrics
	if mcfg.Prometheus != nil && mcfg.Prometheus.Enabled {
		prom := metrics.NewPrometheus(*mcfg.Prometheus)

		go func() {
			if err := prom.Start(); err != nil {
				panic(err)
			}
		}()

		coll = append(coll, prom)
	}

	if mcfg.StatsD != nil && mcfg.StatsD.Enabled {
		sd := metrics.NewStatsD(*mcfg.StatsD)

		go func() {
			if err := sd.Open(); err != nil {
				panic(err)
			}
		}()

		coll = append(coll, sd)
	}

	met := metrics.NewMulti(coll...)
	m := NewManager(mem, WithMetrics(met))

	dir := cfg.ExpandRel(cfg.ServerConfigDir)

	if _, err := os.Stat(dir); err != nil {
		if err := os.MkdirAll(dir, 0755); err != nil {
			return nil, err
		}
	}

	entries, err := os.ReadDir(dir)
	if err != nil {
		return nil, err
	}

	sort.Slice(entries, func(i, j int) bool {
		return entries[i].Name() < entries[j].Name()
	})

	servers, err := m.LoadServers(cfg)
	if err != nil {
		return nil, err
	}

	for _, server := range servers {
		m.Add(ID(server.Config().ID), server)
	}

	return m, nil
}

func (m *Manager) LoadServers(cfg config.Config) ([]Server, error) {
	dir := cfg.ExpandRel(cfg.ServerConfigDir)

	if _, err := os.Stat(dir); err != nil {
		if err := os.MkdirAll(dir, 0755); err != nil {
			return nil, err
		}
	}

	entries, err := os.ReadDir(dir)
	if err != nil {
		return nil, err
	}

	sort.Slice(entries, func(i, j int) bool {
		return entries[i].Name() < entries[j].Name()
	})

	var servers []Server

	for _, entry := range entries {
		if !entry.IsDir() && strings.HasSuffix(entry.Name(), ".toml") {
			scfg, err := config.LoadServer(filepath.Join(dir, entry.Name()))
			if err != nil {
				return nil, err
			}

			server, err := m.serverFromConfig(cfg, scfg)
			if err != nil {
				return nil, err
			}

			servers = append(servers, server)
		}
	}

	return servers, nil
}

func (m *Manager) Reload(cfg config.Config) error {
	newServers, err := m.LoadServers(cfg)
	if err != nil {
		return fmt.Errorf("could not reload servers: %w", err)
	}

	oldCfgs := util.NewSet[ID, config.Server]()

	for _, s := range m.servers {
		cfg := s.Config()

		oldCfgs.Put(ID(cfg.ID), cfg)
	}

	newCfgs := util.NewSet[ID, config.Server]()

	for _, s := range newServers {
		cfg := s.Config()

		newCfgs.Put(ID(cfg.ID), cfg)
	}

	n := newCfgs.Difference(oldCfgs)
	r := oldCfgs.Difference(newCfgs)
	e := newCfgs.Intersection(oldCfgs)

	m.m.Lock()
	defer m.m.Unlock()

	for id, c := range n.Entries() {
		ns, err := m.serverFromConfig(cfg, c)
		if err != nil {
			return err
		}

		m.Add(id, ns)
	}

	for i, neew := range e.Entries() {
		old, found := m.Get(i)
		if !found {
			return fmt.Errorf("should have found server config for comparison but didn't")
		}

		if neew.Equals(old.Config()) {
			e.Del(i)
			continue
		}

		ns, err := m.serverFromConfig(cfg, neew)
		if err != nil {
			return err
		}

		id := ID(old.Config().ID)

		m.Stop(id)
		m.remove(id)
		m.Add(id, ns)
	}

	for _, id := range r.Keys() {
		m.Stop(id)
		m.remove(id)
	}

	for _, k := range n.Keys() {
		m.Start(k)
	}

	for _, k := range e.Keys() {
		m.Start(k)
	}

	return nil
}

func (m *Manager) serverFromConfig(cfg config.Config, scfg config.Server) (Server, error) {
	var server Server
	var err error

	switch scfg.Engine {
	case config.Zandronum:
		if _, found := binaryPaths[config.Zandronum]; !found {
			zandbinary := cfg.Expand(cfg.ServerBinaries.Zandronum)
			if !cfg.Exists(zandbinary) {
				return nil, fmt.Errorf("server binary %s not found", zandbinary)
			}

			binaryPaths[config.Zandronum] = zandbinary
		}

		server, err = NewZandronumServer(binaryPaths[config.Zandronum], cfg.WADPaths, scfg, m.metrics)
	case config.Odamex:
		if _, found := binaryPaths[config.Odamex]; !found {
			odabinary := cfg.Expand(cfg.ServerBinaries.Odamex)
			if !cfg.Exists(odabinary) {
				return nil, fmt.Errorf("server binary %s not found", odabinary)
			}

			binaryPaths[config.Odamex] = odabinary
		}

		server, err = NewOdamexServer(binaryPaths[config.Odamex], cfg.WADPaths, scfg, m.metrics)
	default:
		err = fmt.Errorf("unknown engine: '%s'", scfg.Engine)
	}

	return server, err
}
