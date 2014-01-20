using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Zander.Modules.Settings.Appearance {
    public partial class AppearanceView : UserControl, IAppearanceView {
        public IAppearanceViewModel ViewModel {
            get {
                return (IAppearanceViewModel)this.DataContext;
            }
            set {
                this.DataContext = value;
            }
        }

        public AppearanceView(IAppearanceViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }

        public string SectionName {
            get { return "Appearance"; }
        }

        public IEnumerable<ISettingView> ChildViews {
            get { return Enumerable.Empty<ISettingView>(); }
        }

        public UserControl View {
            get { return this; }
        }
    }
}
