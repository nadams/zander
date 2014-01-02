using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Settings.General;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Settings {
    public class SettingsModule : BaseModule {
        public SettingsModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

        public override void Initialize() {
            this.container.RegisterType<ISettingsViewModel, SettingsViewModel>();
            this.container.RegisterType<ISettingsWindow, SettingsWindow>();
            this.container.RegisterType<IGeneralViewModel, GeneralViewModel>();
            this.container.RegisterType<IGeneralView, GeneralView>();

            var eventAggregator = this.container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<SettingsEvent>().Subscribe(empty => this.container.Resolve<ISettingsWindow>().ShowDialog());
        }
    }
}
