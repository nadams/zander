using System.Windows.Input;
using Zander.Domain;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser {
	public interface IServerBrowserViewModel : IViewModel {
		ServerBrowserModel Model { get; set; }
		IMasterServerRepository MasterServerRepository { get; }
		IServerRepository ServerRepository { get; }
		ICommand QueryAllServers { get; }
	}
}
