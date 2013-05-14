using Zander.Domain.Entities;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public interface IServerBrowserService {
        void QueryAllServers();
        void RefreshServer(Server server);
    }
}
