using System.Windows.Controls;
using System.Windows.Input;

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

        private void DataGridMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var viewmodel = this.ViewModel;

            if(viewmodel.LaunchSelectedServer.CanExecute(null)) {
                viewmodel.LaunchSelectedServer.Execute(null);
            }
        }
	}
}
