using System;
using System.Collections.Generic;
using System.Net;
using Zander.Domain.Entities;

namespace Zander.Domain.Remote {
	public interface IRemoteServerApi : IDisposable {
		MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request);
		IEnumerable<IPEndPoint> GetServerList(string masterServerAddress, int challenge, short protocolVersion);
		Server GetServerInfo(IPEndPoint serverEndpoint);
	}
}
