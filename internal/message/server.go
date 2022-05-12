package message

import "time"

type Server struct {
	ID      string    `json:"id"`
	Name    string    `json:"name"`
	Status  string    `json:"status"`
	Started time.Time `json:"started"`
}
