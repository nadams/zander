using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
        private const int DefaultPort = 15300;

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

		public IMasterServer Get(string address, int timeout) {
			var servers = new List<IPEndPoint>();
			var masterServer = new MasterServer(address, servers);

			var split = address.Split(':');
            var endpoint = this.ResolveHostname(address);

			var serverApi = this.serverApiProvider.GetInstance();
			var response = this.ChallengeMaster(serverApi, endpoint, timeout);
			var endpoints = this.GetServerEndpoints(response);

			servers.AddRange(endpoints);

			return masterServer;
		}

        private IPEndPoint ResolveHostname(string address) {
            var parts = address.Split(':');
            IPAddress ipAddress;
            if(!IPAddress.TryParse(parts[0], out ipAddress)) {
                ipAddress = Dns.GetHostAddresses(parts[0]).First();
            }

            int port = DefaultPort;
            if(parts.Length > 1) {
                port = int.Parse(parts[1]);
            }

            return new IPEndPoint(ipAddress, port);
        }

		private MasterChallengeResponse ChallengeMaster(IRemoteServerApi api, IPEndPoint endpoint, int timeout) {
			var request = new MasterChallengeRequest(endpoint, timeout, this.MasterChallenge, this.ProtocolVersion);

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
