package handler

import (
	"encoding/json"
	"fmt"
	"log"

	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

func Attach(manager *zandronum.Manager) Handler {
	return func(recv <-chan message.Message, send chan<- message.Message) error {
		attachCmd := <-recv

		var id string
		json.Unmarshal(attachCmd.Body, &id)

		log.Println("got attach cmd for server: %+v", id)

		server, found := manager.Get(zandronum.ID(id))
		if !found {
			return fmt.Errorf("server not found")
		}

		// attach to server

		return nil
	}
}
