using Microsoft.Practices.Prism.Events;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.MenuBar.Models {
	public class MenuBarViewModel : MessagingViewModel, IMenuBarViewModel {

		private MenuBarActions actions;
		public MenuBarActions Actions {
			get {
				return this.actions;
			}

			set {
				this.actions = value;
				this.RaisePropertyChanged(() => this.Actions);
			}
		}

		public MenuBarViewModel(IEventAggregator eventAggregator) : base(eventAggregator) {
			this.Actions = new MenuBarActions(eventAggregator);
		}
	}
}
