namespace Zander.Domain.Entities {
	public class ZandronumMasterServer : MasterServer {
		public override short ProtocolVersion {
			get { return 2; }
		}

		public override int Challenge {
			get { return 5660028; }
		}

		public ZandronumMasterServer(string address) : base(address) { }
	}
}
