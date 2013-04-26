using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Presentation.WPF.Zander.Infrastructure;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.MenuBar {
	public class MenuBarModule : BaseModule {
		public MenuBarModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

		public override void Initialize() {
			this.container.RegisterType<IMenuBarViewModel, MenuBarviewModel>();
			this.container.RegisterType<IMenuBarView, MenuBarControl>();

			this.regionManager.RegisterViewWithRegion(Regions.MenuBar, () => this.container.Resolve<IMenuBarView>());
		}
	}
}
