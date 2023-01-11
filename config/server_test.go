package config

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func Test_serverParams(t *testing.T) {
	type inner struct {
		Prop string `zander:"prop"`
	}

	type ts struct {
		Str      string   `zander:"str"`
		Skipped  string   `zander:"-"`
		Int      int      `zander:"int"`
		StrSlice []string `zander:"strslice"`
		Inner    inner
	}

	x := ts{
		Str:      "str",
		Skipped:  "skipped",
		Int:      10,
		StrSlice: []string{"a", "b", "c"},
		Inner: inner{
			Prop: "test",
		},
	}

	out, err := serverParams(x, []string{})

	assert.NoError(t, err)
	assert.Equal(t, []string{
		"-str", "str",
		"-int", "10",
		"-strslice", "a",
		"-strslice", "b",
		"-strslice", "c",
		"-prop", "test",
	}, out)
}
