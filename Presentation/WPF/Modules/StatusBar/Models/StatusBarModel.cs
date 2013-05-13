using Microsoft.Practices.Prism.ViewModel;

namespace Zander.Modules.StatusBar.Models {
	public class StatusBarModel : NotificationObject {

		private int totalServers;
		public int TotalServers {
			get {
				return this.totalServers;
			}

			set {
				this.totalServers = value;
				this.RaisePropertyChanged(() => this.TotalServers);
				this.RaisePropertyChanged(() => this.ServerQueryStatus);
                this.RaisePropertyChanged(() => this.IsQuerying);
			}
		}

		private int serversQueried;
		public int ServersQueried {
			get {
				return this.serversQueried;
			}

			set {
				this.serversQueried = value;
				this.RaisePropertyChanged(() => this.ServersQueried);
				this.RaisePropertyChanged(() => this.ServerQueryStatus);
                this.RaisePropertyChanged(() => this.IsQuerying);
			}
		}

		public string ServerQueryStatus {
			get {
				return this.ServersQueried + " / " + this.TotalServers;
			}
		}

        private bool IsQuerying {
            get {
                return this.ServersQueried < this.TotalServers;
            }
        }
	}
}
