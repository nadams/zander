using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Zander.Domain;
using Zander.Domain.Remote;
using Zander.Modules.MenuBar;
using Zander.Modules.ServerBrowser;
using Zander.Modules.StatusBar;
using Zander.Presentation.WPF.Zander.Extensions;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;
using Zander.Presentation.WPF.Zander.Services.ServerBrowser;
using Zander.Provider.Net.Sockets;

namespace Zander.Presentation.WPF.Zander.Core {
	public class ZanderBootstrapper : UnityBootstrapper {
		protected override DependencyObject CreateShell() {
			return this.Container.Resolve<Shell>();
		}

		protected override void InitializeShell() {
			base.InitializeShell();

			App.Current.MainWindow = (Window)this.Shell;

			this.Container.Resolve<IEventAggregator>().GetEvent<QuitEvent>().Subscribe(empty => App.Current.MainWindow.Close());

			App.Current.MainWindow.Show();

			Action queryAllServers = () => this.Container.Resolve<IEventAggregator>().GetEvent<QueryAllServersEvent>().Publish(Empty.Value);

			Dispatcher.CurrentDispatcher.BeginInvoke(queryAllServers, DispatcherPriority.ContextIdle);
		}

        protected override void ConfigureContainer() {
            base.ConfigureContainer();

            this.Container.RegisterType<IMasterServerRepository, ZandronumMasterServerRepository>();
            this.Container.RegisterType<IServerRepository, ServerRepository>();
            this.Container.RegisterType<IRemoteServerApiProvider, RemoteServerApiProvider>();
            this.Container.RegisterType<IServerBrowserService, ServerBrowserService>(new ContainerControlledLifetimeManager());
        }

		protected override void ConfigureModuleCatalog() {
			base.ConfigureModuleCatalog();

			this.ModuleCatalog.RegisterModule<MenuBarModule>();
			this.ModuleCatalog.RegisterModule<StatusBarModule>();
			this.ModuleCatalog.RegisterModule<ServerBrowserModule>();
		}
	}
}
