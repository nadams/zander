using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace MenuBar {
	public class MenuBarModule : BaseModule {
		public MenuBarModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

		public override void Initialize() {

		}
	}
}
