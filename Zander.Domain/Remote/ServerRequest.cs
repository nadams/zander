using System.Net;

namespace Zander.Domain.Remote {
	public class ServerRequest {
		private readonly IPEndPoint address;
		private readonly uint query;
		private readonly int challenge;
		private readonly int tickCount;

		public IPEndPoint Address {
			get { return this.address; }
		}

		public uint Query {
			get { return this.query; }
		}

		public int Challenge {
			get { return this.challenge; }
		}

		public int TickCount {
			get { return this.tickCount; }
		}

		public ServerRequest(IPEndPoint address, uint query, int challenge, int tickCount) {
			this.address = address;
			this.query = query;
			this.challenge = challenge;
			this.tickCount = tickCount;
		}
	}
}
