package util

import (
	"errors"
	"fmt"
	"net"
)

func FreeTCPPort() (int, error) {
	addr, err := net.ResolveTCPAddr("tcp", "localhost:0")
	if err != nil {
		return 0, err
	}

	l, err := net.ListenTCP("tcp", addr)
	if err != nil {
		return 0, err
	}

	defer l.Close()

	return l.Addr().(*net.TCPAddr).Port, nil
}

func FreeTCPPortFrom(start int) (int, error) {
	if start <= 0 {
		return 0, errors.New("invalid start port")
	}

	for {
		addr, err := net.ResolveTCPAddr("tcp", fmt.Sprintf("localhost:%d", start))
		if err == nil {
			l, err := net.ListenTCP("tcp", addr)
			if err == nil {
				defer l.Close()

				return l.Addr().(*net.TCPAddr).Port, nil
			}
		}
	}
}
