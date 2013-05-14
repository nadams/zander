using System.Collections.Generic;
using System.Collections.Specialized;
using Zander.Domain.Entities;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public interface IServerBrowserService {
        event NotifyCollectionChangedEventHandler CollectionChanged;
        event TotalServersUpdatedEventHandler TotalServersUpdated;
        event DoneQueryingServersEventHandler DoneQueryingServers;

        IEnumerable<Server> Servers { get; }

        void QueryAllServers();
        void RefreshServer(Server server);
        void AddServer(Server server);
        void RemoveServer(Server server);
    }
}
