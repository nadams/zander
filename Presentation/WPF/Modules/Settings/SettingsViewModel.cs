using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : ISettingsViewModel {
        public IEnumerable<ISettingView> Views { get; set; }

        public DelegateCommand OkCommand {
            get { return new DelegateCommand(() => MessageBox.Show("Ok Clicked")); }
        }

        public DelegateCommand CancelCommand {
            get { return new DelegateCommand(() => MessageBox.Show("Cancel Clicked")); }
        }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView>();
        }
    }
}
