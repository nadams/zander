package handler

import (
	"encoding/json"
	"io"
	"log"
	"net"
	"reflect"
	"sync"

	"gitlab.node-3.net/nadams/zander/internal/message"
)

func Handle(conn net.Conn) {
	defer conn.Close()

	var wg sync.WaitGroup
	send := make(chan message.Message)
	recv := make(chan message.Message)

	encoder := json.NewEncoder(conn)
	encoder.SetEscapeHTML(false)
	decoder := json.NewDecoder(conn)

	var msg message.Message

	if err := decoder.Decode(&msg); err != nil {
		log.Printf("got error when waiting for initial command: %v", err)
		return
	}

	handler, found := handlers[msg.BodyType]
	if !found {
		log.Println("handler for body type %v not found", msg.BodyType)
		return
	}

	wg.Add(1)
	go func() {
		defer func() {
			close(recv)
			close(send)

			wg.Done()
		}()

		if err := handler(recv, send); err != nil {
			log.Println("error from handler: %v", err)
		}
	}()

	wg.Add(1)
	go func() {
		defer wg.Done()

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

			log.Printf("got message: %+v", msg)

			recv <- msg
			//switch msg.BodyType {
			//case message.DISCONNECT:
			//  return
			//default:
			//  recv <- msg
			//}
		}
	}()

	recv <- msg

	for msg := range send {
		if err := encoder.Encode(msg); err != nil {
			log.Println(reflect.TypeOf(err))
			log.Printf("will not send unknown message: %v, %+v", err, msg)
			continue
		}
	}

	wg.Wait()
}
