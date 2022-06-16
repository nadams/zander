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
)

var (
	ErrServerNotFound = errors.New("server not found")
)

type Manager struct {
	m       sync.RWMutex
	servers map[ID]*Server
}

type ID string

func NewManager() *Manager {
	return &Manager{
		servers: map[ID]*Server{},
	}
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

		newServer, err := NewServer(server.binary, server.waddir, server.cfg)
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

	for key, server := range m.servers {
		out = append(out, ServerInfo{
			ID:      string(key),
			Name:    server.cfg.Hostname,
			Mode:    server.cfg.Mode,
			Status:  string(server.Status()),
			Port:    server.cfg.Port,
			IWAD:    server.cfg.IWAD,
			PWADs:   server.cfg.PWADs,
			Started: server.started,
			Stopped: server.stopped,
		})
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
	m := NewManager()
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
