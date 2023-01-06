package doom

import (
	"io"
	"strings"
	"sync"
)

var _ io.Writer = (*LogBuffer)(nil)

type LogBuffer struct {
	m sync.Mutex

	lines    []string
	maxLines int
}

func NewLogBuffer(maxLines int) *LogBuffer {
	if maxLines < 0 {
		panic("maxLines should be >= 0")
	}

	return &LogBuffer{
		maxLines: maxLines,
	}
}

func (l *LogBuffer) Write(b []byte) (int, error) {
	lines := strings.Split(string(b), "\n")

	l.m.Lock()
	defer l.m.Unlock()

	switch {
	case l.maxLines == 0:
		l.lines = append(l.lines, lines...)
	case len(lines) > l.maxLines:
		l.lines = lines[len(lines)-l.maxLines:]
	case len(lines)+len(l.lines) > l.maxLines:
		x := append(l.lines, lines...)

		l.lines = x[len(x)-l.maxLines:]
	default:
		l.lines = append(l.lines, lines...)
	}

	return len(b), nil
}

func (l *LogBuffer) Lines(n int) []string {
	if n <= 0 || n > len(l.lines) {
		return l.lines
	}

	return l.lines[len(l.lines)-n:]
}

func (l *LogBuffer) Bytes(n int) []byte {
	return l.asBytes(l.Lines(n))
}

func (*LogBuffer) asBytes(lines []string) []byte {
	return []byte(strings.Join(lines, "\n"))
}
