package command

import (
	"os"
	"reflect"

	"github.com/alecthomas/kong"
	"github.com/pkg/errors"
)

func PathEnvDecoder() kong.MapperFunc {
	return func(ctx *kong.DecodeContext, target reflect.Value) error {
		t, err := ctx.Scan.PopValue("pathenv")
		if err != nil {
			return err
		}

		switch v := t.Value.(type) {
		case string:
			target.SetString(os.ExpandEnv(v))
		default:
			return errors.Errorf("expected a string but got %q (%T)", t, t.Value)
		}

		return nil
	}
}
