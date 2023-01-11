package config

import (
	"os"
	"path/filepath"
	"strings"
	"testing"

	"github.com/davecgh/go-spew/spew"
	"github.com/stretchr/testify/assert"
)

const wadDir = "testdata"

var (
	doomwaddir  = filepath.Join(wadDir, "env")
	doomwadpath = strings.Join([]string{
		filepath.Join(wadDir, "env", "a"),
		filepath.Join(wadDir, "env", "b"),
		filepath.Join(wadDir, "env", "c"),
	}, string(os.PathListSeparator))
)

func Test_FindWAD_FoundInSingleDirectory(t *testing.T) {
	x, err := FindWAD("test.wad", wadDir)

	spew.Dump(x)
	assert.NoError(t, err)
	assert.True(t, strings.HasSuffix(x, "test.wad"))
}

func Test_FindWAD_DoesNotSearchRecursively(t *testing.T) {
	x, err := FindWAD("r.wad", wadDir)

	assert.EqualError(t, err, ErrWADNotFound.Error())
	assert.Empty(t, x)
}

func Test_FindWAD_NotFound(t *testing.T) {
	x, err := FindWAD("notfound.wad", wadDir)

	assert.EqualError(t, err, ErrWADNotFound.Error())
	assert.Empty(t, x)
}

func Test_FindWADFromEnv_DOOMWADDIR_FoundInSingleDirectory(t *testing.T) {
	os.Setenv("DOOMWADDIR", doomwaddir)
	defer os.Unsetenv("DOOMWADDIR")

	x, err := FindWADFromEnv("test2.wad", wadDir)

	assert.NoError(t, err)
	assert.True(t, strings.HasSuffix(x, "test2.wad"))
}

func Test_FindWADFromEnv_DOOMWADPATH_FoundInSingleDirectory(t *testing.T) {
	os.Setenv("DOOMWADPATH", doomwadpath)
	defer os.Unsetenv("DOOMWADDIR")

	x, err := FindWADFromEnv("test3.wad", wadDir)

	assert.NoError(t, err)
	assert.Equal(t, filepath.Join(wadDir, "env", "a", "test3.wad"), x)
}

func Test_FindWADFromEnv_DOOMWADPATH_FoundInMultipleDirectories_FirstOneUsed(t *testing.T) {
	os.Setenv("DOOMWADPATH", doomwadpath)
	defer os.Unsetenv("DOOMWADDIR")

	x, err := FindWADFromEnv("test5.wad", wadDir)

	assert.NoError(t, err)
	assert.Equal(t, filepath.Join(wadDir, "env", "b", "test5.wad"), x)
}
