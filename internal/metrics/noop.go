package metrics

var _ Metrics = (*Noop)(nil)

type Noop struct{}
