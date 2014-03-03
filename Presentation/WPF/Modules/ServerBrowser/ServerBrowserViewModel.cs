using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;
using Zander.Presentation.WPF.Zander.Services.ServerBrowser;

namespace Zander.Modules.ServerBrowser {
	public class ServerBrowserViewModel : NotificationObject, IServerBrowserViewModel {
		private readonly IEventAggregator eventAggregator;
        private readonly IServerBrowserService serverBrowserService;
        
        public ServerBrowserModel Model { get; set; }

		public ICommand QueryAllServers {
			get {
                return new DelegateCommand(this.serverBrowserService.QueryAllServers);
			}
		}

        public ICommand QueryCurrentServer {
            get {
                return new DelegateCommand(this.QueryCurrentServerCommand);
            }
        }

        public ICommand LaunchSelectedServer {
            get { 
                return new DelegateCommand(this.LaunchSelectedServerCommand); 
            }
        }

		public ServerBrowserViewModel(IEventAggregator eventAggregator, IServerBrowserService serverBrowserService) {
			this.Model = new ServerBrowserModel();
			this.eventAggregator = eventAggregator;
            this.serverBrowserService = serverBrowserService;

			this.eventAggregator.GetEvent<QueryAllServersEvent>().Subscribe(this.ExecuteQueryAllServers);
            this.eventAggregator.GetEvent<RefreshCurrentServerEvent>().Subscribe(this.ExecuteQueryCurrentServer);
			this.eventAggregator.GetEvent<ServerQueriedEvent>().Subscribe(this.ServerRefreshedAction, ThreadOption.UIThread);

            this.serverBrowserService.ServersChanged += this.CollectionChanged;
            this.serverBrowserService.DoneQueryingServers += o => this.eventAggregator.GetEvent<DoneQueryingServersEvent>().Publish(Empty.Value);
            this.serverBrowserService.TotalServersUpdated += (o, e) => this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Publish(e.TotalServers);
		}

        public void ExecuteQueryAllServers(Empty empty) {
            this.QueryAllServers.Execute(empty);
        }

        public void ExecuteQueryCurrentServer(Empty empty) {
            this.QueryCurrentServer.Execute(empty);
        }

        public void ServerRefreshedAction(Server server) {
            this.Model.AddServer(server);

            this.eventAggregator.GetEvent<CurrentServerQueryCountEvent>().Publish(this.Model.QueriedServers);
        }

        private void CollectionChanged(object sender, ServersCollectionChangedEventArgs args) {
            var changedValue = args.ChangedValue;

            switch(args.Action) {
                case ServersCollectionChangedActions.Add:
                    this.eventAggregator.GetEvent<ServerQueriedEvent>().Publish(changedValue);
                    break;

                case ServersCollectionChangedActions.Update:
                    this.Model.UpdateServer(changedValue);
                    break;

                case ServersCollectionChangedActions.Remove:
                    this.Model.RemoveServer(changedValue);
                    break;
            }
        }

        private void QueryCurrentServerCommand() {
            var selectedServer = this.Model.SelectedServer;

            if(selectedServer != null) {
                this.serverBrowserService.RefreshServer(selectedServer.Address);
            }
        }

        private void LaunchSelectedServerCommand() {
            var payload = new LaunchServerEventArgs {
                Address = this.Model.SelectedServer.Address.ToString()
            };

            this.eventAggregator.GetEvent<LaunchServerEvent>().Publish(payload);
        }
    }
}
