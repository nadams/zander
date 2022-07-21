package metrics

type Metrics interface {
	IncPlayerCount(string)
	DecPlayerCount(string)
	SetPlayerCount(string, uint)
}
