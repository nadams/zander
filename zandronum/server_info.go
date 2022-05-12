package zandronum

import "time"

type ServerInfo struct {
	ID      string
	Name    string
	Status  string
	Started time.Time
}
