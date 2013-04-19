using System.Collections.Generic;
using System.Net;

namespace Zander.Domain.Entities {
	public class ZandronumMasterServer : MasterServer {
		public override short ProtocolVersion {
			get { return 2; }
		}

		public override int Challenge {
			get { return 5660028; }
		}

		public ZandronumMasterServer(string address, IEnumerable<IPEndPoint> servers) : base(address, servers) { }
	}
}
