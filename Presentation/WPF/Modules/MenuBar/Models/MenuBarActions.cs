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

		public ICommand Refresh {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<RefreshEvent>().Publish(Empty.Value));
			}
		}

		public ICommand AddToIgnoreList {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<AddServerToIgnoreListEvent>().Publish(Empty.Value));
			}
		}

		public ICommand EditCustomServers {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<EditCustomServersEvent>().Publish(Empty.Value));
			}
		}

		public ICommand EditIgnoreList {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<EditIgnoreListEvent>().Publish(Empty.Value));
			}
		}

		public ICommand About {
			get {
				return new DelegateCommand(() => this.eventAggregator.GetEvent<AboutEvent>().Publish(Empty.Value));
			}
		}

		public MenuBarActions(IEventAggregator eventAggregator) {
			this.eventAggregator = eventAggregator;
		}
	}
}
