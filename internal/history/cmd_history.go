package history

import "sync"

type CmdHistory struct {
	m sync.RWMutex

	max  int
	cmds []string
}

func NewCmdHistory(max int) *CmdHistory {
	return &CmdHistory{
		max:  max,
		cmds: []string{},
	}
}

func (c *CmdHistory) Append(cmd string) error {
	c.m.Lock()
	defer c.m.Unlock()

	c.cmds = append(c.cmds, cmd)

	// TODO: write to disk?

	return nil
}

func (c *CmdHistory) Next(from int) (string, bool) {
	c.m.RLock()
	defer c.m.RUnlock()

	if len(c.cmds) > 0 {
		if from >= len(c.cmds) || from+1 >= len(c.cmds) {
			return "", false
		}

		return c.cmds[from+1], true
	}

	return "", false
}

func (c *CmdHistory) Prev(from int) (string, bool) {
	c.m.RLock()
	defer c.m.RUnlock()

	if len(c.cmds) > 0 {
		if from-1 == 0 {
			return c.cmds[0], true
		}

		if from-1 < 0 {
			return c.cmds[0], false
		}

		return c.cmds[from-1], true
	}

	return "", false
}

func (c *CmdHistory) Get(idx int) (string, bool) {
	c.m.RLock()
	defer c.m.RUnlock()

	if idx < 0 || idx >= len(c.cmds) {
		return "", false
	}

	return c.cmds[idx], true
}

func (c *CmdHistory) Len() int {
	c.m.RLock()
	defer c.m.RUnlock()

	return len(c.cmds)
}

func (c *CmdHistory) All() []string {
	c.m.RLock()
	defer c.m.RUnlock()

	out := make([]string, 0)
	for i := range c.cmds {
		out[i] = c.cmds[i]
	}

	return out
}

func (c *CmdHistory) Ptr() *CmdPtr {
	return &CmdPtr{
		history: c,
		idx:     -1,
	}
}
