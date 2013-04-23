using System;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ServerRepository : IServerRepository {

		private readonly IRemoteServerApiProvider serverApiProvider;

		public int ServerChallenge {
			get { return 199; }
		}

		public ServerRepository(IRemoteServerApiProvider serverApiProvider) {
			this.serverApiProvider = serverApiProvider;
		}

		public Server Get(IPEndPoint endpoint, ServerQueryValues query) {
			throw new NotImplementedException();
		}
	}
}
