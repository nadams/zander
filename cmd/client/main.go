package main

import (
	"fmt"
	"log"
	"net"
)

func main() {
	if err := listen(); err != nil {
		log.Fatal(err)
	}
}

func listen() error {
	conn, err := net.Dial("unix", "/tmp/zander.sock")
	if err != nil {
		return fmt.Errorf("could not connect to zander: %w", err)
	}

	defer conn.Close()

	for {
	}

	return nil
}
