using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Domain;
using Zander.Domain.Remote;
using Zander.Presentation.WPF.Zander.Infrastructure;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Provider.Net.Sockets;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserModule : BaseModule {

		public ServerBrowserModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

		public override void Initialize() {
			this.container.RegisterType<IServerBrowserViewModel, ServerBrowserViewModel>();
			this.container.RegisterType<IServerBrowserView, ServerBrowserControl>();
			this.container.RegisterType<IMasterServerRepository, ZandronumMasterServerRepository>();
			this.container.RegisterType<IServerRepository, ServerRepository>();
			this.container.RegisterType<IRemoteServerApiProvider, RemoteServerApiProvider>();

			this.regionManager.RegisterViewWithRegion(Regions.ServerBrowser, () => this.container.Resolve<IServerBrowserView>());
		}
	}
}
