using System.Net;
using Zander.Domain.Entities;

namespace Zander.Domain.Remote {
	public interface IRemoteServerApi {
		MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request);
		Server GetServerInfo(IPEndPoint serverEndpoint);
	}
}
