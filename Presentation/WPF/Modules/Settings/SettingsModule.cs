using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Zander.Modules.Settings.Appearance;
using Zander.Modules.Settings.Engines.Zandronum;
using Zander.Modules.Settings.General;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public class SettingsModule : BaseModule {
        public SettingsModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager) { }

        public override void Initialize() {
            this.container.RegisterType<ISettingViewCollection, SettingViewCollection>();
            this.container.RegisterType<IGeneralViewModel, GeneralViewModel>();
            this.container.RegisterType<IGeneralView, GeneralView>();
            this.container.RegisterType<IAppearanceViewModel, AppearanceViewModel>();
            this.container.RegisterType<IAppearanceView, AppearanceView>();
            this.container.RegisterInstance<IFileBrowserProvider>(new FileBrowserProvider());

            this.container.RegisterType<ISettingsViewModel, SettingsViewModel>();
            this.container.RegisterType<ISettingsWindow, SettingsWindow>();

            this.container.RegisterType<IZandronumEngineViewModel, ZandronumEngineViewModel>();
            this.container.RegisterType<IZandronumEngineView, ZandronumEngineView>();

            this.container.RegisterInstance<ZanderConfigProvider>(new ZanderConfigProvider());

            var eventAggregator = this.container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<SettingsEvent>().Subscribe(empty => this.container.Resolve<ISettingsWindow>().ShowDialog());
        }
    }
}
