using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public class ServerBrowserService : IServerBrowserService { 
        private const int RefreshRestTime = 15;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event TotalServersUpdatedEventHandler TotalServersUpdated;
        public event DoneQueryingServersEventHandler DoneQueryingServers;

        private readonly object serversLock;
        private readonly IServerRepository serverRepository;
        private readonly IMasterServerRepository masterServerRepository;
        private readonly IDictionary<string, Server> servers;

        private DateTime lastQueriedTime;
        private bool isQuerying;

        public bool CanRefresh {
            get {
                var refreshAtTime = DateTime.UtcNow.AddSeconds(RefreshRestTime);
                var duration = refreshAtTime - this.lastQueriedTime;

                bool canRefresh = duration.Duration().Seconds > RefreshRestTime;

                return !this.isQuerying && canRefresh;
            }
        }

        public IEnumerable<Server> Servers {
            get {
                lock(this.serversLock) {
                    return this.servers.Values.AsEnumerable();
                }
            }
        }

        public ServerBrowserService(IServerRepository serverRepo, IMasterServerRepository masterRepo) {
            this.serversLock = new object();
            this.serverRepository = serverRepo;
            this.masterServerRepository = masterRepo;
            this.lastQueriedTime = DateTime.MinValue;

            this.servers = new Dictionary<string, Server>();
        }

        public void RefreshServer(Server server) {
            Task.Factory.StartNew(() => {
                var serverToModify = this.servers[server.IPEndPoint.ToString()];

                var updatedInformation = this.GetServer(server.IPEndPoint.ToString());

                server.CopyData(updatedInformation);

                if(this.CollectionChanged != null) {
                    var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, server);

                    this.CollectionChanged(this, args);
                }
            });
        }

        public void QueryAllServers() {
            if(this.CanRefresh) {
                Task.Factory.StartNew(() => {
                    this.isQuerying = true;

                    var masterServer = this.GetMasterServer();

                    if(this.TotalServersUpdated != null) {
                        var args = new TotalServersUpdatedEventArgs(masterServer.Servers.Count());

                        this.TotalServersUpdated(this, args);
                    }

                    Parallel.ForEach(masterServer.Servers, (server, status) => {
                        var address = this.GetAddress(server.Address, server.Port);

                        var entity = this.GetServer(address);

                        this.AddServer(entity);
                    });

                    if(this.DoneQueryingServers != null) {
                        this.DoneQueryingServers(this);
                    }

                    this.isQuerying = false;
                    this.lastQueriedTime = DateTime.UtcNow;
                });
            }
        }

        public void AddServer(Server server) {
            lock(this.serversLock) {
                this.servers.Add(server.IPEndPoint.ToString(), server);
            }

            if(this.CollectionChanged != null) {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, server);

                this.CollectionChanged(this, args);
            }
        }

        public void RemoveServer(Server server) {
            lock(this.serversLock) {
                this.servers.Remove(server.IPEndPoint.ToString());
            }

            if(this.CollectionChanged != null) {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, server);

                this.CollectionChanged(this, args);
            }
        }

        private IMasterServer GetMasterServer() {
            var masterServer = this.masterServerRepository.Get("64.15.129.183:15300", 5000);

            return masterServer;
        }

        private string GetAddress(IPAddress address, int port) {
            return address.ToString() + ":" + port;
        }

        private Server GetServer(string address) {
            Server entity = null;

            try {
                entity = this.serverRepository.Get(address, 1000, ServerQueryValues.AllData);

                this.AddServer(entity);

                return entity;
            } catch { }

            return entity;
        }
    }
}
