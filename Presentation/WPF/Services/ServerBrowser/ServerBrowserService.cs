using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public class ServerBrowserService : IServerBrowserService { 
        private const int RefreshRestTime = 15;

        public event ServersCollectionChangedEventHandler ServersChanged;
        public event TotalServersUpdatedEventHandler TotalServersUpdated;
        public event DoneQueryingServersEventHandler DoneQueryingServers;

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
                return this.servers.Values.AsEnumerable();
            }
        }

        public ServerBrowserService(IServerRepository serverRepo, IMasterServerRepository masterRepo) {
            this.serverRepository = serverRepo;
            this.masterServerRepository = masterRepo;
            this.lastQueriedTime = DateTime.MinValue;

            this.servers = new ConcurrentDictionary<string, Server>();
        }

        public void RefreshServer(IPEndPoint endPoint) {
            Task.Factory.StartNew(() => {
                var serverToModify = this.servers[endPoint.ToString()];

                var updatedInformation = this.GetServer(endPoint.ToString());

                if(updatedInformation != null) {
                    serverToModify.CopyData(updatedInformation);
                }

                this.HandleServersChange(ServersCollectionChangedActions.Update, serverToModify);
            });
        }

        public void QueryAllServers() {
            if(this.CanRefresh) {
                this.servers.Clear();

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

                        if(entity != null) {
                            this.AddServer(entity);
                        }
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
            this.servers.Add(server.IPEndPoint.ToString(), server);

            this.HandleServersChange(ServersCollectionChangedActions.Add, server);
        }

        public void RemoveServer(IPEndPoint endPoint) {
            string address = endPoint.ToString();
            Server server = this.servers[address];
            this.servers.Remove(address);

            this.HandleServersChange(ServersCollectionChangedActions.Remove, server);
        }

        private IMasterServer GetMasterServer() {
            var hostname = "master.zandronum.com:15300";

            var masterServer = this.masterServerRepository.Get(hostname, 5000);

            return masterServer;
        }

        private string GetAddress(IPAddress address, int port) {
            return address.ToString() + ":" + port;
        }

        private Server GetServer(string address) {
            Server entity = null;

            try {
                entity = this.serverRepository.Get(address, 1000, ServerQueryValues.AllData);
            } catch { }

            return entity;
        }

        private void HandleServersChange(ServersCollectionChangedActions action, Server server) {
            if(this.ServersChanged != null) {
                var args = new ServersCollectionChangedEventArgs(action, server);

                this.ServersChanged(this, args);
            }
        }
    }
}
