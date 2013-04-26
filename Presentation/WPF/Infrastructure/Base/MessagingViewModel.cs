using Microsoft.Practices.Prism.Events;

namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
	public abstract class MessagingViewModel : BaseViewModel {

		protected readonly IEventAggregator eventAggregator;

		public MessagingViewModel(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;
		}
	}
}
