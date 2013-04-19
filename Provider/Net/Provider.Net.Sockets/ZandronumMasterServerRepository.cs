using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly IRemoteServerApi serverApi;

		public int Challenge {
			get { return 5660028; }
		}

		public short ProtocolVersion {
			get { return 2; }
		}

		public ZandronumMasterServerRepository(IRemoteServerApi serverApi) {
			this.serverApi = serverApi;
		}

		public IMasterServer Get(string address) {
			var servers = new List<IPEndPoint>();
			IMasterServer masterServer = new ZandronumMasterServer(address, servers);

			var response = this.ChallengeMaster();
			var endpoints = this.GetServerEndpoints(response.ServerBlock);

			servers.AddRange(endpoints);

			return masterServer;
		}

		private MasterChallengeResponse ChallengeMaster() {
			var request = new MasterChallengeRequest(this.Challenge, this.ProtocolVersion);

			var response = this.serverApi.ChallengeMasterServer(request);

			switch(response.Status) {
				case MasterChallengeValues.Banned:
					throw new ClientBannedException();

				case MasterChallengeValues.Denied:
					throw new ClientIgnoredException();

				case MasterChallengeValues.ObsoleteProtocol:
					throw new ObsoleteProtocolException();

				case MasterChallengeValues.BeginningOfServerList:
					break;

				default:
					throw new UnknownMasterServerResponseException();
			}

			return response;
		}

		private IEnumerable<IPEndPoint> GetServerEndpoints(MasterChallengeValues status) {
			if(status != MasterChallengeValues.ServerBlock) {
				throw new UnknownMasterServerResponseException();
			}

			var responses = this.serverApi.GetServerList();

			var endpoints = responses.Select(x => {
				var ipAddress = new IPAddress(new byte[] { x.Octet1, x.Octet2, x.Octet3, x.Octet4 });

				return new IPEndPoint(ipAddress, x.Port);
			});

			return endpoints;
		}
	}
}
