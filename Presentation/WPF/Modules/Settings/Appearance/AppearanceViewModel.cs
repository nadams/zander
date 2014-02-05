namespace Zander.Modules.Settings.Appearance {
    public class AppearanceViewModel : IAppearanceViewModel {
        private readonly ZanderConfigProvider configProvider;

        public AppearanceViewModel(ZanderConfigProvider configProvider) {
            this.configProvider = configProvider;
        }
    }
}
