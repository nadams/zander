using System.Collections.Generic;
using System.ComponentModel;

namespace Zander.Modules.Settings {
    public interface ISettingView : INotifyPropertyChanged {
        string SectionName { get; }
        IEnumerable<ISettingView> ChildViews { get; }
    }
}
