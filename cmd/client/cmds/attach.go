package cmds

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net"
	"os"

	"gitlab.node-3.net/nadams/zander/internal/message"
)

type AttachCmd struct {
}

func (a *AttachCmd) Run(socket string) error {
	conn, err := net.Dial("unix", socket)
	if err != nil {
		return fmt.Errorf("could not connect to zander: %w", err)
	}

	defer conn.Close()

	encoder := json.NewEncoder(conn)
	encoder.SetEscapeHTML(false)
	decoder := json.NewDecoder(conn)

	go func() {
		scanner := bufio.NewScanner(os.Stdin)
		for scanner.Scan() {
			b, _ := json.Marshal(scanner.Text())

			encoder.Encode(message.Message{
				BodyType: message.LINE,
				Body:     b,
			})
		}
	}()

	for {
		var msg message.Message

		if err := decoder.Decode(&msg); err != nil {
			if err == io.EOF {
				return nil
			}

			log.Printf("could not decode message: %v", err)
		}

		switch msg.BodyType {
		case message.PING:
			reply := message.Message{BodyType: message.PONG}

			if err := encoder.Encode(reply); err != nil {
				log.Println(err)
			}
		case message.LINE:
			var body string
			json.Unmarshal(msg.Body, &body)
			fmt.Fprint(os.Stdout, body)
		}
	}
}
