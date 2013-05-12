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
			}
		}
	}
}
