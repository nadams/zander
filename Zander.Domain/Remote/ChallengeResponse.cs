
namespace Zander.Domain.Remote {
	public class ChallengeResponse {
		public int Status { get; set; }
		public byte PacketNumber { get; set; }
		public int ServerBlock { get; set; }
	}
}
