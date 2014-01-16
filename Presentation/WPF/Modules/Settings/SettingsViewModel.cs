using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : ISettingsViewModel {
        public IEnumerable<ISettingView> Views { get; set; }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView>();
        }
    }
}
