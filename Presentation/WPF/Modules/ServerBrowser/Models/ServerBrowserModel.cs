using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Converters;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerBrowserModel : NotificationObject {

        private readonly ServerEntityMapper serverMapper;

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
                this.RaisePropertyChanged(() => this.HasCurrentServer);
            }
        }

		public int QueriedServers {
			get {
				return this.Servers.Count;
			}
		}

        public bool HasCurrentServer {
            get {
                return this.SelectedServer != null;
            }
        }

		public ServerBrowserModel() {
            this.serverMapper = new ServerEntityMapper();

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
            var model = this.serverMapper.ModelFromEntity(server);

		    this.Servers.Add(model);
		}

        public void UpdateServer(Server server) {
            var model = this.serverMapper.ModelFromEntity(server);

            var existingModel = this.Servers.Single(x => x.Address.ToString() == server.IPEndPoint.ToString());

            existingModel.CopyData(model);

            this.RaisePropertyChanged(() => this.Servers);
        }

        public void RemoveServer(Server server) {
            var model = this.Servers.Single(x => x.Address.ToString() == server.IPEndPoint.ToString());

            this.Servers.Remove(model);

            this.RaisePropertyChanged(() => this.Servers);
        }

        public void ResetServerList() {
            this.SelectedServer = null;
            this.Servers.Clear();
        }
	}
}
