package doom

import (
	"strings"
	"testing"

	"github.com/stretchr/testify/assert"
)

func Test_LogBuffer_MaxLines(t *testing.T) {
	type tst struct {
		maxLines      int
		lines         []string
		content       string
		expectedLines []string
	}

	for name, test := range map[string]tst{
		"appended line amount is less than max lines": {
			maxLines:      10,
			content:       "test",
			lines:         []string{"1"},
			expectedLines: []string{"1", "test"},
		},
		"appended line amount plus existing lines is more than max lines": {
			maxLines:      4,
			content:       "5",
			lines:         []string{"1", "2", "3", "4"},
			expectedLines: []string{"2", "3", "4", "5"},
		},
		"appended line amount is equal to max lines": {
			maxLines:      4,
			content:       "5\n6\n7\n8",
			lines:         []string{"1", "2", "3", "4"},
			expectedLines: []string{"5", "6", "7", "8"},
		},
		"appended line amount is more than max lines": {
			maxLines:      4,
			content:       "5\n6\n7\n8\n9",
			lines:         []string{"1", "2", "3", "4"},
			expectedLines: []string{"6", "7", "8", "9"},
		},
		"maxLines is zero": {
			maxLines:      0,
			content:       "5\n6\n7\n8\n9",
			lines:         []string{"1", "2", "3", "4"},
			expectedLines: []string{"1", "2", "3", "4", "5", "6", "7", "8", "9"},
		},
	} {
		test := test

		t.Run(name, func(t *testing.T) {
			logger := NewLogBuffer(test.maxLines)
			logger.lines = test.lines
			logger.Write([]byte(test.content))

			assert.Equal(t, test.expectedLines, strings.Split(string(logger.Bytes(0)), "\n"))
		})
	}
}

func Test_LogBuffer_Constructor(t *testing.T) {
	assert.Panics(t, func() {
		NewLogBuffer(-1)
	})
}
