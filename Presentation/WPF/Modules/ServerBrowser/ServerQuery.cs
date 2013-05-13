using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Modules.ServerBrowser {
    public class ServerQuery {
        private const int RefreshRestTime = 15;

        private readonly IEventAggregator eventAggregator;
        private readonly IServerRepository serverRepository;
        private readonly IMasterServerRepository masterServerRepository;

        private DateTime lastQueriedTime;
        private bool isQuerying;

        public ServerBrowserModel Model { get; set; }

        private bool CanRefresh {
            get {
                var refreshAtTime = DateTime.UtcNow.AddSeconds(RefreshRestTime);
                var duration = refreshAtTime - this.lastQueriedTime;

                bool canRefresh = duration.Duration().Seconds > RefreshRestTime;

                return !this.isQuerying && canRefresh;
            }
        }

        public ServerQuery(IEventAggregator eventAggregator, IServerRepository serverRepo, IMasterServerRepository masterRepo, ServerBrowserModel model) {
            this.eventAggregator = eventAggregator;
            this.serverRepository = serverRepo;
            this.masterServerRepository = masterRepo;
            this.lastQueriedTime = DateTime.MinValue;

            this.Model = model;
        }

        public void QueryAllServers() {
            if(this.CanRefresh) {
                this.Model.ResetServerList();

                Task.Factory.StartNew(() => {
                    this.isQuerying = true;

                    var masterServer = this.GetMasterServer();

                    this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Publish(masterServer.Servers.Count());

                    Parallel.ForEach(masterServer.Servers, (server, status) => {
                        var address = server.Address.ToString() + ":" + server.Port;

                        try {
                            var entity = this.serverRepository.Get(address, 1000, ServerQueryValues.AllData);

                            this.eventAggregator.GetEvent<ServerQueriedEvent>().Publish(entity);
                        } catch { }
                    });

                    this.eventAggregator.GetEvent<DoneQueryingServersEvent>().Publish(Empty.Value);

                    this.isQuerying = false;
                    this.lastQueriedTime = DateTime.UtcNow;
                });
            }
        }

        private IMasterServer GetMasterServer() {
            var masterServer = this.masterServerRepository.Get("64.15.129.183:15300", 5000);

            return masterServer;
        }
    }
}
