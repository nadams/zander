using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : BaseViewModel, ISettingsViewModel {
        public event CloseWindowEventHandler CloseWindowEvent;
        private readonly IRegionManager regionManager;

        private ISettingViewCollection views;
        public IEnumerable<ISettingView> Views {
            get {
                return this.views.Views;
            }
        }

        private ISettingView currentView;
        public ISettingView CurrentView {
            get {
                return this.currentView;
            }

            set {
                this.currentView = value;
                if(value != null) {
                    var region = this.regionManager.Regions[SettingsRegions.SettingsContent];
                    if(!region.Views.Contains(value)) {
                        region.Add(value);
                    }

                    region.Activate(value);
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

        public SettingsViewModel(IRegionManager regionManager, ISettingViewCollection views) {
            this.regionManager = regionManager;
            this.views = views;
        }

        private void HandleCloseWindowEvent() {
            if(this.CloseWindowEvent != null) {
                this.CloseWindowEvent(this);
            }
        }
    }
}
