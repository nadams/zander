package cmds

import (
	"bufio"
	"encoding/json"
	"fmt"
	"os"

	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type AttachCmd struct {
}

func (a *AttachCmd) Run(socket string) error {
	client := zandronum.NewClient(socket)
	if err := client.Open(); err != nil {
		return err
	}

	defer client.Close()

	go func() {
		scanner := bufio.NewScanner(os.Stdin)

		for scanner.Scan() {
			b, _ := json.Marshal(scanner.Text())

			client.Send() <- message.Message{
				BodyType: message.LINE,
				Body:     b,
			}
		}
	}()

	for msg := range client.Recv() {
		switch msg.BodyType {
		case message.LINE:
			var body string
			json.Unmarshal(msg.Body, &body)
			fmt.Fprint(os.Stdout, body)
		}
	}

	return nil
}
