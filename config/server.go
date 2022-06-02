package config

import (
	"fmt"
	"os"
	"path/filepath"
	"reflect"
	"regexp"
	"strconv"
	"strings"

	"github.com/fatih/structtag"
	"github.com/pelletier/go-toml/v2"
)

type RestartPolicy string

const (
	No            RestartPolicy = "no"
	OnFailure                   = "on-failure"
	UnlessStopped               = "unless-stopped"
)

type Server struct {
	ID            string        `toml:"id,omitempty" zander:"-"`
	Disabled      bool          `toml:"disabled,omitempty" zander:"-"`
	RestartPolicy RestartPolicy `toml:"restart_policy,omitempty" zander:"-"`
	Mode          string        `toml:"mode,omitempty" zander:"mode"`
	Email         string        `toml:"email,omitempty" zander:"sv_hostemail,cvar"`
	Port          int           `toml:"port,omitempty" zander:"port"`
	Hostname      string        `toml:"hostname,omitempty" zander:"sv_hostname,cvar"`
	Website       string        `toml:"website,omitempty" zander:"sv_website,cvar"`
	IWAD          string        `toml:"iwad,omitempty" zander:"iwad"`
	PWADs         []string      `toml:"pwads,omitempty" zander:"file"`
	Skill         int           `toml:"skill,omitempty" zander:"skill"`
	MOTD          string        `toml:"motd,multiline,omitempty" zander:"sv_motd,cvar"`
	Maplist       []string      `toml:"maplist,omitempty" zander:"addmap,cvar"`
	RCONPassword  string        `toml:"rcon_password,omitempty" zander:"sv_rconpassword,cvar"`
	RawParams     string        `toml:"raw_params,multiline,omitempty" zander:",rawcvar"`
}

func LoadServer(path string) (Server, error) {
	f, err := os.OpenFile(path, os.O_RDONLY, 0)
	if err != nil {
		return Server{}, nil
	}

	defer f.Close()

	name := filepath.Base(path)
	name = name[:len(name)-len(filepath.Ext(name))]

	s := Server{
		ID: name,
	}

	if err := toml.NewDecoder(f).Decode(&s); err != nil {
		return Server{}, err
	}

	if s.RestartPolicy == "" {
		s.RestartPolicy = OnFailure
	}

	return s, nil
}

var rawcvarRegexp = regexp.MustCompile(`(\w+)\s+(.+)`)

func (s Server) Parameters() ([]string, error) {
	var out []string

	t := reflect.TypeOf(s)
	v := reflect.ValueOf(s)
	for i := 0; i < t.NumField(); i++ {
		tag := t.Field(i).Tag

		tags, err := structtag.Parse(string(tag))
		if err != nil {
			return nil, err
		}

		zanderTag, err := tags.Get("zander")
		if err != nil {
			return nil, err
		}

		if zanderTag.Name == "-" || zanderTag.HasOption("cvar") || zanderTag.HasOption("rawcvar") {
			continue
		}

		option := fmt.Sprintf("-%s", zanderTag.Name)

		switch v := v.Field(i).Interface().(type) {
		case []string:
			for _, x := range v {
				if x != "" {
					out = append(out, option, x)
				}
			}
		case string:
			if v != "" {
				out = append(out, option, v)
			}
		case int:
			if v != 0 {
				out = append(out, option, strconv.Itoa(v))
			}
		}
	}

	return out, nil
}

func (s Server) CVARs() (string, error) {
	var out strings.Builder

	t := reflect.TypeOf(s)
	v := reflect.ValueOf(s)
	for i := 0; i < t.NumField(); i++ {
		tag := t.Field(i).Tag

		tags, err := structtag.Parse(string(tag))
		if err != nil {
			return "", err
		}

		zanderTag, err := tags.Get("zander")
		if err != nil {
			return "", err
		}

		switch {
		case zanderTag.Name == "-":
			continue
		case zanderTag.HasOption("cvar"):
			val := v.Field(i).Interface()
			switch z := val.(type) {
			case string:
				out.WriteString(zanderTag.Name)
				out.WriteString(" ")
				out.WriteString("\"")
				out.WriteString(z)
				out.WriteString("\"")
				out.WriteString("\n")
			case []string:
				for _, a := range z {
					out.WriteString(zanderTag.Name)
					out.WriteString(" ")
					out.WriteString("\"")
					out.WriteString(a)
					out.WriteString("\"")
					out.WriteString("\n")
				}
			default:
				out.WriteString(zanderTag.Name)
				out.WriteString(" ")
				out.WriteString(fmt.Sprintf("%v", z))
				out.WriteString("\n")
			}
		case zanderTag.HasOption("rawcvar"):
			val := v.Field(i).Interface()
			out.WriteString(fmt.Sprintf("%v", val))
			out.WriteString("\n")
		default:
			continue
		}
	}

	return out.String(), nil
}
