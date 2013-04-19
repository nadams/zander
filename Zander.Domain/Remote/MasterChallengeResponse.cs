
namespace Zander.Domain.Remote {
	public class MasterChallengeResponse {
		public MasterChallengeValues Status { get; set; }
		public MasterChallengeValues ServerBlock { get; set; }
		public byte PacketNumber { get; set; }
	}
}
