package zandronum

import (
	"errors"
	"sync"
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

func (m *Manager) Add(server *Server) ID {
	//id := ID(uuid.New().String())
	id := ID("1")
	m.add(id, server)

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

func (m *Manager) Restart(id ID) error {
	server, found := m.servers[id]
	if found {
		server.Stop()

		newServer := NewServer(server.binary, server.opts)
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
			Status:  server.Status(),
			Started: server.started,
		})
	}

	return out
}

func (m *Manager) Get(id ID) (*Server, bool) {
	server, found := m.servers[id]

	return server, found
}

func (m *Manager) remove(id ID) {
	m.m.Lock()
	defer m.m.Unlock()

	delete(m.servers, id)
}

func (m *Manager) add(id ID, server *Server) {
	m.m.Lock()
	defer m.m.Unlock()

	m.servers[id] = server
}
