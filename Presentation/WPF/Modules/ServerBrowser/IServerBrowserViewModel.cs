using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser {
	public interface IServerBrowserViewModel : IViewModel {
		ServerBrowserModel Model { get; set; }
	}
}
