package zandronum

import (
	"errors"
	"sync"

	"github.com/google/uuid"
)

var (
	ErrServerNotFound = errors.New("server not found")
)

type Manager struct {
	sync.RWMutex

	servers map[ID]*Server
}

type ID string

func NewManager() *Manager {
	return &Manager{
		servers: map[ID]*Server{},
	}
}

func (m *Manager) Add(server *Server) ID {
	m.Lock()
	defer m.Unlock()

	id := ID(uuid.New().String())

	m.servers[id] = server

	return id
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

func (m *Manager) List() []ServerInfo {
	m.RLock()
	defer m.RUnlock()

	out := make([]ServerInfo, 0, len(m.servers))

	for key := range m.servers {
		out = append(out, ServerInfo{
			ID: string(key),
		})
	}

	return out
}
