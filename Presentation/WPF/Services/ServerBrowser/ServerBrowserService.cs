using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public class ServerBrowserService : IServerBrowserService { 
        private const int RefreshRestTime = 15;

        private readonly IEventAggregator eventAggregator;
        private readonly IServerRepository serverRepository;
        private readonly IMasterServerRepository masterServerRepository;

        private DateTime lastQueriedTime;
        private bool isQuerying;

        private bool CanRefresh {
            get {
                var refreshAtTime = DateTime.UtcNow.AddSeconds(RefreshRestTime);
                var duration = refreshAtTime - this.lastQueriedTime;

                bool canRefresh = duration.Duration().Seconds > RefreshRestTime;

                return !this.isQuerying && canRefresh;
            }
        }

        public ServerBrowserService(IEventAggregator eventAggregator, IServerRepository serverRepo, IMasterServerRepository masterRepo) {
            this.eventAggregator = eventAggregator;
            this.serverRepository = serverRepo;
            this.masterServerRepository = masterRepo;
            this.lastQueriedTime = DateTime.MinValue;
        }

        public Server RefreshServerAsync(Server server) {
            Task.Factory.StartNew(() => {
                //var entity = this.serverRepository.Get(selectedServer.Address, 1000, ServerQueryValues.AllData);

                //var mapper = new ServerEntityMapper();
                //var model = mapper.ModelFromEntity(entity);

                //mapper.CopyModel(selectedServer, model);
            });

            return null;
        }

        public void QueryAllServersAsync() {
            if(this.CanRefresh) {
                Task.Factory.StartNew(() => {
                    this.isQuerying = true;

                    var masterServer = this.GetMasterServer();

                    this.eventAggregator.GetEvent<TotalServersUpdatedEvent>().Publish(masterServer.Servers.Count());

                    Parallel.ForEach(masterServer.Servers, (server, status) => {
                        var address = this.GetAddress(server.Address, server.Port);

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

        private string GetAddress(IPAddress address, int port) {
            return address.ToString() + ":" + port;
        }
    }
}
