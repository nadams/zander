using System;
using System.Windows;
using Zander.Domain.Config;

namespace Zander.Presentation.WPF.Zander.Core {
    public class WindowClosingEventHandler {
        private readonly IZanderConfigService configService;

        public WindowClosingEventHandler(IZanderConfigService configService) {
            this.configService = configService;
        }

        public void OnWindowClosing(object sender, EventArgs args) {
            var window = (Window)sender;
            var config = this.configService.GetDefaultConfig();

            config.WindowHeight = window.Height;
            config.WindowWidth = window.Width;

            this.configService.SaveConfig(config);
        }
    }
}
