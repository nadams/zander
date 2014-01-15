using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Zander.Domain;
using Zander.Domain.Entities;

namespace Zander.Provider.Net.Sockets.Fake {
    public class FakeMasterServerRepository : IMasterServerRepository {
        private readonly Random random;

        public int MasterChallenge {
            get { return 0; }
        }

        public short ProtocolVersion {
            get { return 0; }
        }

        public FakeMasterServerRepository() {
            this.random = new Random();
        }

        public IMasterServer Get(string address, int timeout) {
            var endpoints = new List<IPEndPoint>();
            for(int i = this.random.Next(300); i < this.random.Next(300, 500); i++) {
                var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), i + 100);
                endpoints.Add(endpoint);
            }

            Thread.Sleep(random.Next(500, timeout));

            return new MasterServer(address, endpoints);
        }
    }
}
