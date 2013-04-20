using System;
using System.Net;
using System.Net.Sockets;

namespace Zander.Provider.Net.Sockets.IO {
	public interface ISocket : IDisposable {
		int ReceiveTimeout { get; set; }
		int SendTo(byte[] data, SocketFlags flags, IPEndPoint address);
		int ReceiveFrom(byte[] buffer, SocketFlags flags, IPEndPoint address);
	}
}
