using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Settings.General;

namespace Settings {
    public class SettingsViewModel : ISettingsViewModel {
        public IEnumerable<ISettingView> Views { get; set; }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView> {
                container.Resolve<IGeneralView>()
            };
        }
    }
}
