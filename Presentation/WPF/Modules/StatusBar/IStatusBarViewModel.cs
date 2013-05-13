using Zander.Modules.StatusBar.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.StatusBar {
	public interface IStatusBarViewModel : IViewModel {
        StatusBarModel Model { get; set; }
    }
}
