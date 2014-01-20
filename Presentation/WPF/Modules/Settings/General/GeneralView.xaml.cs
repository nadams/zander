using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Zander.Modules.Settings.General {
    public partial class GeneralView : UserControl, IGeneralView, ISettingView {
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
            get { 
                return new List<ISettingView> { 
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                }; 
            }
        }

        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public GeneralView(IGeneralViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }
    }
}
