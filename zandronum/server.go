package zandronum

import (
	"bufio"
	"fmt"
	"io"
	"log"
	"os/exec"
	"sync"
	"syscall"
	"time"
)

type Server struct {
	m         sync.RWMutex
	cmd       *exec.Cmd
	content   []byte
	stdout    io.ReadCloser
	stdin     io.WriteCloser
	consumers map[string]chan<- []byte
	started   time.Time
}

func NewServer(binary string, opts map[string]string) *Server {
	cmd := exec.Command(binary)

	return &Server{
		cmd:       cmd,
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
	if s.cmd != nil {
		return s.attach(id, send, recv)
	}

	return nil
}

func (s *Server) Disconnect(id string) {
	log.Printf("client %s disconnecting", id)

	s.m.Lock()
	defer s.m.Unlock()

	if consumer, ok := s.consumers[id]; ok {
		close(consumer)

		delete(s.consumers, id)
	}
}

func (s *Server) Status() string {
	switch {
	case s.cmd == nil:
		return "not started"

	case s.cmd.ProcessState != nil:
		return "stopped"

	case s.cmd.ProcessState == nil && s.cmd != nil:
		return "running"

	default:
		return ""
	}
}

func (s *Server) attach(id string, send chan<- []byte, recv <-chan []byte) error {
	fmt.Printf("%+v\n", s.cmd.ProcessState)
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
