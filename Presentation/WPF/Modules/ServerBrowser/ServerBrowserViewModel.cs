using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Domain;
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

		public ServerBrowserViewModel(IEventAggregator eventAggregator, IMasterServerRepository masterServerRepository, IServerRepository serverRepository) {
			this.Model = new ServerBrowserModel();
			this.masterServerRepository = masterServerRepository;
			this.serverRepository = serverRepository;
			this.eventAggregator = eventAggregator;

			this.eventAggregator.GetEvent<QueryAllServersEvent>().Subscribe(empty => {
				var masterServer = this.MasterServerRepository.Get("64.15.129.183:15300", 5000);
				foreach(var item in masterServer.Servers) {
					this.model.Servers.Add(new ServerModel { 
						Address = item.Address.ToString()
					});
				}
			});
		}
	}
}
