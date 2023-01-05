# Zander

Zander is a Doom server manager.

## Features
- Automatically restart crashed servers
- Collect metrics (prometheus and statsd are supported)
- Attach to a running server
- Tail server logs
- Shell completion

## Installation

### Arch Linux
https://aur.archlinux.org/packages/zander-bin

### Other
Download current release for your platform and architecture.

https://gitlab.node-3.net/zander/zander/-/releases

Add `zander` to your `PATH`.

### Configuration
Configuration for zander is stored in `$XDG_CONFIG_HOME/zander`, which usually defaults to `$HOME/.config`.

1. Copy `zander.toml` to `$XDG_CONFIG_HOME/zander/zander.toml`

## Usage

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
