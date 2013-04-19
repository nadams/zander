namespace Zander.Domain.Remote {
	public class ServerListResponse {
		private readonly byte octet1;
		private readonly byte octet2;
		private readonly byte octet3;
		private readonly byte octet4;
		private readonly short port;

		public byte Octet1 {
			get { return this.octet1; }
		}

		public byte Octet2 {
			get { return this.octet2; }
		}

		public byte Octet3 {
			get { return this.octet3; }
		}

		public byte Octet4 {
			get { return this.octet4; }
		}

		public short Port {
			get { return this.port; }
		}

		public ServerListResponse(byte octet1, byte octet2, byte octet3, byte octet4, short port) {
			this.octet1 = octet1;
			this.octet2 = octet2;
			this.octet3 = octet3;
			this.octet4 = octet4;
			this.port = port;
		}
	}
}
