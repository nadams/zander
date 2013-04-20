using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;
using Zander.Provider.Net.Sockets.IO;
using System.Linq;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class RemoteServerApiTests {

		[TestMethod]
		public void ChallengeMasterServer_SendIncorrectProtcolVersion_IncorrectProtocolResponseReceived() {
			var socketMock = new Mock<ISocket>();
			socketMock.Setup(
				x => x.ReceiveFrom(
					It.IsAny<byte[]>(),
					It.Is<SocketFlags>(y => y == SocketFlags.None),
					It.Is<IPEndPoint>(y => y.ToString() == "10.0.0.1:15300"))
				).Returns((byte[] b, SocketFlags flags, IPEndPoint endpoint) => { 
					var stream = new MemoryStream();
					var writer = new BinaryWriter(stream);
					writer.Write((int)MasterChallengeValues.ObsoleteProtocol);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();
					
					return data.Length; 
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.ObsoleteProtocol, response.Status);
		}

		[TestMethod]
		public void ChallengeMasterServer_ZeroServersAvailable_EmptyListOfEndpointsReturned() {
			var socketMock = new Mock<ISocket>();
			socketMock.Setup(
				x => x.ReceiveFrom(
					It.IsAny<byte[]>(),
					It.Is<SocketFlags>(y => y == SocketFlags.None),
					It.Is<IPEndPoint>(y => y.ToString() == "10.0.0.1:15300"))
				).Returns((byte[] b, SocketFlags flags, IPEndPoint endpoint) => {
					var stream = new MemoryStream();
					var writer = new BinaryWriter(stream);
					writer.Write((int)MasterChallengeValues.BeginningOfServerList);
					writer.Write((byte)0);
					writer.Write((int)MasterChallengeValues.ServerBlock);
					writer.Write((byte)0);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();

					return data.Length;
				});

			var request = new MasterChallengeRequest(0, 0);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(0, response.Servers.Count());
		}

		private class FakeSocketProvider : ISocketProvider {
			private readonly ISocket socket;

			public FakeSocketProvider(ISocket socket) {
				this.socket = socket;
			}

			public ISocket GetSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocol) {
				return this.socket;
			}
		}

		private class EmptyNetworkCompressor : INetworkCompressor {
			public byte[] Encode(byte[] decodedData) {
				return decodedData;
			}

			public byte[] Decode(byte[] encodedData) {
				return encodedData;
			}
		}
	}
}
