using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApiProvider : IRemoteServerApiProvider {
		public IRemoteServerApi GetInstance(string address, int timeout) {
			return new RemoteServerApi(address, timeout);
		}
	}
}
