using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Converters;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerBrowserModel : NotificationObject {

		private readonly object serversLock;
        private readonly ServerEntityMapper serverMapper;

		private ObservableCollection<Server> servers;
		public ObservableCollection<Server> Servers {
			get {
				return this.servers;
			}

			set {
				this.servers = value;
				this.RaisePropertyChanged(() => this.Servers);
			}
		}

        private Server selectedServer;
        public Server SelectedServer {
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
            this.serverMapper = new ServerEntityMapper();

			this.Servers = this.GetNewServersModel();
		}

		private void ServersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			this.RaisePropertyChanged(() => this.Servers);
		}

		private ObservableCollection<Server> GetNewServersModel() {
			if(this.Servers != null) {
				this.Servers.CollectionChanged -= this.ServersChanged;
			}

			var servers = new ObservableCollection<Server>();
			servers.CollectionChanged += this.ServersChanged;

			return servers;
		}

		public void AddServer(Server server) {
			lock(this.serversLock) {
				this.Servers.Add(server);
			}
		}

        public void ResetServerList() {
            this.SelectedServer = null;
            this.Servers.Clear();
        }
	}
}
