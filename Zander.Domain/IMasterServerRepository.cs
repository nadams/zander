using Zander.Domain.Entities;

namespace Zander.Domain {
	public interface IMasterServerRepository {
		int MasterChallenge { get; }
		short ProtocolVersion { get; }
		IMasterServer Get(string address, int timeout);
	}
}
