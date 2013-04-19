using System;
using System.Collections.Generic;
using System.Net;
using Zander.Domain.Entities;

namespace Zander.Domain.Remote {
	public interface IRemoteServerApi : IDisposable {
		ChallengeResponse ChallengeMasterServer(ChallengeRequest request);
		IEnumerable<IPEndPoint> GetServerList(string masterServerAddress, long challenge, short protocolVersion);
		Server GetServerInfo(IPEndPoint serverEndpoint);
	}
}
