package handler

import (
	"encoding/json"

	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

func ListServers(manager *zandronum.Manager) Handler {
	return func(recv <-chan message.Message, send chan<- message.Message) error {
		_ = <-recv

		servers := manager.List()
		serversOut := make([]message.Server, 0, len(servers))

		for _, server := range servers {
			serversOut = append(serversOut, message.Server{
				ID:      server.ID,
				Name:    server.Name,
				Status:  server.Status,
				Started: server.Started,
			})
		}

		b, err := json.Marshal(message.ServerList{Servers: serversOut})
		if err != nil {
			return err
		}

		send <- message.Message{
			BodyType: message.SERVER_LIST,
			Body:     b,
		}

		return nil
	}
}
