using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Zander.Domain.Entities;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public interface IServerBrowserService {
        event ServersCollectionChangedEventHandler ServersChanged;
        event TotalServersUpdatedEventHandler TotalServersUpdated;
        event DoneQueryingServersEventHandler DoneQueryingServers;

        IEnumerable<Server> Servers { get; }
        bool CanRefresh { get; }

        void QueryAllServers();
        void RefreshServer(IPEndPoint endPoint);
        void AddServer(Server server);
        void RemoveServer(IPEndPoint endPoint);
    }
}
