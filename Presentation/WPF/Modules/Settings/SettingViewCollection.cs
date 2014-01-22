using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Zander.Domain.Config;
using Zander.Modules.Settings.Appearance;
using Zander.Modules.Settings.Engines;
using Zander.Modules.Settings.General;

namespace Zander.Modules.Settings {
    public class SettingViewCollection : ISettingViewCollection {
        private readonly IEnumerable<ISettingView> views;
        public IEnumerable<ISettingView> Views {
            get { return this.views; }
        }

        public SettingViewCollection(IUnityContainer container) {
            var configService = container.Resolve<IZanderConfigService>();
            var config = configService.CloneConfig(configService.GetDefaultConfig());
            this.views = new List<ISettingView> {
                container.Resolve<IGeneralView>(),
                container.Resolve<EnginesPlaceholder>(),
                container.Resolve<IAppearanceView>()
            };
        }
    }
}
