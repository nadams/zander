using System.Net;
namespace Zander.Domain.Remote {
	public class MasterChallengeRequest {
		private readonly int challenge;
		private readonly short protocolVersion;
		private readonly IPEndPoint endpoint;
		private readonly int timeout;

		public int Challenge {
			get { return this.challenge; }
		}

		public short ProtocolVersion {
			get { return this.protocolVersion; }
		}

		public IPEndPoint Endpoint {
			get { return this.endpoint; }
		}

		public int Timeout {
			get { return this.timeout; }
		}

		public MasterChallengeRequest(IPEndPoint endpoint, int timeout, int challenge, short protocolVersion) {
			this.challenge = challenge;
			this.protocolVersion = protocolVersion;
			this.endpoint = endpoint;
			this.timeout = timeout;
		}
	}
}
