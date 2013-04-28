using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Modules.ServerBrowser.Models;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserViewModel : NotificationObject, IServerBrowserViewModel {

		private ServerBrowserModel model;
		public ServerBrowserModel Model {
			get {
				return this.model;
			}

			set {
				this.model = value;
				this.RaisePropertyChanged(() => this.Model);
			}
		}

		public ServerBrowserViewModel() {
			this.Model = new ServerBrowserModel();
		}
	}
}
