using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Converters;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerBrowserModel : NotificationObject {

		private readonly object serversLock;

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

        private ServerModel selectedServer;
        public ServerModel SelectedServer {
            get {
                return this.selectedServer;
            }

            set {
                this.selectedServer = value;
                this.RaisePropertyChanged(() => this.SelectedServer);
            }
        }

		public int QueriedServers {
			get {
				lock(this.serversLock) {
					return this.Servers.Count;
				}
			}
		}

		public ServerBrowserModel() {
			this.serversLock = new object();
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

			lock(this.serversLock) {
				this.Servers.Add(model);
			}
		}
	}
}
