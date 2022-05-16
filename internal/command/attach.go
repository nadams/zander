package command

import (
	"bufio"
	"encoding/json"
	"fmt"
	"os"
	"os/signal"
	"syscall"

	"gitlab.node-3.net/nadams/zander/internal/message"
	"gitlab.node-3.net/nadams/zander/zandronum"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`
}

func (a *AttachCmd) Run(ctx CmdCtx) error {
	client := zandronum.NewClient(ctx.Socket)
	if err := client.Open(); err != nil {
		return err
	}

	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

	defer client.Close()

	go func() {
		<-sigs

		client.Close()
	}()

	b, _ := json.Marshal(a.ID)

	client.Send() <- message.Message{BodyType: message.CMD_ATTACH, Body: b}
	client.StartPingPong()

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