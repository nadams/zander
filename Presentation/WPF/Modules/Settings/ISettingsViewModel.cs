using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.Settings {
    public interface ISettingsViewModel : IViewModel {
        IEnumerable<ISettingView> Views { get; set; }
        DelegateCommand OkCommand { get; }
        DelegateCommand CancelCommand { get; }
    }
}
