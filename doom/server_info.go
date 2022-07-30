package doom

import (
	"time"

	"gitlab.node-3.net/zander/zander/config"
)

type ServerInfo struct {
	ID      string
	Name    string
	Engine  config.Engine
	Mode    string
	Status  string
	Port    int
	IWAD    string
	PWADs   []string
	Started time.Time
	Stopped time.Time
}
