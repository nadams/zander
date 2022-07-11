package doom

import (
	"fmt"
	"io"
	"os"
	"os/exec"
	"path/filepath"
	"strconv"
	"strings"

	log "github.com/sirupsen/logrus"

	"gitlab.node-3.net/zander/zander/config"
)

type OdamexServer struct {
	*server
}

func NewOdamexServer(binary string, wadPaths config.WADPaths, cfg config.Server) (*OdamexServer, error) {
	s := &OdamexServer{
		server: newServer(binary, wadPaths, cfg),
	}

	s.server.logMappers = []logMapper{s.scanPort}
	s.server.preStart = s.newCmd

	if err := s.newCmd(); err != nil {
		return nil, err
	}

	return s, nil
}

func (s *OdamexServer) Copy() (Server, error) {
	return NewOdamexServer(s.binary, s.wadPaths, s.cfg)
}

func (s *OdamexServer) newCmd() error {
	if _, err := FindWAD(s.cfg.IWAD, s.wadPaths.Expanded()...); err != nil {
		return fmt.Errorf("could not find IWAD %s", s.cfg.IWAD)
	}

	for _, pwad := range s.cfg.PWADs {
		if _, err := FindWAD(pwad, s.wadPaths.Expanded()...); err != nil {
			return fmt.Errorf("could not find PWAD %s", pwad)
		}
	}

	params, err := s.cfg.Parameters()
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

	params = append(params, "+exec", f.Name())

	switch strings.ToLower(s.cfg.Mode) {
	case "dm", "deathmatch":
		params = append(params, "+sv_gametype 1", "-deathmatch")
	case "altdeath":
		params = append(params, "+sv_gametype 1", "-altdeath")
	case "nsdm":
		params = append(params, "+sv_gametype 1", "\"Newschool\"")
	case "cooperative":
		params = append(params, "+sv_gametype 0")
	case "tdm":
		params = append(params, "+sv_gametype 2")
	case "ctf":
		params = append(params, "+sv_gametype 3")
	default:
		s.cfg.Mode = "dm"
		params = append(params, "+sv_gametype 1", "-deathmatch")
	}

	s.cmd = exec.Command(s.binary, params...)
	s.cmd.Env = append(s.cmd.Env, fmt.Sprintf("DOOMWADPATH=%s", s.wadPaths.String()))

	return nil
}

const (
	odaTimestamp = "[00:00:00] "
	odaPortStr   = "Bound to local port "
)

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
