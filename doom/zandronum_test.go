package doom

import (
	"bytes"
	"io"
	"testing"

	"github.com/stretchr/testify/assert"
	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
)

func Test_ZandronumMetricsPlayerDisconnect(t *testing.T) {
	for name, test := range map[string]struct {
		text                string
		initialPlayerCount  uint
		expectedPlayerCount uint
	}{
		"disconnect not matched": {
			text:                "garbage\n",
			initialPlayerCount:  1,
			expectedPlayerCount: 1,
		},
		"single normal disconnect matched": {
			text:                "client player (127.0.0.1) disconnected.\n",
			initialPlayerCount:  2,
			expectedPlayerCount: 1,
		},
		"multiple normal disconnects matched": {
			text:                "client player (127.0.0.1) disconnected.\nclient player2 (127.0.0.1) disconnected.\nclient player3 (127.0.0.1) disconnected.\n",
			initialPlayerCount:  10,
			expectedPlayerCount: 7,
		},
		"single player time out matched": {
			text:                "player (127.0.0.1) timed out.",
			initialPlayerCount:  5,
			expectedPlayerCount: 4,
		},
		"multiple players time out matched": {
			text:                "player (127.0.0.1) timed out.\nplayer2 (127.0.0.1) timed out.\n",
			initialPlayerCount:  5,
			expectedPlayerCount: 3,
		},
		"disconnect and time out matched": {
			text:                "client player (127.0.0.1) disconnected.\ngarbage\nplayer2 (127.0.0.1) timed out.\n",
			initialPlayerCount:  5,
			expectedPlayerCount: 3,
		},
	} {
		test := test
		t.Run(name, func(t *testing.T) {
			serverID := "test server"
			met := metrics.NewMemory()
			text := bytes.NewBufferString(test.text)
			cfg := config.Server{ID: serverID, Engine: config.Zandronum}
			zand, err := NewZandronumServer("", config.WADPaths{}, cfg, met)
			assert.NoError(t, err)

			zand.stdout = io.NopCloser(text)

			met.SetPlayerCount(serverID, string(config.Zandronum), test.initialPlayerCount)

			zand.scanStdout()

			assert.Equal(t, test.expectedPlayerCount, met.PlayerCounts(string(config.Zandronum))[serverID])
		})
	}
}
