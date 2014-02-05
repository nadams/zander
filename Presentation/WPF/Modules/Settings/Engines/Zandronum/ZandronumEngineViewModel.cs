using System;
using Microsoft.Practices.Prism.Commands;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.Settings.Engines.Zandronum {
    public class ZandronumEngineViewModel : BaseViewModel, IZandronumEngineViewModel {
        private readonly ZanderConfigProvider configProvider;
        private readonly IFileBrowserProvider fileBrowser;

        public string MasterAddress {
            get {
                return this.configProvider.Config.ZandronumConfig.MasterAddress;
            }

            set {
                this.configProvider.Config.ZandronumConfig.MasterAddress = value;
                this.RaisePropertyChanged(() => this.MasterAddress);
            }
        }

        public string PathToClinetBinary {
            get {
                return this.configProvider.Config.ZandronumConfig.PathToClinetBinary;
            }

            set {
                this.configProvider.Config.ZandronumConfig.PathToClinetBinary = value;
                this.RaisePropertyChanged(() => this.PathToClinetBinary);
            }
        }

        public string PathToServerBinary {
            get {
                return this.configProvider.Config.ZandronumConfig.PathToServerBinary;
            }

            set {
                this.configProvider.Config.ZandronumConfig.PathToServerBinary = value;
                this.RaisePropertyChanged(() => this.PathToServerBinary);
            }
        }

        public string CustomParameters {
            get {
                return this.configProvider.Config.ZandronumConfig.CustomParameters;
            }

            set {
                this.configProvider.Config.ZandronumConfig.CustomParameters = value;
                this.RaisePropertyChanged(() => this.CustomParameters);
            }
        }

        public DelegateCommand BrowseForClientBinary {
            get {
                return new DelegateCommand(() => this.SetBinary(x => this.PathToClinetBinary = x));
            }
        }

        public DelegateCommand BrowseForServerBinary {
            get {
                return new DelegateCommand(() => this.SetBinary(x => this.PathToServerBinary = x));
            }
        }

        public ZandronumEngineViewModel(ZanderConfigProvider configProvider, IFileBrowserProvider fileBrowser) {
            this.configProvider = configProvider;
            this.fileBrowser = fileBrowser;
        }

        private void SetBinary(Action<string> setAction) {
            string binary = this.fileBrowser.BrowseForFile();

            if(binary != null) {
                setAction(binary);
            }
        }
    }
}
