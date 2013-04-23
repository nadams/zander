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
					var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.ObsoleteProtocol);

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
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
					var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.Banned);

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
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
					var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.Denied);

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest(1500, 5);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
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
					var masterResponse =
						BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
						Concat(new byte[] { 0 }).
						Concat(BitConverter.GetBytes((int)MasterChallengeValues.ServerBlock)).
						Concat(new byte[] { 0, 0 }).
						Concat(BitConverter.GetBytes((byte)MasterChallengeValues.EndOfServerList)).
						ToArray();

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest(0, 0);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
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
					var masterResponse =
						BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
						Concat(new byte[] { 0 }).
						Concat(BitConverter.GetBytes((int)MasterChallengeValues.ServerBlock)).
						Concat(new byte[] { 1 }).
						Concat(new byte[] { 10, 0, 0, 1 }).
						Concat(BitConverter.GetBytes((ushort)10666)).
						Concat(new byte[] { 0 }).
						Concat(new byte[] { (byte)MasterChallengeValues.EndOfServerList }).
						ToArray();

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest(0, 0);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
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
					var masterResponse =
						BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
						Concat(new byte[] { 0 }).
						Concat(BitConverter.GetBytes((int)MasterChallengeValues.ServerBlock)).
						Concat(new byte[] { 1 }).
						Concat(new byte[] { 10, 0, 0, 1 }).
						Concat(BitConverter.GetBytes((ushort)10666)).
						Concat(new byte[] { 0 }).
						Concat(new byte[] { (byte)MasterChallengeValues.EndOfCurrentList }).
						Concat(new byte[] { 1 }).
						Concat(new byte[] { 10, 0, 0, 2 }).
						Concat(BitConverter.GetBytes((ushort)10667)).
						Concat(new byte[] { 0 }).
						Concat(new byte[] { (byte)MasterChallengeValues.EndOfServerList }).
						ToArray();

					return this.EncodeData(masterResponse, b);
				});

			var request = new MasterChallengeRequest((int)ChallengeValues.MasterChallenge, (short)ChallengeValues.MasterProtocol);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socketMock.Object), new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0);
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(2, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.ElementAt(0).ToString());
			Assert.AreEqual("10.0.0.2:10667", response.Servers.ElementAt(1).ToString());
		}

		private int EncodeData(byte[] rawData, byte[] output) {
			var encodedData = new byte[2048];
			var encodedLength = new EmptyCompressor().Encode(rawData, encodedData, rawData.Length);

			encodedData = encodedData.Take(encodedLength).ToArray();

			for(int i = 0; i < encodedData.Length; i++) {
				output[i] = encodedData[i];
			}

			return encodedData.Length;
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

		private class EmptyCompressor : INetworkCompressor {

			public int Encode(byte[] input, byte[] output, int length) {
				for(int i = 0; i < input.Length; i++) {
					output[i] = input[i];
				}

				return length;
			}

			public int Decode(byte[] input, byte[] output, int length) {
				for(int i = 0; i < input.Length; i++) {
					output[i] = input[i];
				}

				return length;
			}
		}
	}
}
