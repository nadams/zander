using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserViewModel : NotificationObject, IServerBrowserViewModel {
		private readonly IMasterServerRepository masterServerRepository;
		private readonly IServerRepository serverRepository;
		private readonly IEventAggregator eventAggregator;
        private readonly ServerQuery serverQuery;
        
        public ServerBrowserModel Model { get; set; }

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

		public ICommand QueryAllServers {
			get {
                return new DelegateCommand(this.serverQuery.QueryAllServers);
			}
		}

        public ICommand QueryCurrentServer {
            get {
                return new DelegateCommand(this.serverQuery.RefreshCurrentServer);
            }
        }

        public ICommand LaunchSelectedServer {
            get { 
                return new DelegateCommand(this.LaunchSelectedServerCommand); 
            }
        }

		public ServerBrowserViewModel(IEventAggregator eventAggregator, IMasterServerRepository masterServerRepository, IServerRepository serverRepository) {
			this.Model = new ServerBrowserModel();
			this.masterServerRepository = masterServerRepository;
			this.serverRepository = serverRepository;
			this.eventAggregator = eventAggregator;
            this.serverQuery = new ServerQuery(this.eventAggregator, this.serverRepository, this.masterServerRepository, this.Model);

			this.eventAggregator.GetEvent<QueryAllServersEvent>().Subscribe(empty => this.QueryAllServers.Execute(null));
            this.eventAggregator.GetEvent<RefreshCurrentServerEvent>().Subscribe(empty => this.QueryCurrentServer.Execute(null));
			this.eventAggregator.GetEvent<ServerQueriedEvent>().Subscribe(server => {
				this.Model.AddServer(server);

				this.eventAggregator.GetEvent<CurrentServerQueryCountEvent>().Publish(this.Model.QueriedServers);
			}, ThreadOption.UIThread);
		}

        private void LaunchSelectedServerCommand() {

        }
    }
}
