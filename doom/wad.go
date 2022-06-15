package doom

import (
	"errors"
	"os"
	"path/filepath"

	log "github.com/sirupsen/logrus"
)

var ErrWADNotFound = errors.New("wad not found")

func FindWAD(name string) (string, error) {
	dirs := []string{
		os.Getenv("DOOMWADDIR"),
	}

	//dirs = append(dirs, strings.Split(os.Getenv("DOOMWADPATH"), string(os.PathListSeparator))...)

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
