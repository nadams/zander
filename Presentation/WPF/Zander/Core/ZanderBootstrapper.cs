using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace Zander.Presentation.WPF.Zander.Core {
    public class ZanderBootstrapper : UnityBootstrapper {
        protected override DependencyObject CreateShell() {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell() {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer() {
            base.ConfigureContainer();
        }

        protected override void ConfigureModuleCatalog() {
            base.ConfigureModuleCatalog();
        }
    }
}
