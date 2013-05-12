using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.StatusBar {
	public class StatusBarViewModel : NotificationObject, IStatusBarViewModel {
		private readonly IEventAggregator eventAggregator;

		public StatusBarViewModel(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;

			this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Subscribe(count => { });
		}
	}
}
