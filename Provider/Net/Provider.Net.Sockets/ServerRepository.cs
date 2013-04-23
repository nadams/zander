using System;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ServerRepository : IServerRepository {

		private readonly IRemoteServerApiProvider serverApiProvider;

		public int ServerChallenge {
			get { return (int)ChallengeValues.ServerChallenge; }
		}

		public ServerRepository(IRemoteServerApiProvider serverApiProvider) {
			this.serverApiProvider = serverApiProvider;
		}

		public Server Get(string address, int timeout, ServerQueryValues query) {
			throw new NotImplementedException();
		}
	}
}
