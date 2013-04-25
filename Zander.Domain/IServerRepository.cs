using System.Net;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Domain {
	public interface IServerRepository {
		int ServerChallenge { get; }

		Server Get(string address, int timeout, ServerQueryValues query);
	}
}
