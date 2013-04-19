using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly IRemoteServerApi serverApi;

		public virtual long Challenge {
			get { return 5660028L; }
		}

		public virtual short ProtocolVersion {
			get { return 2; }
		}

		public ZandronumMasterServerRepository(IRemoteServerApi serverApi) {
			this.serverApi = serverApi;
		}

		public IMasterServer Get(string address) {
			IMasterServer masterServer = new ZandronumMasterServer(address);

			return masterServer;
		}
	}
}
