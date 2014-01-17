using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.Settings {
    public interface ISettingsViewModel : IViewModel {
        event CloseWindowEventHandler CloseWindowEvent;

        IEnumerable<ISettingView> Views { get; set; }
        ISettingView CurrentView { get; set; }
        DelegateCommand OkCommand { get; }
        DelegateCommand CancelCommand { get; }
    }
}
