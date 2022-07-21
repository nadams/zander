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

func (m *Multi) IncPlayerCount(serverID string) {
	for _, x := range m.m {
		x.IncPlayerCount(serverID)
	}
}

func (m *Multi) DecPlayerCount(serverID string) {
	for _, x := range m.m {
		x.DecPlayerCount(serverID)
	}
}

func (m *Multi) SetPlayerCount(serverID string, count uint) {
	for _, x := range m.m {
		x.SetPlayerCount(serverID, count)
	}
}
