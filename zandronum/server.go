package zandronum

import (
	"bytes"
	"fmt"
	"io"
	"log"
	"os/exec"
	"syscall"
)

type Server struct {
	cmd       *exec.Cmd
	stdoutBuf bytes.Buffer
	stdout    io.ReadCloser
	stdin     io.WriteCloser
	teeStdout io.Reader
}

func NewServer(binary string, opts map[string]string) *Server {
	cmd := exec.Command(binary, "+dm", "0", "+sv_forcepassword", "false", "-iwad", "/home/nick/wads/doom2.wad")

	return &Server{
		cmd: cmd,
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
		log.Println("copying stdout to buffer")
		_, err := io.Copy(&s.stdoutBuf, s.stdout)
		fmt.Println("stdoutbuf copy error (any)", err)
		log.Println("done")
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

func (s *Server) Connect(w io.Writer, r io.Reader) error {
	if s.cmd != nil {
		go func() {
			fmt.Println(io.Copy(s.stdin, r))
		}()

		io.Copy(w, &s.stdoutBuf)
		io.Copy(w, s.stdout)
	}

	return nil
}
