using System.Windows.Controls;

namespace Zander.Modules.ServerBrowser {
	public partial class ServerBrowserControl : UserControl, IServerBrowserView {
		public IServerBrowserViewModel ViewModel {
			get {
				return (IServerBrowserViewModel)this.DataContext;
			}

			set {
				this.DataContext = value;
			}
		}

		public ServerBrowserControl() {
			InitializeComponent();
		}

		public ServerBrowserControl(IServerBrowserViewModel viewModel) : this() {
			this.ViewModel = viewModel;
		}
	}
}
