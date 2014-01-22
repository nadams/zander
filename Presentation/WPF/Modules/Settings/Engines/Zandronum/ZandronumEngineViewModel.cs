using Zander.Presentation.WPF.Zander.Infrastructure.Base;
namespace Zander.Modules.Settings.Engines.Zandronum {
    public class ZandronumEngineViewModel : BaseViewModel, IZandronumEngineViewModel {
        private readonly ZanderConfigProvider configProvider;

        public string MasterAddress {
            get {
                return this.configProvider.Config.ZandronumMasterAddress;
            }

            set {
                this.configProvider.Config.ZandronumMasterAddress = value;
                this.RaisePropertyChanged(() => this.MasterAddress);
            }
        }

        public ZandronumEngineViewModel(ZanderConfigProvider configProvider) {
            this.configProvider = configProvider;
        }
    }
}
