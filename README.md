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

Using a pre-built package for your platform will install `zander` at a system level. If you don't want to pollute your system, you can download a precompiled binary archive and install it to your `$HOME` directory instead. This archive comes with example configuration that you can copy and modify for your use case.

### Arch Linux
Install from the [AUR](https://aur.archlinux.org/packages/zander-bin) (zander-bin) or install a pre-built pacman package for your architecture from the [releases](https://gitlab.node-3.net/zander/zander/-/releases) page.

### Debian/Ubuntu
Download the current `.deb` release for your architecture and install using `dpkg`.

https://gitlab.node-3.net/zander/zander/-/releases

### RHEL/Fedora
Download the current `.rpm` release for your architecture and install using `rpm`.

https://gitlab.node-3.net/zander/zander/-/releases

### Pre-compiled Binaries
You can find pre-compiled binaries on the [releases](https://gitlab.node-3.net/zander/zander/-/releases) page.

Simply extract the archive and place the `zander` binary in your `$PATH`. There are also other files which are example
configurations. Refer to the [configuration](#configuration) section for more information.

### Configuration
Configuration for zander is stored in `$XDG_CONFIG_HOME/zander`, which usually defaults to `$HOME/.config`.

1. Copy `zander.toml` to `$XDG_CONFIG_HOME/zander/zander.toml`. You should go through the example file and configure it for your system.
2. Create a `servers` directory in `$XDG_CONFIG_HOME/zander` and copy example `doom2.toml` file there.

### Install as a Systemd User Service
```sh
mkdir -p $HOME/.config/systemd/user
cp config/zander.service $HOME/.config/systemd/user
# Edit the ExecStart field in zander.service file to point to where you installed `zander`
systemctl --user daemon-reload
systemctl --user enable --now zander
sudo loginctl enable-linger
```

The Systemd service file can be found in this repository (`.pkg/zander.service`) or in a pre-compiled binary archive found on the releases page.

## Usage

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)

