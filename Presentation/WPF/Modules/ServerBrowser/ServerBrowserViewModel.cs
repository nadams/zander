using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserViewModel : NotificationObject, IServerBrowserViewModel {
		private readonly IMasterServerRepository masterServerRepository;
		private readonly IServerRepository serverRepository;
		private readonly IEventAggregator eventAggregator;

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

		public ICommand QueryAllServers {
			get {
				return new DelegateCommand(this.QueryAllServersCommand);
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

			this.eventAggregator.GetEvent<QueryAllServersEvent>().Subscribe(empty => this.QueryAllServers.Execute(null));

			this.eventAggregator.GetEvent<ServerQueriedEvent>().Subscribe(server => {
				this.model.AddServer(server);

				this.eventAggregator.GetEvent<CurrentServerQueryCountEvent>().Publish(this.model.QueriedServers);
			}, ThreadOption.UIThread);
		}

		private void QueryAllServersCommand() {
			Task.Factory.StartNew(() => {
				var masterServer = this.GetMasterServer();

				this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Publish(masterServer.Servers.Count());

				Parallel.ForEach(masterServer.Servers, (server, status) => {
					var address = server.Address.ToString() + ":" + server.Port;

					try {
						var entity = this.serverRepository.Get(address, 1000, ServerQueryValues.AllData);

						this.eventAggregator.GetEvent<ServerQueriedEvent>().Publish(entity);
					} catch { }
				});

                this.eventAggregator.GetEvent<DoneQueryingServersEvent>().Publish(Empty.Value);
			});
		}

		private IMasterServer GetMasterServer() {
			var masterServer = this.MasterServerRepository.Get("64.15.129.183:15300", 5000);

			return masterServer;
		}

        private void LaunchSelectedServerCommand() {

        }
    }

	public class ServerQueriedEvent : CompositePresentationEvent<Server> { }
}
