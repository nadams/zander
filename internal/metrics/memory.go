package metrics

import "sync"

var _ Metrics = (*Memory)(nil)

type IntegerMetrics map[string]map[string]uint

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

	d.serverPlayerCounts(engine)[serverID] += 1

}

func (d *Memory) DecPlayerCount(serverID, engine string) {
	d.playerCountsM.Lock()
	defer d.playerCountsM.Unlock()

	d.serverPlayerCounts(engine)[serverID] -= 1
}

func (d *Memory) SetPlayerCount(serverID, engine string, count uint) {
	d.playerCountsM.Lock()
	defer d.playerCountsM.Unlock()

	d.serverPlayerCounts(engine)[serverID] = count
}

func (d *Memory) serverPlayerCounts(engine string) map[string]uint {
	v, found := d.playerCounts[engine]
	if !found {
		v = make(map[string]uint)
		d.playerCounts[engine] = v
	}

	return v
}

func (d *Memory) PlayerCounts(engine string) map[string]uint {
	d.playerCountsM.RLock()
	defer d.playerCountsM.RUnlock()

	newV := make(map[string]uint)
	v, found := d.playerCounts[engine]
	if !found {
		return newV
	}

	for k, v := range v {
		newV[k] = v
	}

	return newV
}
