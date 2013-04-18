using Zander.Domain;
using Zander.Domain.Entities;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly ISocketApi sockets;

		public ZandronumMasterServerRepository(ISocketApi socketApi) {
			this.sockets = socketApi;
		}

		public IMasterServer Get(string address) {
			throw new System.NotImplementedException();
		}
	}
}
