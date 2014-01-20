using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Zander.Modules.Settings.General {
    public partial class GeneralView : UserControl, IGeneralView {
        public IGeneralViewModel ViewModel {
            get {
                return (IGeneralViewModel)this.DataContext;
            }
            set {
                this.DataContext = value;
            }
        }

        public string SectionName {
            get { return "General"; }
        }

        public IEnumerable<ISettingView> ChildViews {
            get { return Enumerable.Empty<ISettingView>(); }
        }

        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public GeneralView(IGeneralViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }

        public UserControl View {
            get { return this; }
        }
    }
}
