using System.Net;
using Zander.Domain.Entities;

namespace Zander.Domain {
	public interface IServerRepository {
		int ServerChallenge { get; }
		Server Get(IPEndPoint endpoint);
	}
}
