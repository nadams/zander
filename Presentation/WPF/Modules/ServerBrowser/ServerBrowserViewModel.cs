using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain;
using Zander.Modules.ServerBrowser.Models;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserViewModel : NotificationObject, IServerBrowserViewModel {

		private readonly IMasterServerRepository masterServerRepository;
		private readonly IServerRepository serverRepository;

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

		public IMasterServerRepository MasterServerRepository {
			get {
				return this.masterServerRepository;
			}
		}

		public IServerRepository ServerRepository {
			get {
				return this.serverRepository;
			}
		}

		public ServerBrowserViewModel(IMasterServerRepository masterServerRepository, IServerRepository serverRepository) {
			this.Model = new ServerBrowserModel();
			this.masterServerRepository = masterServerRepository;
			this.serverRepository = serverRepository;
		}
	}
}
