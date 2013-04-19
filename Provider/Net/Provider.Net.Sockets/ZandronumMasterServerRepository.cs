using Zander.Domain;
using Zander.Domain.Entities;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly ISocketApi sockets;

		public virtual long Challenge {
			get { return 5660028L; }
		}

		public virtual short ProtocolVersion {
			get { return 2; }
		}

		public ZandronumMasterServerRepository(ISocketApi socketApi) {
			this.sockets = socketApi;
		}

		public IMasterServer Get(string address) {
			IMasterServer masterServer = new ZandronumMasterServer(address);

			return masterServer;
		}
	}
}
