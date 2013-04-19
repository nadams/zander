using System.Collections.Generic;
using System.Net;

namespace Zander.Domain.Entities {
	public abstract class MasterServer : IMasterServer {
		public abstract short ProtocolVersion { get; }
		public abstract int Challenge { get; }

		private readonly IEnumerable<IPEndPoint> servers;
		private readonly string address;
		private readonly MasterServerStatus status;

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
