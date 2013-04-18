using System.Collections.Generic;

namespace Zander.Domain.Entities {
	public abstract class MasterServer : IMasterServer {
		public abstract short ProtocolVersion { get; }
		public abstract long Challenge { get; }

		private readonly ICollection<Server> servers;
		private readonly string address;

		public IEnumerable<Server> Servers {
			get { return this.servers; }
		}

		public string Address {
			get { return this.address; }
		}

		public MasterServerStatus Status { get; set; }

		public MasterServer(string address) {
			this.address = address;
			this.servers = new List<Server>();
		}
	}
}
