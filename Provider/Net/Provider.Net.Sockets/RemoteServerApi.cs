using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApi : IRemoteServerApi {

		private readonly string address;
		private readonly int timeout;

		public RemoteServerApi(string address, int timeout) {
			this.address = address;
			this.timeout = timeout;
		}

		public MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request) {

			using(var sendStream = new BinaryWriter(new MemoryStream())) {
				sendStream.Write(request.Challenge);
				sendStream.Write(request.ProtocolVersion);

			}

			return null;
		}

		public IEnumerable<ServerListResponse> GetServerList() {
			throw new NotImplementedException();
		}

		public Domain.Entities.Server GetServerInfo(IPEndPoint serverEndpoint) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}
	}
}
