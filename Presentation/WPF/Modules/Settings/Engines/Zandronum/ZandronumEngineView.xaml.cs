using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Zander.Modules.Settings.Engines.Zandronum {
    public partial class ZandronumEngineView : UserControl, IZandronumEngineView {
        public IZandronumEngineViewModel ViewModel {
            get {
                return (IZandronumEngineViewModel)this.DataContext;
            }
            set {
                this.DataContext = value;
            }
        }

        public ZandronumEngineView(IZandronumEngineViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }

        public string SectionName {
            get { return Strings.ZandronumEngine; }
        }

        public IEnumerable<ISettingView> ChildViews {
            get { return Enumerable.Empty<ISettingView>(); }
        }

        public UserControl View {
            get { return this; }
        }

        public bool IsSectionGroup {
            get { return false; }
        }
    }
}
