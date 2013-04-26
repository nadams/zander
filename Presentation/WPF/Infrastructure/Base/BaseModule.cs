using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
	public abstract class BaseModule : IModule {
		protected readonly IUnityContainer container;
		protected readonly IRegionManager regionManager;

		public BaseModule(IUnityContainer container, IRegionManager regionManager) {
			this.container = container;
			this.regionManager = regionManager;
		}

		public abstract void Initialize();
	}
}
