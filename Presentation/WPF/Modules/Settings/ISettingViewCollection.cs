using System.Collections.Generic;

namespace Zander.Modules.Settings {
    public interface ISettingViewCollection {
        IEnumerable<ISettingView> Views { get; }
    }
}
