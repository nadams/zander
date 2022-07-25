package metrics

type Metrics interface {
	IncPlayerCount(string, string)
	DecPlayerCount(string, string)
	SetPlayerCount(string, string, uint)
}
