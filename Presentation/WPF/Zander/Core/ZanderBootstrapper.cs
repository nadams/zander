using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Zander.Modules.MenuBar;
using Zander.Modules.ServerBrowser;
using Zander.Presentation.WPF.Zander.Extensions;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Presentation.WPF.Zander.Core {
	public class ZanderBootstrapper : UnityBootstrapper {
		protected override DependencyObject CreateShell() {
			return this.Container.Resolve<Shell>();
		}

		protected override void InitializeShell() {
			base.InitializeShell();

			App.Current.MainWindow = (Window)this.Shell;
			App.Current.MainWindow.Show();

			var queryAllServers = new Action(() => this.Container.Resolve<IEventAggregator>().GetEvent<QueryAllServersEvent>().Publish(Empty.Value));

			Dispatcher.CurrentDispatcher.BeginInvoke(queryAllServers, DispatcherPriority.ContextIdle);
		}

		protected override void ConfigureModuleCatalog() {
			base.ConfigureModuleCatalog();

			this.ModuleCatalog.RegisterModule<MenuBarModule>();
			this.ModuleCatalog.RegisterModule<ServerBrowserModule>();
		}
	}
}
