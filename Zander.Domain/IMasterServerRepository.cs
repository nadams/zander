using Zander.Domain.Entities;

namespace Zander.Domain {
	public interface IMasterServerRepository {
		int Challenge { get; }
		short ProtocolVersion { get; }
		IMasterServer Get(string address);
	}
}
