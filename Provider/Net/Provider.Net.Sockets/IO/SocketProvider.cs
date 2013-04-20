using System.Net.Sockets;

namespace Zander.Provider.Net.Sockets.IO {
	public class SocketProvider : ISocketProvider {
		public int ReceiveTimeout { get; set; }

		public ISocket GetSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocol) {
			var socket = new Socket(addressFamily, socketType, protocol);

			return new SocketWrapper(socket);
		}
	}
}
