using System.Net;

namespace Zander.Domain.Remote {
	public class ServerRequest {
		private readonly IPEndPoint address;
		private readonly uint query;
		private readonly int challenge;

		public IPEndPoint Address {
			get { return this.address; }
		}

		public uint Query {
			get { return this.query; }
		}

		public int Challenge {
			get { return this.challenge; }
		}

		public ServerRequest(IPEndPoint address, uint query, int challenge) {
			this.address = address;
			this.query = query;
			this.challenge = challenge;
		}
	}
}
