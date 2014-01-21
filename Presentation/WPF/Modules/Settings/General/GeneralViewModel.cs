using Zander.Domain.Config;

namespace Zander.Modules.Settings.General {
    public class GeneralViewModel : IGeneralViewModel {
        private readonly ZanderConfigProvider configProvider;

        public GeneralViewModel(ZanderConfigProvider configProvider) {
            this.configProvider = configProvider;
        }
    }
}
