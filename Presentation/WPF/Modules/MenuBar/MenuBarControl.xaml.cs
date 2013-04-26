using System.Windows.Controls;

namespace Zander.Modules.MenuBar {
	public partial class MenuBarControl : UserControl, IMenuBarView {
		public IMenuBarViewModel ViewModel {
			get { return (IMenuBarViewModel)this.DataContext; }
			set { this.DataContext = value; }
		}

		public MenuBarControl(IMenuBarViewModel viewModel) {
			InitializeComponent();

			this.ViewModel = viewModel;
		}
	}
}
