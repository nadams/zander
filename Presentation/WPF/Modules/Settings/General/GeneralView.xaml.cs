using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Settings.General {
    public partial class GeneralView : UserControl, IGeneralView, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private bool isSelected;
        public bool IsSelected {
            get {
                return this.isSelected;
            }

            set {
                this.isSelected = value;
                this.FireEvent("IsSelected");
            }
        }

        private bool isExpanded;
        public bool IsExpanded {
            get {
                return this.isExpanded;
            }

            set {
                this.isExpanded = value;
                this.FireEvent("IsExpanded");
            }
        }

        public IEnumerable<ISettingView> ChildViews {
            get { 
                return new List<ISettingView> { 
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                    new GeneralView(new GeneralViewModel()),
                }; 
            }
        }

        public GeneralView(IGeneralViewModel viewModel) {
            InitializeComponent();

            this.ViewModel = viewModel;
        }

        private void FireEvent(string propName) {
            if(this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
