using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Modules.StatusBar.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.StatusBar {
	public class StatusBarViewModel : NotificationObject, IStatusBarViewModel {
		private readonly IEventAggregator eventAggregator;

		private StatusBarModel statusBarModel;
		public StatusBarModel StatusBarModel {
			get {
				return this.statusBarModel;
			}

			set {
				this.statusBarModel = value;
				this.RaisePropertyChanged(() => this.StatusBarModel);
			}
		}

		public StatusBarViewModel(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;

			this.StatusBarModel = new StatusBarModel();

			this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Subscribe(count => this.StatusBarModel.TotalServers = count);
			this.eventAggregator.GetEvent<CurrentServerQueryCountEvent>().Subscribe(count => this.StatusBarModel.ServersQueried = count);
            this.eventAggregator.GetEvent<DoneQueryingServersEvent>().Subscribe(empty => { 
                this.StatusBarModel.TotalServers = this.StatusBarModel.ServersQueried; 

            });
		}
	}
}
