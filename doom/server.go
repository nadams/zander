package doom

import (
	"bufio"
	"fmt"
	"io"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"sync"
	"syscall"
	"time"

	log "github.com/sirupsen/logrus"
	"gitlab.node-3.net/nadams/zander/config"
)

type ServerStatus string

const (
	Running    ServerStatus = "running"
	Stopped                 = "stopped"
	Errored                 = "errored"
	NotStarted              = "not started"
)

type Server struct {
	m         sync.RWMutex
	binary    string
	waddir    string
	opts      map[string]string
	cfg       config.Server
	cmd       *exec.Cmd
	content   []byte
	stdout    io.ReadCloser
	stdin     io.WriteCloser
	consumers map[string]chan<- []byte
	started   time.Time
	stopped   time.Time
}

func NewServer(binary string, opts map[string]string) *Server {
	cmd := exec.Command(binary)

	return &Server{
		binary:    binary,
		opts:      opts,
		cmd:       cmd,
		consumers: make(map[string]chan<- []byte),
	}
}

func NewServerWithConfig(binary, waddir string, cfg config.Server) *Server {
	params, err := cfg.Parameters()
	if err != nil {
		panic(err)
	}

	cvars, err := cfg.CVARs()
	if err != nil {
		panic(err)
	}

	f, err := os.OpenFile(filepath.Join(os.TempDir(), fmt.Sprintf("%s.cfg", cfg.ID)), os.O_TRUNC|os.O_CREATE|os.O_RDWR, 0600)
	if err != nil {
		panic(err)
	}

	defer f.Close()

	if _, err := io.Copy(f, strings.NewReader(cvars)); err != nil {
		panic(err)
	}

	params = append(params, "+exec", f.Name())

	switch strings.ToLower(cfg.Mode) {
	case "ctf":
		params = append(params, "+ctf 1")
	case "1ctf":
		params = append(params, "+oneflagctf 1")
	case "skulltag":
		params = append(params, "+skulltag 1")
	case "duel":
		params = append(params, "+duel 1")
	case "teamgame":
		params = append(params, "+teamgame 1")
	case "domination":
		params = append(params, "+domination 1")
	case "survival":
		params = append(params, "+survival 1")
	case "invasion":
		params = append(params, "+invasion 1")
	case "cooperative":
		params = append(params, "+cooperative 1")
	case "dm":
		params = append(params, "+deathmatch 1")
	case "tdm":
		params = append(params, "+teamplay 1")
	case "terminator":
		params = append(params, "+terminator 1")
	case "possession":
		params = append(params, "+possession 1")
	case "tpossession":
		params = append(params, "+teampossession 1")
	case "lms":
		params = append(params, "+lastmanstanding 1")
	case "tlms":
		params = append(params, "+teamlms 1")
	default:
		cfg.Mode = "dm"
		params = append(params, "+deathmatch 1")
	}

	cmd := exec.Command(binary, params...)
	cmd.Env = append(cmd.Env, fmt.Sprintf("DOOMWADDIR=%s", waddir))

	return &Server{
		binary:    binary,
		waddir:    waddir,
		cmd:       cmd,
		cfg:       cfg,
		consumers: make(map[string]chan<- []byte),
	}
}

func (s *Server) Start() error {
	s.started = time.Now()

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

	if err := s.cmd.Start(); err != nil {
		return fmt.Errorf("could not start server: %w", err)
	}

	go s.cmd.Wait()

	go func() {
		scanner := bufio.NewScanner(s.stdout)
		for scanner.Scan() {
			b := scanner.Bytes()
			b = append(b, '\n')

			s.content = append(s.content, b...)

			s.m.RLock()
			for _, consumer := range s.consumers {
				consumer <- b
			}
			s.m.RUnlock()
		}

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
	}()

	return nil
}

func (s *Server) Stop() error {
	if s.cmd != nil {
		s.stopped = time.Now()

		if s.stdin != nil {
			s.stdin.Close()
		}

		if s.stdout != nil {
			s.stdout.Close()
		}

		return s.cmd.Process.Signal(syscall.SIGTERM)
	}

	return nil
}

func (s *Server) Connect(id string, send chan<- []byte, recv <-chan []byte) error {
	log.Infof("client %s connecting", id)

	if s.cmd != nil {
		return s.attach(id, send, recv)
	}

	return nil
}

func (s *Server) Disconnect(id string) {
	log.Infof("client %s disconnecting", id)

	s.m.Lock()
	defer s.m.Unlock()

	if consumer, ok := s.consumers[id]; ok {
		close(consumer)

		delete(s.consumers, id)
	}
}

func (s *Server) Status() ServerStatus {
	switch {
	case s.started == time.Time{}:
		return NotStarted
	case s.cmd.ProcessState != nil && s.stopped == time.Time{}:
		return Errored
	case s.stopped != time.Time{}:
		return Stopped
	case s.cmd.ProcessState == nil:
		return Running
	default:
		return ""
	}
}

func (s *Server) attach(id string, send chan<- []byte, recv <-chan []byte) error {
	if s.cmd.ProcessState != nil {
		send <- s.content

		return nil
	}

	consumer := make(chan []byte)

	s.m.Lock()
	s.consumers[id] = consumer
	s.m.Unlock()

	go func() {
		for msg := range recv {
			s.stdin.Write([]byte(msg))
			s.stdin.Write([]byte{'\n'})
		}
	}()

	send <- s.content

	for line := range consumer {
		send <- line
	}

	return nil
}
