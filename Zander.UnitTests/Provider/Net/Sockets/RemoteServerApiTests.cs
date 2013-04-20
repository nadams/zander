using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;
using Zander.Provider.Net.Sockets.IO;

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
		public void ChallengeMasterServer_UserHasBeenBanned_BannedResponseReceived() {
			var socketMock = new Mock<ISocket>();
			socketMock.Setup(
				x => x.ReceiveFrom(
					It.IsAny<byte[]>(),
					It.Is<SocketFlags>(y => y == SocketFlags.None),
					It.Is<IPEndPoint>(y => y.ToString() == "10.0.0.1:15300"))
				).Returns((byte[] b, SocketFlags flags, IPEndPoint endpoint) => {
					var stream = new MemoryStream();
					var writer = new BinaryWriter(stream);
					writer.Write((int)MasterChallengeValues.Banned);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();

					return data.Length;
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.Banned, response.Status);
		}

		[TestMethod]
		public void ChallengeMasterServer_UserHasBeenIgnored_DeniedResponseReceived() {
			var socketMock = new Mock<ISocket>();
			socketMock.Setup(
				x => x.ReceiveFrom(
					It.IsAny<byte[]>(),
					It.Is<SocketFlags>(y => y == SocketFlags.None),
					It.Is<IPEndPoint>(y => y.ToString() == "10.0.0.1:15300"))
				).Returns((byte[] b, SocketFlags flags, IPEndPoint endpoint) => {
					var stream = new MemoryStream();
					var writer = new BinaryWriter(stream);
					writer.Write((int)MasterChallengeValues.Denied);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();

					return data.Length;
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.Denied, response.Status);
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

		[TestMethod]
		public void ChallengeMasterServer_OnePageOfServersAvailable_OneServerReturned() {
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
					writer.Write((byte)1);
					writer.Write(new byte[] { 10, 0, 0, 1 });
					writer.Write((ushort)10666);
					writer.Write((byte)0);
					writer.Write((byte)MasterChallengeValues.EndOfServerList);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();

					return data.Length;
				});

			var request = new MasterChallengeRequest(0, 0);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(1, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.First().ToString());
		}

		[TestMethod]
		public void ChallengeMasterServer_TwoPagesOfServersAvailable_TwoServersReturned() {
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
					writer.Write((byte)1);
					writer.Write(new byte[] { 10, 0, 0, 1 });
					writer.Write((ushort)10666);
					writer.Write((byte)0);
					writer.Write((byte)MasterChallengeValues.EndOfCurrentList);
					writer.Write((byte)1);
					writer.Write(new byte[] { 10, 0, 0, 2 });
					writer.Write((ushort)10667);
					writer.Write((byte)0);
					writer.Write((byte)MasterChallengeValues.EndOfServerList);
					writer.Flush();

					var data = stream.GetBuffer();
					Buffer.BlockCopy(data, 0, b, 0, data.Length);

					writer.Close();

					return data.Length;
				});

			var request = new MasterChallengeRequest(0, 0);

			var api = new RemoteServerApi(new EmptyNetworkCompressor(), new FakeSocketProvider(socketMock.Object), "10.0.0.1:15300", 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(2, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.ElementAt(0).ToString());
			Assert.AreEqual("10.0.0.2:10667", response.Servers.ElementAt(1).ToString());
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
