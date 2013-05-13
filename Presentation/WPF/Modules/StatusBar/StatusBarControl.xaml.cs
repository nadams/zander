using System.Windows.Controls;

namespace Zander.Modules.StatusBar {
	public partial class StatusBarControl : UserControl, IStatusBarView {
		public IStatusBarViewModel ViewModel {
			get {
				return (IStatusBarViewModel)this.DataContext;
			}
			set {
				this.DataContext = value;
			}
		}

		public StatusBarControl() {
			InitializeComponent();
		}

        public StatusBarControl(IStatusBarViewModel viewModel) : this() {
			this.ViewModel = viewModel;
        }
	}
}
