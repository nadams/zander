using System;
using System.Net;
using Zander.Domain.Entities;

namespace Zander.Domain.Remote {
	public interface IRemoteServerApi : IDisposable {
		MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request);
		Server GetServerInfo(IPEndPoint serverEndpoint);
	}
}
