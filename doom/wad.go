package doom

import (
	"errors"
	"os"
	"path/filepath"
	"strings"

	log "github.com/sirupsen/logrus"
)

var ErrWADNotFound = errors.New("wad not found")

// FindWADFromEnv tries to find the given WAD file in the given list of dirs, with
// the environment variables DOOMWADDIR and DOOMWADPATH taken into account.
func FindWADFromEnv(name string, dirs ...string) (string, error) {
	if p := os.Getenv("DOOMWADDIR"); len(p) > 0 {
		dirs = append(dirs, p)
	}

	dirs = append(dirs, strings.Split(os.Getenv("DOOMWADPATH"), string(os.PathListSeparator))...)

	return FindWAD(name, dirs...)
}

// FindWAD tries to find the given WAD file in the given list of dirs.
func FindWAD(name string, dirs ...string) (string, error) {
	for _, dir := range dirs {
		path, err := findWADInDir(name, dir)
		if err != nil {
			log.Infof("%s not found, searched %s", name, dir)
			continue
		}

		log.Infof("found %s in %s", name, dir)

		return path, nil
	}

	return "", ErrWADNotFound
}

func findWADInDir(name, dir string) (string, error) {
	entries, err := os.ReadDir(dir)
	if err != nil {
		return "", err
	}

	for _, entry := range entries {
		if entry.Name() == name {
			return filepath.Join(dir, name), nil
		}
	}

	return "", ErrWADNotFound
}
