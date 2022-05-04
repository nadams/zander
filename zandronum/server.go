package zandronum

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"os/exec"
	"sync"
	"syscall"

	"gitlab.node-3.net/nadams/zander/internal/message"
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

	go s.cmd.Wait()

	go func() {
		scanner := bufio.NewScanner(s.stdout)
		for scanner.Scan() {
			b := scanner.Bytes()
			b = append(b, '\n')

			s.content = append(s.content, b...)

			s.RLock()
			for _, consumer := range s.consumers {
				consumer <- b
			}
			s.RUnlock()
		}

		s.Lock()
		defer s.Unlock()

		for _, consumer := range s.consumers {
			close(consumer)
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

func (s *Server) Connect(id string, send chan<- message.Message, recv <-chan message.Message) error {
	if s.cmd != nil {
		if s.cmd.ProcessState != nil {
			b, _ := json.Marshal(string(s.content))
			send <- message.Message{
				BodyType: message.LINE,
				Body:     b,
			}

			return nil
		}

		consumer := make(chan []byte)

		s.Lock()
		s.consumers[id] = consumer
		s.Unlock()

		go func() {
			for msg := range recv {
				if msg.BodyType == message.LINE {
					var body string
					if err := json.Unmarshal(msg.Body, &body); err != nil {
						log.Println(err)
					}

					s.stdin.Write([]byte(body))
					s.stdin.Write([]byte{'\n'})
				}
			}
		}()

		b, _ := json.Marshal(string(s.content))
		send <- message.Message{
			BodyType: message.LINE,
			Body:     b,
		}

		for line := range consumer {
			b, _ = json.Marshal(string(line))

			send <- message.Message{
				BodyType: message.LINE,
				Body:     b,
			}
		}
	}

	return nil
}

func (s *Server) Disconnect(id string) {
	log.Printf("client %s disconnecting", id)

	s.Lock()
	defer s.Unlock()

	if consumer, ok := s.consumers[id]; ok {
		close(consumer)

		delete(s.consumers, id)
	}
}
