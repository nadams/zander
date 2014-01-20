using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using Zander.Modules.Settings.General;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : BaseViewModel, ISettingsViewModel {
        public event CloseWindowEventHandler CloseWindowEvent;
        public IEnumerable<ISettingView> Views { get; set; }

        private ISettingView currentView;
        public ISettingView CurrentView {
            get {
                return this.currentView;
            }

            set {
                this.currentView = value;
                this.RaisePropertyChanged(() => this.CurrentView);
            }
        }

        public DelegateCommand OkCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public DelegateCommand CancelCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView> {
                new GeneralView(new GeneralViewModel())
            };
        }

        private void HandleCloseWindowEvent() {
            if(this.CloseWindowEvent != null) {
                this.CloseWindowEvent(this);
            }
        }
    }
}
