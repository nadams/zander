package handler

import (
	"encoding/json"
	"fmt"

	"github.com/google/uuid"
	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

func Attach(manager *zandronum.Manager) Handler {
	return func(recv <-chan message.Message, send chan<- message.Message) error {
		attachCmd := <-recv

		var serverID string
		if err := json.Unmarshal(attachCmd.Body, &serverID); err != nil {
			return err
		}

		server, found := manager.Get(zandronum.ID(serverID))
		if !found {
			return fmt.Errorf("server not found")
		}

		clientID := uuid.New().String()

		recv2 := make(chan message.Message)

		go func() {
			for msg := range recv {
				recv2 <- msg
			}

			server.Disconnect(clientID)
		}()

		return server.Connect(clientID, send, recv2)
	}
}
