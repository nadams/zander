namespace Zander.Domain.Remote {
	public class MasterChallengeResponse {
		private readonly MasterChallengeValues status;
		private readonly MasterChallengeValues serverBlock;
		private readonly byte packetNumber;

		public MasterChallengeValues Status {
			get { return this.status; }
		}

		public MasterChallengeValues ServerBlock {
			get { return this.serverBlock; }
		}

		public byte PacketNumber {
			get { return this.packetNumber; }
		}

		public MasterChallengeResponse(MasterChallengeValues status, MasterChallengeValues serverBlock, byte packetNumber) {
			this.status = status;
			this.serverBlock = serverBlock;
			this.packetNumber = packetNumber;
		}
	}
}
