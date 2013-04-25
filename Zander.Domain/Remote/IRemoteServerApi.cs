namespace Zander.Domain.Remote {
	public interface IRemoteServerApi {
		MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request);
		ServerResponse GetServerInfo(ServerRequest request);
	}
}
