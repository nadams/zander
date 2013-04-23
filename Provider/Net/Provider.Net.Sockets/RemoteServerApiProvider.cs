using System.Diagnostics.CodeAnalysis;
using System.Net;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {

	[ExcludeFromCodeCoverage]
	public class RemoteServerApiProvider : IRemoteServerApiProvider {
		public IRemoteServerApi GetInstance(IPEndPoint endpoint, int timeout) {
			return new RemoteServerApi(new HuffmanWrapper(), new SocketProvider(), endpoint, timeout);
		}
	}
}
