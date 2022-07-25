package metrics

var _ Metrics = (*Noop)(nil)

type Noop struct{}

func (n *Noop) IncPlayerCount(serverID, engine string)             {}
func (n *Noop) DecPlayerCount(serverID, engine string)             {}
func (n *Noop) SetPlayerCount(serverID, engine string, count uint) {}
