using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Modules.StatusBar.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.StatusBar {
	public class StatusBarViewModel : NotificationObject, IStatusBarViewModel {
		private readonly IEventAggregator eventAggregator;

		private StatusBarModel model;
		public StatusBarModel Model {
			get {
				return this.model;
			}

			set {
				this.model = value;
				this.RaisePropertyChanged(() => this.Model);
			}
		}

		public StatusBarViewModel(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;

			this.Model = new StatusBarModel();

			this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Subscribe(count => this.Model.TotalServers = count);
			this.eventAggregator.GetEvent<CurrentServerQueryCountEvent>().Subscribe(count => this.Model.ServersQueried = count);
            this.eventAggregator.GetEvent<DoneQueryingServersEvent>().Subscribe(empty => this.Model.TotalServers = this.Model.ServersQueried);
		}
	}
}
