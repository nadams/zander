package doom

import (
	"bytes"
	"io"
	"testing"

	"github.com/stretchr/testify/assert"
	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
)

func Test_OdamexMetricsPlayerDisconnect(t *testing.T) {
	for name, test := range map[string]struct {
		text                string
		initialPlayerCount  uint32
		expectedPlayerCount uint32
	}{
		"disconnect not matched": {
			text:                "garbage\n",
			initialPlayerCount:  1,
			expectedPlayerCount: 1,
		},
		"single normal disconnect matched": {
			text:                "[00:00:00] player disconnected. (127.0.0.1)\n",
			initialPlayerCount:  2,
			expectedPlayerCount: 1,
		},
		"multiple normal disconnects matched": {
			text:                "[00:00:00] player disconnected. (127.0.0.1)\n[00:00:00] player2 disconnected. (127.0.0.1)\n[00:00:00] player3 disconnected. (127.0.0.1)\n",
			initialPlayerCount:  10,
			expectedPlayerCount: 7,
		},
	} {
		test := test
		t.Run(name, func(t *testing.T) {
			serverID := "test server"
			met := metrics.NewMemory()
			text := bytes.NewBufferString(test.text)
			cfg := config.Server{ID: serverID, Engine: config.Odamex}
			oda, err := NewOdamexServer("", config.WADPaths{}, cfg, met)
			assert.NoError(t, err)

			oda.stdout = io.NopCloser(text)

			met.SetPlayerCount(serverID, string(config.Odamex), uint(test.initialPlayerCount))

			oda.scanStdout()

			assert.Equal(t, test.expectedPlayerCount, met.PlayerCounts()[serverID])
		})
	}
}

func Test_OdamexMetricsMapChange(t *testing.T) {
	for name, test := range map[string]struct {
		text                string
		initialPlayerCount  uint32
		expectedPlayerCount uint32
	}{
		"map change with no players": {
			text:                "[00:00:00] text\n[00:00:00] --- MAP01: x factor ---\n",
			initialPlayerCount:  0,
			expectedPlayerCount: 0,
		},
		"player counts are reset after map change": {
			text:                "[00:00:00] text\n[00:00:00] --- MAP01: x factor ---\n",
			initialPlayerCount:  10,
			expectedPlayerCount: 0,
		},
		"player connects after map change": {
			text:                "text\n[00:00:00] --- MAP01: x factor ---\n[00:00:00] player has connected.\n",
			initialPlayerCount:  10,
			expectedPlayerCount: 1,
		},
	} {
		test := test
		t.Run(name, func(t *testing.T) {
			serverID := "test server"
			met := metrics.NewMemory()
			text := bytes.NewBufferString(test.text)
			cfg := config.Server{ID: serverID, Engine: config.Odamex}
			oda, err := NewOdamexServer("", config.WADPaths{}, cfg, met)
			assert.NoError(t, err)

			oda.stdout = io.NopCloser(text)
			met.SetPlayerCount(serverID, string(config.Odamex), uint(test.initialPlayerCount))
			oda.scanStdout()

			assert.Equal(t, test.expectedPlayerCount, met.PlayerCounts()[serverID])
		})
	}
}
