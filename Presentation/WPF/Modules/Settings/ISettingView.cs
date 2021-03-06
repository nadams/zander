using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Zander.Modules.Settings {
    public interface ISettingView {
        string SectionName { get; }
        bool IsSectionGroup { get; }
        IEnumerable<ISettingView> ChildViews { get; }
        UserControl View { get; }
    }
}
