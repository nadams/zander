package message

import "encoding/json"

type BodyType int

const (
	PING BodyType = iota + 1
	PONG
	LINE
	SERVER_LIST
	CMD_ATTACH
	CMD_LIST_SERVERS
	DISCONNECT
)

type Message struct {
	BodyType BodyType        `json:"bodyType"`
	Body     json.RawMessage `json:"body,omitempty"`
}
