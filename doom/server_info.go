package doom

import "time"

type ServerInfo struct {
	ID      string
	Name    string
	Mode    string
	Status  string
	Port    int
	IWAD    string
	PWADs   []string
	Started time.Time
	Stopped time.Time
}
