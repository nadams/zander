namespace Zander.Domain.Entities {
	public class ZandronumMasterServer : MasterServer {
		public override short ProtocolVersion {
			get { return 2; }
		}

		public override long Challenge {
			get { return 5660028L; }
		}

		public ZandronumMasterServer(string address) : base(address) { }
	}
}
