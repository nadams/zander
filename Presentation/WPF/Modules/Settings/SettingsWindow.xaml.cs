using System.Windows;

namespace Zander.Modules.Settings {
    public partial class SettingsWindow : Window, ISettingsWindow {
        public ISettingsViewModel ViewModel {
            get {
                return (ISettingsViewModel)this.DataContext;
            }

            set {
                this.DataContext = value;
            }
        }

        public SettingsWindow(ISettingsViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
            this.ViewModel.CloseWindowEvent += sender => this.Close();
        }
    }
}
