using Zander.Domain.Config;
namespace Zander.Modules.Settings.General {
    public class GeneralViewModel : IGeneralViewModel {
        private readonly IZanderConfigService configService;

        public GeneralViewModel(IZanderConfigService configService) {
            this.configService = configService;
        }
    }
}
