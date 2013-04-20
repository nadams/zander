using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApiProvider : IRemoteServerApiProvider {
		public IRemoteServerApi GetInstance(string address, int timeout) {
			return new RemoteServerApi(new HuffmanWrapper(), address, timeout);
		}
	}
}
