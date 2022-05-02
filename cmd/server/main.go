package main

import (
	"fmt"
	"log"
	"net"
	"os"

	"gitlab.node-3.net/nadams/zander/zandronum"
)

const socketAddr = "/tmp/zander.sock"

func main() {
	//sigs := make(chan os.Signal, 1)

	//signal.Notify(sigs, syscall.SIGTERM, syscall.SIGINT)

	//<-sigs
	//log.Println("shutting down")

	//log.Println(server.Stop())
	if err := ListenAndServe(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
}

func ListenAndServe() error {
	if err := os.RemoveAll(socketAddr); err != nil {
		return fmt.Errorf("could not remove zander socket: %w", err)
	}

	l, err := net.Listen("unix", socketAddr)
	if err != nil {
		return fmt.Errorf("could not create socket: %w", err)
	}

	defer l.Close()

	server := zandronum.NewServer("/usr/bin/zandronum-server", nil)
	if err := server.Start(); err != nil {
		log.Println(err)
	}

	for {
		conn, err := l.Accept()
		if err != nil {
			log.Println(err)
		}

		go connect(conn, server)
	}
}

func connect(conn net.Conn, server *zandronum.Server) {
	if err := server.Connect(conn, conn); err != nil {
		log.Println(err)
	}
}
