package config

type Type string

var (
	Zandronum Type = "zandronum"
	Odamex         = "odamex"
)

type Server struct {
	ID    string   `json:"id"`
	Type  Type     `json:"type"`
	IWAD  string   `json:"iwad,omitempty"`
	PWADs []string `json:"pwads,omitempty"`
	Port  string   `json:"port,omitempty"`
	Args  []string `json:"args,omitempty"`
}
