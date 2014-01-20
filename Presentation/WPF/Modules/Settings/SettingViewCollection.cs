using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Zander.Modules.Settings.Appearance;
using Zander.Modules.Settings.General;

namespace Zander.Modules.Settings {
    public class SettingViewCollection : ISettingViewCollection {
        private readonly IEnumerable<ISettingView> views;
        public IEnumerable<ISettingView> Views {
            get { return this.views; }
        }

        public SettingViewCollection(IUnityContainer container) {
            this.views = new List<ISettingView> {
                container.Resolve<IGeneralView>(),
                container.Resolve<IAppearanceView>()
            };
        }
    }
}
