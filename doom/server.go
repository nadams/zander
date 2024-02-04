package doom

import (
	"bufio"
	"fmt"
	"io"
	"os/exec"
	"sync"
	"syscall"
	"time"

	log "github.com/sirupsen/logrus"
	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
)

var emptyTime = time.Time{}

type ConnectOpts struct {
	ID    string
	Lines int
}

type Server interface {
	Start() error
	Stop() error
	Connect(opts ConnectOpts, send chan<- []byte, recv <-chan []byte) error
	Disconnect(id string)
	Info() ServerInfo
	Copy() (Server, error)
	Config() config.Server
	Logs(n int) []string
}

type logMapper func([]byte) []byte

type server struct {
	m                  sync.RWMutex
	binary             string
	wadPaths           config.WADPaths
	cfg                config.Server
	cmd                *exec.Cmd
	content            *LogBuffer
	stdout             io.ReadCloser
	stdin              io.WriteCloser
	consumers          map[string]chan<- []byte
	started            time.Time
	stopped            time.Time
	metrics            metrics.Metrics
	foundAlternatePort bool
	logMappers         []logMapper
	preStart           func() error
}

func newServer(binary string, wadPaths config.WADPaths, cfg config.Server, metrics metrics.Metrics) *server {
	return &server{
		binary:    binary,
		wadPaths:  wadPaths,
		cfg:       cfg,
		metrics:   metrics,
		consumers: make(map[string]chan<- []byte),
		content:   NewLogBuffer(cfg.MaxLogLines),
	}
}

func (s *server) Start() error {
	if s.cfg.Disabled {
		return nil
	}

	if s.stopped != emptyTime {
		if s.preStart != nil {
			if err := s.preStart(); err != nil {
				return err
			}
		}
	}

	stdout, err := s.cmd.StdoutPipe()
	if err != nil {
		return fmt.Errorf("could not create server stdout pipe: %w", err)
	}

	stdin, err := s.cmd.StdinPipe()
	if err != nil {
		return fmt.Errorf("could not create server stdin pipe: %w", err)
	}

	s.stdout = stdout
	s.stdin = stdin
	s.started = time.Now()
	s.stopped = emptyTime

	if err := s.cmd.Start(); err != nil {
		return fmt.Errorf("could not start server: %w", err)
	}

	s.metrics.SetPlayerCount(s.cfg.ID, string(s.cfg.Engine), 0)

	go s.cmd.Wait()
	go func() {
		s.scanStdout()
		s.scanCleanup()
	}()

	return nil
}

func (s *server) scanStdout() {
	scanner := bufio.NewScanner(s.stdout)
	for scanner.Scan() {
		b := scanner.Bytes()

		for _, mapper := range s.logMappers {
			if mapper != nil {
				b = mapper(b)
			}
		}

		s.content.Write(b)

		s.m.RLock()
		for _, consumer := range s.consumers {
			consumer <- b
		}
		s.m.RUnlock()
	}
}

func (s *server) scanCleanup() {
	s.m.Lock()
	defer s.m.Unlock()

	consIDs := make([]string, 0, len(s.consumers))
	cons := make([]chan<- []byte, 0, len(s.consumers))

	for key, consumer := range s.consumers {
		consIDs = append(consIDs, key)
		cons = append(cons, consumer)
	}

	for _, id := range consIDs {
		delete(s.consumers, id)
	}

	for _, consumer := range cons {
		close(consumer)
	}

	s.Stop()

}

func (s *server) Stop() error {
	if s.cmd != nil {
		defer s.metrics.SetPlayerCount(s.cfg.ID, "zandronum", 0)

		s.stopped = time.Now()

		if s.stdin != nil {
			s.stdin.Close()
		}

		if s.stdout != nil {
			s.stdout.Close()
		}

		if s.cmd.Process != nil {
			return s.cmd.Process.Signal(syscall.SIGTERM)
		}
	}

	return nil
}

func (s *server) Connect(opts ConnectOpts, send chan<- []byte, recv <-chan []byte) error {
	log.Infof("client %s connecting", opts.ID)

	if s.cmd != nil {
		return s.attach(opts, send, recv)
	}

	return nil
}

func (s *server) Disconnect(id string) {
	log.Infof("client %s disconnecting", id)

	s.m.Lock()
	defer s.m.Unlock()

	if consumer, ok := s.consumers[id]; ok {
		close(consumer)

		delete(s.consumers, id)
	}
}

func (s *server) Info() ServerInfo {
	return ServerInfo{
		ID:      string(s.cfg.ID),
		Name:    s.cfg.Hostname,
		Engine:  s.cfg.Engine,
		Mode:    s.cfg.Mode,
		Status:  string(s.status()),
		Port:    s.cfg.Port,
		IWAD:    s.cfg.IWAD,
		PWADs:   s.cfg.PWADs,
		Started: s.started,
		Stopped: s.stopped,
	}
}

func (s *server) Config() config.Server {
	return s.cfg
}

func (s *server) status() ServerStatus {
	switch {
	case s.cfg.Disabled:
		return Disabled
	case s.started == emptyTime:
		return NotStarted
	case s.cmd.ProcessState != nil && s.stopped == emptyTime:
		return Errored
	case s.stopped != emptyTime:
		return Stopped
	case s.cmd.ProcessState == nil:
		return Running
	default:
		return ""
	}
}

func (s *server) attach(opts ConnectOpts, send chan<- []byte, recv <-chan []byte) error {
	if s.cmd.ProcessState != nil {
		send <- s.content.Bytes(opts.Lines)

		return nil
	}

	consumer := make(chan []byte)

	s.m.Lock()
	s.consumers[opts.ID] = consumer
	s.m.Unlock()

	go func() {
		for msg := range recv {
			s.stdin.Write([]byte(msg))
			s.stdin.Write([]byte{'\n'})
		}
	}()

	send <- s.content.Bytes(opts.Lines)

	for line := range consumer {
		send <- line
	}

	return nil
}

func (s *server) Logs(n int) []string {
	return s.content.Lines(n)
}

func (s *server) resetPlayerCount() {
	s.metrics.SetPlayerCount(s.cfg.ID, string(s.cfg.Engine), 0)
}
