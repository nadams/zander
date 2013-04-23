using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly IRemoteServerApiProvider serverApiProvider;

		public int MasterChallenge {
			get { return (int)ChallengeValues.MasterChallenge; }
		}

		public short ProtocolVersion {
			get { return (int)ChallengeValues.MasterProtocol; }
		}

		public ZandronumMasterServerRepository(IRemoteServerApiProvider serverApiProvider) {
			this.serverApiProvider = serverApiProvider;
		}

		public IMasterServer Get(string address) {
			var servers = new List<IPEndPoint>();
			var masterServer = new MasterServer(address, servers);

			var serverApi = this.serverApiProvider.GetInstance(address, 1000);
			var response = this.ChallengeMaster(serverApi);
			var endpoints = this.GetServerEndpoints(response);

			servers.AddRange(endpoints);

			return masterServer;
		}

		private MasterChallengeResponse ChallengeMaster(IRemoteServerApi api) {
			var request = new MasterChallengeRequest(this.MasterChallenge, this.ProtocolVersion);

			var response = api.ChallengeMasterServer(request);

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

		private IEnumerable<IPEndPoint> GetServerEndpoints(MasterChallengeResponse response) {
			if(response.ServerBlock != MasterChallengeValues.ServerBlock) {
				throw new UnknownMasterServerResponseException();
			}

			var endpoints = response.Servers.Select(x => {
				var ipAddress = new IPAddress(new byte[] { x.Octet1, x.Octet2, x.Octet3, x.Octet4 });

				return new IPEndPoint(ipAddress, x.Port);
			});

			return endpoints;
		}
	}
}
