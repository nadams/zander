using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Converters;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerBrowserModel : NotificationObject {

		private ObservableCollection<ServerModel> servers;
		public ObservableCollection<ServerModel> Servers {
			get {
				return this.servers;
			}

			set {
				this.servers = value;
				this.RaisePropertyChanged(() => this.Servers);
			}
		}

		public ServerBrowserModel() {
			this.Servers = this.GetNewServersModel();
		}

		private void ServersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			this.RaisePropertyChanged(() => this.Servers);
		}

		private ObservableCollection<ServerModel> GetNewServersModel() {
			if(this.Servers != null) {
				this.Servers.CollectionChanged -= this.ServersChanged;
			}

			var servers = new ObservableCollection<ServerModel>();
			servers.CollectionChanged += this.ServersChanged;

			return servers;
		}

		public void AddServer(Server server) {
			var mapper = new ServerEntityMapper();

			var model = mapper.ModelFromEntity(server);

			this.Servers.Add(model);
		}
	}
}
