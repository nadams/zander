using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Zander.Modules.Settings.Engines.Zandronum;

namespace Zander.Modules.Settings.Engines {
    public partial class EnginesPlaceholder : UserControl, ISettingView {
        private readonly IUnityContainer container;

        public string SectionName {
            get { return "Engines"; }
        }

        public bool IsSectionGroup {
            get { return true; }
        }

        public IEnumerable<ISettingView> ChildViews {
            get {
                return new List<ISettingView> {
                    container.Resolve<IZandronumEngineView>()
                };
            }
        }

        public UserControl View {
            get { return this; }
        }

        public EnginesPlaceholder(IUnityContainer container) {
            InitializeComponent();

            this.container = container;
        }
    }
}
