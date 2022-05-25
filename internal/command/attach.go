package command

import (
	"bufio"
	"context"
	"fmt"
	"io"
	"os"
	"os/signal"
	"syscall"

	"github.com/gdamore/tcell/v2"
	"github.com/rivo/tview"
	log "github.com/sirupsen/logrus"
	"google.golang.org/grpc/status"

	"gitlab.node-3.net/nadams/zander/zproto"
)

type AttachCmd struct {
	ID string `arg:"" required:"true"`

	Output string `flag:"" enum:"raw,default" default:"default" help:"Output mode. valid values: (valid values: ${enum})"`
}

func (a *AttachCmd) Run(cmdCtx CmdCtx) error {
	return WithConn(cmdCtx.Socket, func(client zproto.ZanderClient) error {
		ctx, cancel := context.WithCancel(context.Background())
		defer cancel()

		in := make(chan string)
		out := make(chan string)
		sigs := make(chan os.Signal, 1)

		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

		go func() {
			<-sigs

			close(in)
			close(out)
		}()

		stream, err := client.Attach(ctx)
		if err != nil {
			return err
		}

		if err := stream.Send(&zproto.AttachIn{Id: a.ID}); err != nil {
			return err
		}

		go func() {
			for cmd := range out {
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

		go func() {
			for {
				msg, err := stream.Recv()
				if err != nil {
					if err == io.EOF {
						close(in)
						break
					}

					if _, ok := status.FromError(err); ok {
						close(in)
						break
					}

					log.Errorf("unknown error from server: %v", err)
				}

				in <- string(msg.Content)
			}
		}()

		switch a.Output {
		case "raw":
			return a.setupRawOutput(cancel, in, out)
		default:
			return a.setupDefaultOutput(cancel, in, out)
		}
	})
}

func (a *AttachCmd) setupDefaultOutput(cancel func(), in <-chan string, out chan<- string) error {
	app := tview.NewApplication()

	output := tview.NewTextView()
	output.SetBorder(false)
	output.SetBackgroundColor(tcell.ColorDefault)
	output.SetScrollable(true)

	go func() {
		for content := range in {
			fmt.Fprint(output, string(content))
			app.Draw()
			app.QueueUpdate(func() {
				output.ScrollToEnd()
			})
		}

		app.Stop()
	}()

	input := tview.NewInputField()
	input.SetBorder(false)
	input.SetFieldWidth(0)
	input.SetBackgroundColor(tcell.ColorDefault)
	input.SetFieldBackgroundColor(tcell.ColorDefault)
	input.SetDoneFunc(func(key tcell.Key) {
		out <- input.GetText()
		input.SetText("")
	})

	layout := tview.NewFlex()
	layout.SetDirection(tview.FlexRow)
	layout.AddItem(output, 0, 1, false)
	layout.AddItem(hr(), 1, 0, false)
	layout.AddItem(input, 1, 0, true)

	return app.SetRoot(layout, true).Run()
}

func hr() *tview.Box {
	return tview.NewBox().
		SetBackgroundColor(tcell.ColorDefault).
		SetDrawFunc(func(screen tcell.Screen, x, y, width, height int) (int, int, int, int) {
			for cx := x; cx < x+width; cx++ {
				screen.SetContent(cx, y, tview.BoxDrawingsLightHorizontal, nil, tcell.StyleDefault.Foreground(tcell.ColorWhite))
			}

			return x, y, width, height
		})
}

func (a *AttachCmd) setupRawOutput(cancel func(), in <-chan string, out chan<- string) error {
	defer cancel()

	go func() {
		scanner := bufio.NewScanner(os.Stdin)

		for scanner.Scan() {
			out <- scanner.Text()
		}
	}()

	for msg := range in {
		fmt.Print(msg)
	}

	return nil
}
