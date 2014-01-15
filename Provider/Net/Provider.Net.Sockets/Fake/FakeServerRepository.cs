using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets.Fake {
    public class FakeServerRepository : IServerRepository {
        private readonly Random random;

        public int ServerChallenge {
            get { return 0; }
        }

        public FakeServerRepository() {
            this.random = new Random();
        }

        public Server Get(string address, int timeout, ServerQueryValues query) {
            var endpoint = this.GetIPEndPoint(address);
            var server = new Server(endpoint);
            this.PopluateServerWithFakeData(server);
            
            Thread.Sleep(random.Next(250, timeout));

            return server;
        }

        private void PopluateServerWithFakeData(Server s) {
            s.CurrentMap = "MAP01";
            s.DisplayName = this.RandomString(random.Next(4, 64));
            s.MaxPlayers = random.Next(4, 64);
            s.Players = Enumerable.Repeat(new Player(), random.Next(s.MaxPlayers));
            s.PWads = Enumerable.Repeat(new Wad { Name = "something.wad" }, random.Next(5));
            s.IWad = new Wad { Name = "DOOM2.WAD" };
        }

        private string RandomString(int length) {
            var sb = new StringBuilder();

            char ch;
            for(int i = 0; i < length; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * this.random.NextDouble() + 65)));
                sb.Append(ch);
            }

            return sb.ToString();
        }

        private IPEndPoint GetIPEndPoint(string address) {
            var split = address.Split(':');

            return new IPEndPoint(IPAddress.Parse(split[0]), int.Parse(split[1]));
        }
    }
}
