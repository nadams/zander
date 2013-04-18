using Zander.Domain.Entities;

namespace Zander.Domain {
	public interface IMasterServerRepository {
		IMasterServer Get(string address);
	}
}
