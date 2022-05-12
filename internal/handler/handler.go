package handler

import (
	"log"
	"sync"

	"gitlab.node-3.net/nadams/zander/internal/message"
)

type Handler func(recv <-chan message.Message, send chan<- message.Message) error

var (
	m        sync.Mutex
	handlers map[message.BodyType]Handler
)

func init() {
	handlers = make(map[message.BodyType]Handler)
}

func Register(t message.BodyType, handler Handler) {
	m.Lock()
	defer m.Unlock()

	if _, found := handlers[t]; found {
		log.Printf("warning: handler for type %v is already registered", t)
	}

	handlers[t] = handler
}
