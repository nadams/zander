# Zander

Zander is a Doom server manager.

## Features
- Automatically restart crashed servers
- Collect metrics (prometheus and statsd are supported)
- Attach to a running server
- View and tail server logs
- Shell completion
- Hot reloading of server configurations

## Installation

### Arch Linux
https://aur.archlinux.org/packages/zander-bin

You can also grab a pre-built package from the [releases](https://gitlab.node-3.net/zander/zander/-/releases) page.

### Other
Download current release for your platform and architecture.

https://gitlab.node-3.net/zander/zander/-/releases

Add `zander` to your `PATH`.

### Run as a Systemd User Service

```sh
mkdir -p $HOME/.config/{systemd/user,zander/servers}
cp .pkg/zander.service $HOME/.config/systemd/user
cp .pkg/zander.toml $HOME/.config/zander
systemctl --user daemon-reload
systemctl --user enable zander
sudo loginctl enable-linger
```

### Configuration
Configuration for zander is stored in `$XDG_CONFIG_HOME/zander`, which usually defaults to `$HOME/.config`.

1. Copy `zander.toml` to `$XDG_CONFIG_HOME/zander/zander.toml`

## Usage

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
