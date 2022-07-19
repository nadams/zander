package metrics

var _ Metrics = (*Noop)(nil)

type Noop struct{}

func (n *Noop) IncPlayerCount(serverID string)             {}
func (n *Noop) DecPlayerCount(serverID string)             {}
func (n *Noop) SetPlayerCount(serverID string, count uint) {}
