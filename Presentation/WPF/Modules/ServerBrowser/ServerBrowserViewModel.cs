using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
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
                return new DelegateCommand(() => this.serverBrowserService.RefreshServer(this.Model.SelectedServer));
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
