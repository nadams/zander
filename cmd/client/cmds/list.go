package cmds

import (
	"encoding/json"
	"fmt"
	"log"

	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type ListCmd struct{}

func (l *ListCmd) Run(socket string) error {
	client := zandronum.NewClient(socket)
	if err := client.Open(); err != nil {
		return err
	}

	defer client.Close()

	client.Send() <- message.Message{BodyType: message.CMD_LIST_SERVERS}

	for msg := range client.Recv() {
		if msg.BodyType == message.SERVER_LIST {
			var body message.ServerList

			if err := json.Unmarshal(msg.Body, &body); err != nil {
				log.Println(err)
				break
			}

			fmt.Println(body)
			break
		}
	}

	return nil
}
