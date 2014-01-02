using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Settings.General {
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

        public GeneralView(IGeneralViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }
    }
}
