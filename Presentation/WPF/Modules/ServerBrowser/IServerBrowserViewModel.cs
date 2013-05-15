using System.Windows.Input;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser {
	public interface IServerBrowserViewModel : IViewModel {
		ServerBrowserModel Model { get; set; }
		ICommand QueryAllServers { get; }
        ICommand QueryCurrentServer { get; }
        ICommand LaunchSelectedServer { get; }
	}
}
