using System.Net;

namespace Zander.Domain.Remote {
	public interface IRemoteServerApiProvider {
		IRemoteServerApi GetInstance(IPEndPoint endpoint, int timeout);
	}
}
