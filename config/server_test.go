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
		Inner inner
	}

	x := ts{
		Inner: inner{
			Prop: "test",
		},
	}

	out, err := serverParams(x)

	assert.NoError(t, err)
	assert.Equal(t, []string{"-prop", "test"}, out)
}
