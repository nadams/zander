using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : ISettingsViewModel {
        public event CloseWindowEventHandler CloseWindowEvent;
        public IEnumerable<ISettingView> Views { get; set; }

        public DelegateCommand OkCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public DelegateCommand CancelCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView>();
        }

        private void HandleCloseWindowEvent() {
            if(this.CloseWindowEvent != null) {
                this.CloseWindowEvent(this);
            }
        }
    }
}
