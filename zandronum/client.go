package zandronum

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net"
	"sync"

	"gitlab.node-3.net/nadams/zander/internal/message"
)

type Client struct {
	socket  string
	conn    net.Conn
	encoder *json.Encoder
	decoder *json.Decoder
	send    chan message.Message
	recv    chan message.Message
	wg      sync.WaitGroup
	pp      bool
}

func NewClient(socket string) *Client {
	return &Client{
		socket: socket,
		send:   make(chan message.Message),
		recv:   make(chan message.Message),
	}
}

func (c *Client) Close() error {
	c.wg.Wait()

	if c.conn != nil {
		c.conn.Close()
	}

	return nil
}

func (c *Client) Open() error {
	if c.conn == nil {
		conn, err := net.Dial("unix", c.socket)
		if err != nil {
			return fmt.Errorf("could not connect to zander: %w", err)
		}

		c.conn = conn
		c.encoder = json.NewEncoder(c.conn)
		c.encoder.SetEscapeHTML(false)
		c.decoder = json.NewDecoder(c.conn)

		c.wg.Add(1)
		go func() {
			defer func() {
				close(c.recv)
				c.wg.Done()
			}()

			for msg := range c.send {
				if err := c.encoder.Encode(msg); err != nil {
					log.Println(err)
				}
			}
		}()

		c.wg.Add(1)
		go func() {
			defer func() {
				close(c.send)
				c.wg.Done()
			}()

			for {
				var msg message.Message

				if err := c.decoder.Decode(&msg); err != nil {
					if err == io.EOF {
						return
					} else if _, ok := err.(*net.OpError); ok {
						return
					}

					log.Printf("could not decode message: %v", err)
					return
				}

				switch msg.BodyType {
				case message.PING:
					if c.pp {
						c.send <- message.Message{BodyType: message.PONG}
					}
				case message.PONG:
				default:
					c.recv <- msg
				}
			}
		}()
	}

	return nil
}

func (c *Client) StartPingPong() {
	c.pp = true
}

func (c *Client) Send() chan<- message.Message {
	return c.send
}

func (c *Client) Recv() <-chan message.Message {
	return c.recv
}
