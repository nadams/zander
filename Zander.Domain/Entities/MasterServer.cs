using System.Collections.Generic;
using System.Net;

namespace Zander.Domain.Entities {
	public class MasterServer : IMasterServer {
		private readonly IEnumerable<IPEndPoint> servers;
		private readonly string address;

		public IEnumerable<IPEndPoint> Servers {
			get { return this.servers; }
		}

		public string Address {
			get { return this.address; }
		}

		public MasterServer(string address, IEnumerable<IPEndPoint> servers) {
			this.address = address;
			this.servers = servers;
		}
	}
}
