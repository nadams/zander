using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.MenuBar.Models {
	public class MenuBarActions {
		private readonly IEventAggregator eventAggregator;

		public ICommand Quit {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<QuitEvent>().Publish(Empty.Value));
			}
		}

		public MenuBarActions(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;
		}
	}
}
