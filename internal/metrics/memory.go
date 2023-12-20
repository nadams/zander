package metrics

import "sync"

var _ Metrics = (*Memory)(nil)

type IntegerMetrics map[string]uint32

type Memory struct {
	playerCountsM sync.RWMutex
	playerCounts  IntegerMetrics
}

func NewMemory() *Memory {
	return &Memory{
		playerCounts: make(IntegerMetrics),
	}
}

func (d *Memory) IncPlayerCount(serverID, engine string) {
	d.playerCountsM.Lock()
	defer d.playerCountsM.Unlock()

	d.playerCounts[serverID] += 1
}

func (d *Memory) DecPlayerCount(serverID, engine string) {
	d.playerCountsM.Lock()
	defer d.playerCountsM.Unlock()

	d.playerCounts[serverID] -= 1
}

func (d *Memory) SetPlayerCount(serverID, engine string, count uint) {
	d.playerCountsM.Lock()
	defer d.playerCountsM.Unlock()

	d.playerCounts[serverID] = uint32(count)
}

func (d *Memory) PlayerCounts() map[string]uint32 {
	d.playerCountsM.RLock()
	defer d.playerCountsM.RUnlock()

	newV := make(map[string]uint32)

	for k, v := range d.playerCounts {
		newV[k] = v
	}

	return newV
}
