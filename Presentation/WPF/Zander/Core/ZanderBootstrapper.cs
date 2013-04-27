using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Zander.Modules.MenuBar;
using Zander.Modules.ServerBrowser;
using Zander.Presentation.WPF.Zander.Extensions;

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

		protected override void ConfigureModuleCatalog() {
			base.ConfigureModuleCatalog();

			this.ModuleCatalog.RegisterModule<MenuBarModule>();
			this.ModuleCatalog.RegisterModule<ServerBrowserModule>();
		}
	}
}
