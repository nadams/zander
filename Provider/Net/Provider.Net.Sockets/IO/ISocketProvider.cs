using System.Net.Sockets;

namespace Zander.Provider.Net.Sockets.IO {
	public interface ISocketProvider {
		ISocket GetSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocol);
	}
}
