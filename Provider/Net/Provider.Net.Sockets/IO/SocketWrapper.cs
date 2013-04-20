using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Zander.Provider.Net.Sockets.IO {

	[ExcludeFromCodeCoverage]
	public class SocketWrapper : ISocket {
		private readonly Socket socket;

		public int ReceiveTimeout {
			get {
				return this.socket.ReceiveTimeout;
			}

			set {
				this.socket.ReceiveTimeout = value;
			}
		}

		public SocketWrapper(Socket socket) {
			this.socket = socket;
		}

		public int SendTo(byte[] data, SocketFlags flags, IPEndPoint address) {
			return this.socket.SendTo(data, flags, address);
		}

		public int ReceiveFrom(byte[] buffer, SocketFlags flags, IPEndPoint address) {
			var endpoint = (EndPoint)address;

			return this.socket.ReceiveFrom(buffer, flags, ref endpoint);
		}

		protected virtual void Dispose(bool disposing) {
			this.socket.Dispose();
		}

		public void Dispose() {
			this.Dispose(true);

			GC.SuppressFinalize(this);
		}
	}
}
