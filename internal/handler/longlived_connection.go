package handler

import (
	"encoding/json"
	"io"
	"log"
	"net"
	"reflect"
	"time"

	"github.com/google/uuid"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

func HandleConnection(conn net.Conn, server *zandronum.Server) {
	id := uuid.New().String()
	send := make(chan message.Message)
	recv := make(chan message.Message)
	ticker := time.NewTicker(time.Second * 2)

	encoder := json.NewEncoder(conn)
	encoder.SetEscapeHTML(false)
	decoder := json.NewDecoder(conn)

	go func() {
		msg := message.Message{BodyType: message.PING}

		for range ticker.C {
			conn.SetWriteDeadline(time.Now().Add(time.Second * 5))

			if err := encoder.Encode(msg); err != nil {
				ticker.Stop()
				server.Disconnect(id)
				conn.Close()
			}
		}
	}()

	go func() {
		for {
			var msg message.Message

			if err := decoder.Decode(&msg); err != nil {
				if err == io.EOF {
					return
				} else if _, ok := err.(*net.OpError); ok {
					return
				}

				log.Printf("received unknown message: %v", err)
				continue
			}

			recv <- msg
		}
	}()

	go func() {
		for msg := range send {
			if err := encoder.Encode(msg); err != nil {
				log.Println(reflect.TypeOf(err))
				log.Printf("will not send unknown message: %v, %+v", err, msg)
				continue
			}
		}
	}()

	if err := server.Connect(id, send, recv); err != nil {
		log.Println(err)
	}

	ticker.Stop()
	conn.Close()
	close(send)
	close(recv)
}
