package history

type CmdPtr struct {
	history *CmdHistory

	idx int
}

func (c *CmdPtr) Next() string {
	val, valid := c.history.Next(c.idx)
	if valid {
		c.idx++
	}

	return val
}

func (c *CmdPtr) Prev() string {
	if c.idx < 0 {
		c.idx = len(c.history.cmds)
	}

	val, valid := c.history.Prev(c.idx)
	if valid {
		c.idx--
	}

	return val
}

func (c *CmdPtr) Reset() {
	c.idx = -1
}
