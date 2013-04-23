using System.Diagnostics.CodeAnalysis;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {

	[ExcludeFromCodeCoverage]
	public class RemoteServerApiProvider : IRemoteServerApiProvider {
		public IRemoteServerApi GetInstance() {
			return new RemoteServerApi(new HuffmanWrapper(), new SocketProvider());
		}
	}
}
