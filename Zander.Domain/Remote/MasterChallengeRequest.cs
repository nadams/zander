namespace Zander.Domain.Remote {
	public class MasterChallengeRequest {
		private readonly int challenge;
		private readonly short protocolVersion;

		public int Challenge {
			get { return this.challenge; }
		}

		public short ProtocolVersion {
			get { return this.protocolVersion; }
		}

		public MasterChallengeRequest(int challenge, short protocolVersion) {
			this.challenge = challenge;
			this.protocolVersion = protocolVersion;
		}
	}
}
