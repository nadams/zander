package handler

import (
	"encoding/json"
	"fmt"
	"log"

	"github.com/google/uuid"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

func Attach(manager *zandronum.Manager) Handler {
	return func(recv <-chan message.Message, send chan<- message.Message) error {
		attachCmd := <-recv

		var serverID string
		json.Unmarshal(attachCmd.Body, &serverID)

		log.Printf("got attach cmd for server: %+v", serverID)

		server, found := manager.Get(zandronum.ID(serverID))
		if !found {
			return fmt.Errorf("server not found")
		}

		clientID := uuid.New().String()

		defer server.Disconnect(clientID)

		return server.Connect(clientID, send, recv)
	}
}
