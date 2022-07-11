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
)

type ZandronumServer struct {
	*server
}

var portRegexp = regexp.MustCompile(`^IP address .+:(\d+)$`)

func NewZandronumServer(binary string, wadPath config.WADPaths, cfg config.Server) (*ZandronumServer, error) {
	s := &ZandronumServer{
		server: newServer(binary, wadPath, cfg),
	}

	s.server.logMappers = []logMapper{s.scanPort}
	s.server.preStart = s.newCmd

	if err := s.newCmd(); err != nil {
		return nil, err
	}

	return s, nil
}

func (s *ZandronumServer) Copy() (Server, error) {
	return NewZandronumServer(s.binary, s.wadPaths, s.cfg)
}

func (s *ZandronumServer) newCmd() error {
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
	case "ctf":
		params = append(params, "+ctf 1")
	case "1ctf":
		params = append(params, "+oneflagctf 1")
	case "skulltag":
		params = append(params, "+skulltag 1")
	case "duel":
		params = append(params, "+duel 1")
	case "teamgame":
		params = append(params, "+teamgame 1")
	case "domination":
		params = append(params, "+domination 1")
	case "survival":
		params = append(params, "+survival 1")
	case "invasion":
		params = append(params, "+invasion 1")
	case "cooperative":
		params = append(params, "+cooperative 1")
	case "dm":
		params = append(params, "+deathmatch 1")
	case "tdm":
		params = append(params, "+teamplay 1")
	case "terminator":
		params = append(params, "+terminator 1")
	case "possession":
		params = append(params, "+possession 1")
	case "tpossession":
		params = append(params, "+teampossession 1")
	case "lms":
		params = append(params, "+lastmanstanding 1")
	case "tlms":
		params = append(params, "+teamlms 1")
	default:
		s.cfg.Mode = "dm"
		params = append(params, "+deathmatch 1")
	}

	s.cmd = exec.Command(s.binary, params...)
	s.cmd.Env = append(s.cmd.Env, fmt.Sprintf("DOOMWADPATH=%s", s.wadPaths.String()))

	return nil
}

func (s *ZandronumServer) scanPort(b []byte) []byte {
	const portStr = "IP address "
	if lineStr := string(b); !s.foundAlternatePort && strings.HasPrefix(lineStr, portStr) {
		if matches := portRegexp.FindStringSubmatch(lineStr); len(matches) == 2 {
			if port, err := strconv.Atoi(matches[1]); err == nil {
				s.cfg.Port = port
				s.foundAlternatePort = true
				log.Infof("found alternate port for server %s, %d", s.cfg.ID, s.cfg.Port)
			}
		}
	}

	return b
}
