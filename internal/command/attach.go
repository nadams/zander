package command

import (
	"context"
	"fmt"
	"io"

	"github.com/jroimartin/gocui"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		g, err := gocui.NewGui(gocui.OutputNormal)
		if err != nil {
			return err
		}

		defer g.Close()

		ctx, cancel := context.WithCancel(context.Background())

		cmds := make(chan string)

		quit := func(g *gocui.Gui, v *gocui.View) error {
			log.Println("got quit")
			cancel()

			return gocui.ErrQuit
		}

		sendcmd := func(g *gocui.Gui, v *gocui.View) error {
			v, err := g.View("cmd")
			if err != nil {
				return err
			}

			cmds <- v.Buffer()

			v.Clear()
			v.SetCursor(0, 0)

			return nil
		}

		layout := func(g *gocui.Gui) error {
			maxX, maxY := g.Size()
			if v, err := g.SetView("cmd", 1, maxY-2, maxX, maxY); err != nil {
				if err != gocui.ErrUnknownView {
					return err
				}
				v.Frame = false
				v.Editable = true
				v.Clear()
			}

			v, err := g.SetView("out", -1, -1, maxX, maxY-2)
			if err != nil {
				if err != gocui.ErrUnknownView {
					return err
				}
				v.Autoscroll = true
				v.Wrap = true
				v.Editable = false
				v.Frame = true
			}

			g.SetCurrentView("cmd")

			return nil
		}

		g.SetManagerFunc(layout)
		g.InputEsc = true
		g.Cursor = true

		if err := g.SetKeybinding("", gocui.KeyCtrlC, gocui.ModNone, quit); err != nil {
			return err
		}

		if err := g.SetKeybinding("", gocui.KeyEnter, gocui.ModNone, sendcmd); err != nil {
			return err
		}

		stream, err := client.Attach(ctx)
		if err != nil {
			return err
		}

		if err := stream.Send(&zproto.AttachIn{Id: a.ID}); err != nil {
			return err
		}

		go func() {
			for cmd := range cmds {
				if err := stream.Send(&zproto.AttachIn{
					Id:      a.ID,
					Content: []byte(cmd),
				}); err != nil {
					if err == io.EOF {
						return
					}

					log.Error(err)
					return
				}
			}
		}()

		go g.MainLoop()

		for {
			in, err := stream.Recv()
			if err != nil {
				if err == io.EOF {
					break
				}

				if _, ok := status.FromError(err); ok {
					break
				}

				log.Errorf("unknown error from server: %v", err)
			}

			g.Update(func(g *gocui.Gui) error {
				v, err := g.View("out")
				if err != nil {
					return err
				}

				fmt.Fprint(v, string(in.Content))

				return nil
			})
		}

		return nil
	})
}
