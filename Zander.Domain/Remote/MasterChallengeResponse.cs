
namespace Zander.Domain.Remote {
	public class MasterChallengeResponse {
		public MasterChallengeStatus Status { get; set; }
		public byte PacketNumber { get; set; }
		public int ServerBlock { get; set; }
	}
}
