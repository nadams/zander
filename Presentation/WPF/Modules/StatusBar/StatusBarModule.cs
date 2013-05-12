using System;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.StatusBar {
	public class StatusBarModule : BaseModule {

		public StatusBarModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

		public override void Initialize() {
			throw new NotImplementedException();
		}
	}
}
