package command

import "github.com/alecthomas/kong"

func Parse(a any, options ...kong.Option) *kong.Context {
	return kong.Parse(
		a,
		kong.NamedMapper("pathenv", PathEnvDecoder()),
		kong.UsageOnError(),
	)
}
