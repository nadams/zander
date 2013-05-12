using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Presentation.WPF.Zander.Infrastructure;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.StatusBar {
	public class StatusBarModule : BaseModule {

		public StatusBarModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

		public override void Initialize() {
			this.container.RegisterType<IStatusBarViewModel, StatusBarViewModel>();
			this.container.RegisterType<IStatusBarView, StatusBarControl>();

			this.regionManager.RegisterViewWithRegion(Regions.StatusBar, () => this.container.Resolve<IStatusBarView>());
		}
	}
}
