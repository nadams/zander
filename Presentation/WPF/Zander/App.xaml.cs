using System.Windows;
using Zander.Presentation.WPF.Zander.Core;

namespace Zander {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var bootstrapper = new ZanderBootstrapper();
            bootstrapper.Run();
        }
    }
}
