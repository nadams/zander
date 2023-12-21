package doom

import (
	"fmt"
	"io"
	"os"
	"os/exec"
	"path/filepath"
	"regexp"
	"strconv"
	"strings"

	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
	"gitlab.node-3.net/zander/zander/internal/metrics"
)

type OdamexServer struct {
	*server
}

func NewOdamexServer(binary string, wadPaths config.WADPaths, cfg config.Server, met metrics.Metrics) (*OdamexServer, error) {
	s := &OdamexServer{
		server: newServer(binary, wadPaths, cfg, met),
	}

	s.server.logMappers = []logMapper{
		s.scanPort,
		s.scanPlayerConnect,
		s.scanPlayerDisconnect,
	}

	s.server.preStart = s.newCmd

	if err := s.newCmd(); err != nil {
		return nil, err
	}

	return s, nil
}

func (s *OdamexServer) Copy() (Server, error) {
	return NewOdamexServer(s.binary, s.wadPaths, s.cfg, s.metrics)
}

func (s *OdamexServer) newCmd() error {
	params, err := s.cfg.Parameters(s.wadPaths.Expanded())
	if err != nil {
		return fmt.Errorf("could not get config parameters: %w", err)
	}

	cvars, err := s.cfg.CVARs()
	if err != nil {
		return fmt.Errorf("could not get config cvars: %w", err)
	}

	f, err := os.OpenFile(filepath.Join(os.TempDir(), fmt.Sprintf("%s.cfg", s.cfg.ID)), os.O_TRUNC|os.O_CREATE|os.O_RDWR, 0600)
	if err != nil {
		return fmt.Errorf("could not create temp config file: %w", err)
	}

	defer f.Close()

	if _, err := io.Copy(f, strings.NewReader(cvars)); err != nil {
		return fmt.Errorf("could not write to temp config file: %w", err)
	}

	params = append(params,
		"+exec", f.Name(),
		"+logfile", os.DevNull,
	)

	switch strings.ToLower(s.cfg.Mode) {
	case "dm", "deathmatch":
		params = append(params, "+sv_gametype", "1", "+sv_itemsrespawn", "0", "+sv_weaponstay", "1")
	case "altdeath":
		params = append(params, "+sv_gametype", "1", "+sv_itemsrespawn", "1", "+sv_weaponstay", "0")
	case "nsdm":
		params = append(params, "+sv_gametype", "1", "+sv_itemsrespawn", "1", "+sv_weaponstay", "1")
	case "cooperative":
		params = append(params, "+sv_gametype", "0")
	case "tdm":
		params = append(params, "+sv_gametype", "2")
	case "ctf":
		params = append(params, "+sv_gametype", "3")
	default:
		s.cfg.Mode = "dm"
		params = append(params, "+sv_gametype", "1", "+sv_itemsrespawn", "0", "+sv_weaponstay", "1")
	}

	s.cmd = exec.Command(s.binary, params...)

	return nil
}

const (
	odaTimestamp  = "[00:00:00] "
	odaPortStr    = "Bound to local port "
	odaChatPrefix = "<CHAT> "
)

func (s *OdamexServer) isChat(b []byte) bool {
	if str := string(b); len(str) >= len(odaTimestamp)+len(odaChatPrefix) {
		return strings.HasPrefix(str[len(odaTimestamp):], odaChatPrefix)
	}

	return false
}

func (s *OdamexServer) scanPort(b []byte) []byte {
	if !s.foundAlternatePort && len(b) > len(odaPortStr) {
		if lineStr := string(b)[len(odaTimestamp):]; strings.HasPrefix(lineStr, odaPortStr) {
			portStr := lineStr[len(odaPortStr):]

			if port, err := strconv.Atoi(portStr); err == nil {
				s.cfg.Port = port
				s.foundAlternatePort = true
				log.Infof("found alternate port for server %s, %d", s.cfg.ID, s.cfg.Port)
			}
		}
	}

	return b
}

var (
	odaClientConnectRegexp    = regexp.MustCompile(`^\[\d{2}:\d{2}:\d{2}\] .+ has connected\.$`)
	odaClientDisconnectRegexp = regexp.MustCompile(`^\[\d{2}:\d{2}:\d{2}\] .+ disconnected\. \(.+\)$`)
)

func (s *OdamexServer) scanPlayerConnect(b []byte) []byte {
	if !s.isChat(b) && odaClientConnectRegexp.Match(b) {
		s.metrics.IncPlayerCount(s.cfg.ID, string(s.cfg.Engine))
	}

	return b
}

func (s *OdamexServer) scanPlayerDisconnect(b []byte) []byte {
	if !s.isChat(b) && odaClientDisconnectRegexp.Match(b) {
		s.metrics.DecPlayerCount(s.cfg.ID, string(s.cfg.Engine))
	}

	return b
}
