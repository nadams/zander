using Microsoft.Practices.Prism.Events;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.MenuBar.Models {
	public class MenuBarViewModel : MessagingViewModel, IMenuBarViewModel {

		private MenuBarStrings strings;
		public MenuBarStrings Strings {
			get {
				return this.strings;
			}

			set {
				this.strings = value;
				this.RaisePropertyChanged(() => this.Strings);
			}
		}

		public MenuBarViewModel(IEventAggregator eventAggregator) : base(eventAggregator) {
			this.Strings = new MenuBarStrings();
		}

	}
}
