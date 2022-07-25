package metrics

var _ Metrics = (*Multi)(nil)

type Multi struct {
	m []Metrics
}

func NewMulti(m ...Metrics) *Multi {
	return &Multi{
		m: m,
	}
}

func (m *Multi) IncPlayerCount(serverID, engine string) {
	for _, x := range m.m {
		x.IncPlayerCount(serverID, engine)
	}
}

func (m *Multi) DecPlayerCount(serverID, engine string) {
	for _, x := range m.m {
		x.DecPlayerCount(serverID, engine)
	}
}

func (m *Multi) SetPlayerCount(serverID, engine string, count uint) {
	for _, x := range m.m {
		x.SetPlayerCount(serverID, engine, count)
	}
}
