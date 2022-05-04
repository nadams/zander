package zandronum

import (
	"bufio"
	"fmt"
	"io"
	"log"
	"os/exec"
	"sync"
	"syscall"
)

type Server struct {
	sync.RWMutex

	cmd       *exec.Cmd
	content   []byte
	stdout    io.ReadCloser
	stdin     io.WriteCloser
	consumers map[string]chan<- []byte
}

func NewServer(binary string, opts map[string]string) *Server {
	cmd := exec.Command(binary)

	return &Server{
		cmd:       cmd,
		consumers: make(map[string]chan<- []byte),
	}
}

func (s *Server) Start() error {
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

	go func() {
		scanner := bufio.NewScanner(s.stdout)
		for scanner.Scan() {
			b := scanner.Bytes()

			s.content = append(s.content, b...)
			s.content = append(s.content, '\n')

			s.RLock()
			for _, consumer := range s.consumers {
				consumer <- b
			}
			s.RUnlock()
		}

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

func (s *Server) Connect(id string, w io.Writer, r io.Reader) error {
	if s.cmd != nil {
		consumer := make(chan []byte)

		s.Lock()
		s.consumers[id] = consumer
		s.Unlock()

		go func() {
			scanner := bufio.NewScanner(r)
			for scanner.Scan() {
				b := scanner.Bytes()

				s.stdin.Write(b)
				s.stdin.Write([]byte("\n"))
			}
		}()

		w.Write(s.content)

		for line := range consumer {
			w.Write(line)
			w.Write([]byte{'\n'})
		}
	}

	return nil
}

func (s *Server) Disconnect(id string) {
	log.Printf("client %s disconnecting", id)

	s.Lock()
	defer s.Unlock()

	for key, consumer := range s.consumers {
		if key == id {
			close(consumer)

			delete(s.consumers, id)
		}
	}
}
