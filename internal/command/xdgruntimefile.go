package command

import (
	"reflect"

	"github.com/adrg/xdg"
	"github.com/alecthomas/kong"
	"github.com/pkg/errors"
)

func XDGRuntimeFile() kong.MapperFunc {
	return func(ctx *kong.DecodeContext, target reflect.Value) error {
		t, err := ctx.Scan.PopValue("xdgruntimefile")
		if err != nil {
			return err
		}

		switch v := t.Value.(type) {
		case string:
			x, err := xdg.RuntimeFile(v)
			if err != nil {
				return err
			}

			target.SetString(x)
		default:
			return errors.Errorf("expected a string but got %q (%T)", t, t.Value)
		}

		return nil
	}
}
