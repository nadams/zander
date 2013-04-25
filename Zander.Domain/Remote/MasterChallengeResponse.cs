using System.Collections.Generic;
using System.Net;

namespace Zander.Domain.Remote {
	public class MasterChallengeResponse {
		public MasterChallengeValues Status { get; set; }
		public MasterChallengeValues ServerBlock { get; set; }
		public IEnumerable<ServerListResponse> Servers { get; set; }
		public byte PacketNumber { get; set; }

		public MasterChallengeResponse() {
			this.Servers = new List<ServerListResponse>();
			this.Status = MasterChallengeValues.Unknown;
			this.ServerBlock = MasterChallengeValues.Unknown;
		}
	}
}
