using System.Collections.Generic;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Modules.Settings.Appearance;
using Zander.Modules.Settings.General;

namespace Zander.Modules.Settings {
    public class SettingViewCollection : ISettingViewCollection {
        private readonly IEnumerable<ISettingView> views;
        public IEnumerable<ISettingView> Views {
            get { return this.views; }
        }

        public SettingViewCollection(IUnityContainer container, IRegionManager regionManager) {
            var general = container.Resolve<IGeneralView>();
            var appearance = container.Resolve<IAppearanceView>();

            var region = regionManager.Regions[SettingsRegions.SettingsContent];
            region.Add(general);
            region.Add(appearance);

            this.views = new List<ISettingView> {
                general,
                appearance
            };
        }
    }
}
