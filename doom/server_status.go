package doom

type ServerStatus string

const (
	Running    ServerStatus = "running"
	Stopped                 = "stopped"
	Errored                 = "errored"
	NotStarted              = "not started"
)
