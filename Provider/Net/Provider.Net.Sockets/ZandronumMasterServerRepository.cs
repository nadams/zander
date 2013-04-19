using System.Collections.Generic;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ZandronumMasterServerRepository : IMasterServerRepository {
		private readonly IRemoteServerApi serverApi;

		public virtual int Challenge {
			get { return 5660028; }
		}

		public virtual short ProtocolVersion {
			get { return 2; }
		}

		public ZandronumMasterServerRepository(IRemoteServerApi serverApi) {
			this.serverApi = serverApi;
		}

		public IMasterServer Get(string address) {
			IMasterServer masterServer = new ZandronumMasterServer(address);

			var response = this.ChallengeMaster();
			var endpoints = this.GetServerEndpoints(response.ServerBlock);

			return masterServer;
		}

		private MasterChallengeResponse ChallengeMaster() {
			var request = new MasterChallengeRequest {
				Challenge = this.Challenge,
				ProtocolVersion = this.ProtocolVersion
			};

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

			var endpoints = new List<IPEndPoint>();

			return endpoints;
		}
	}
}
