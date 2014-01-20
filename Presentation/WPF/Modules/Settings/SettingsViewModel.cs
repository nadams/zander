using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Zander.Modules.Settings.General;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : BaseViewModel, ISettingsViewModel {
        public event CloseWindowEventHandler CloseWindowEvent;
        private readonly IRegionManager regionManager;

        public IEnumerable<ISettingView> Views { get; set; }

        private ISettingView currentView;
        public ISettingView CurrentView {
            get {
                return this.currentView;
            }

            set {
                this.currentView = value;
                this.RaisePropertyChanged(() => this.CurrentView);
                if(value != null) {
                    this.regionManager.Regions[SettingsRegions.SettingsContent].Activate(value);
                }
            }
        }

        public DelegateCommand OkCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public DelegateCommand CancelCommand {
            get { return new DelegateCommand(this.HandleCloseWindowEvent); }
        }

        public DelegateCommand<ISettingView> ChangeSelectedItem {
            get {
                return new DelegateCommand<ISettingView>(view => this.CurrentView = view);
            }
        }

        public SettingsViewModel(IRegionManager regionManager) {
            this.regionManager = regionManager;
            this.Views = new List<ISettingView> {
                new GeneralView(new GeneralViewModel()),
                new GeneralView(new GeneralViewModel()),
                new GeneralView(new GeneralViewModel()),
                new GeneralView(new GeneralViewModel()),
            };
        }

        private void HandleCloseWindowEvent() {
            if(this.CloseWindowEvent != null) {
                this.CloseWindowEvent(this);
            }
        }
    }
}
