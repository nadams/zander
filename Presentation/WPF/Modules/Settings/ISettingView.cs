using System.Collections.Generic;
namespace Settings {
    public interface ISettingView {
        string SectionName { get; }
        IEnumerable<ISettingView> ChildViews { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}
