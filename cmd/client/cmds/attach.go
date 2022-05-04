package cmds

import (
	"bufio"
	"fmt"
	"io"
	"net"
	"os"
)

type AttachCmd struct {
}

func (a *AttachCmd) Run(socket string) error {
	conn, err := net.Dial("unix", socket)
	if err != nil {
		return fmt.Errorf("could not connect to zander: %w", err)
	}

	defer conn.Close()

	go func() {
		scanner := bufio.NewScanner(os.Stdin)
		for scanner.Scan() {
			conn.Write(scanner.Bytes())
			conn.Write([]byte("\n"))
		}
	}()

	io.Copy(os.Stdout, conn)

	return nil
}
