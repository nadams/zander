using System.Diagnostics.CodeAnalysis;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {

	[ExcludeFromCodeCoverage]
	public class RemoteServerApiProvider : IRemoteServerApiProvider {
		public IRemoteServerApi GetInstance(string address, int timeout) {
			return new RemoteServerApi(new HuffmanWrapper(), new SocketProvider(), address, timeout);
		}
	}
}
