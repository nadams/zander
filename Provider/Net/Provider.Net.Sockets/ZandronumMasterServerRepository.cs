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

			return masterServer;
		}

		private MasterChallengeResponse ChallengeMaster() {
			var request = new MasterChallengeRequest {
				Challenge = this.Challenge,
				ProtocolVersion = this.ProtocolVersion
			};

			var response = this.serverApi.ChallengeMasterServer(request);
			switch(response.Status) {
				case MasterChallengeStatus.Banned:
					throw new ClientBannedException();

				case MasterChallengeStatus.Denied:
					break;

				case MasterChallengeStatus.ObsoleteProtocol:
					throw new ObsoleteProtocolException();

				case MasterChallengeStatus.BeginningOfServerList:
					break;

				default:
					break;
			}

			return response;
		}
	}
}
