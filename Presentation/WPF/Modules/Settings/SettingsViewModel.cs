using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;
using System.Linq;
using Zander.Domain.Config;

namespace Zander.Modules.Settings {
    public class SettingsViewModel : BaseViewModel, ISettingsViewModel {
        public event CloseWindowEventHandler CloseWindowEvent;

        private readonly IZanderConfigService configService;

        private ISettingViewCollection views;
        public IEnumerable<ISettingView> Views {
            get {
                return this.views.Views;
            }
        }

        private UserControl currentView;
        public UserControl CurrentView {
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

        public DelegateCommand<ISettingView> ChangeSelectedItem {
            get {
                return new DelegateCommand<ISettingView>(view => this.CurrentView = view.View);
            }
        }

        public SettingsViewModel(ISettingViewCollection views, IZanderConfigService configService) {
            this.views = views;
            this.configService = configService;

            var firstView = this.Views.FirstOrDefault();
            if(firstView != null) {
                this.CurrentView = firstView.View;
            }
        }

        private void HandleCloseWindowEvent() {
            if(this.CloseWindowEvent != null) {
                this.CloseWindowEvent(this);
            }
        }
    }
}
