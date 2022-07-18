package metrics

var _ Metrics = (*Prometheus)(nil)

type Prometheus struct {
}
