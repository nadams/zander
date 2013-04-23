using System.Net;

namespace Zander.Domain.Remote {
	public class ServerRequest {
		private readonly IPEndPoint endpoint;
		private readonly int query;
		private readonly int challenge;
		private readonly int tickCount;
		private readonly int timeout;

		public IPEndPoint Endpoint {
			get { return this.endpoint; }
		}

		public int Query {
			get { return this.query; }
		}

		public int Challenge {
			get { return this.challenge; }
		}

		public int TickCount {
			get { return this.tickCount; }
		}

		public int Timeout {
			get { return this.timeout; }
		}

		public ServerRequest(IPEndPoint endpoint, int timeout, int query, int challenge, int tickCount) {
			this.endpoint = endpoint;
			this.query = query;
			this.challenge = challenge;
			this.tickCount = tickCount;
			this.timeout = timeout;
		}
	}
}
