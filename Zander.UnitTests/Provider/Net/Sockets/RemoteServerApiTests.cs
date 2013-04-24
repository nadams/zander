using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class RemoteServerApiTests {

		private readonly Encoding encoding;

		public RemoteServerApiTests() {
			this.encoding = Encoding.ASCII;
		}

		[TestMethod]
		public void ChallengeMasterServer_SendIncorrectProtcolVersion_IncorrectProtocolResponseReceived() {
			var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.ObsoleteProtocol);
			var socket = this.GetSocket(masterResponse);
			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 1500, 5);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.ObsoleteProtocol, response.Status);
		}

		[TestMethod]
		public void ChallengeMasterServer_UserHasBeenBanned_BannedResponseReceived() {
			var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.Banned);

			var socket = this.GetSocket(masterResponse);
			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 1500, 5);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.Banned, response.Status);
		}

		[TestMethod]
		public void ChallengeMasterServer_UserHasBeenIgnored_DeniedResponseReceived() {
			var masterResponse = BitConverter.GetBytes((int)MasterChallengeValues.Denied);
			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 1500, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(MasterChallengeValues.Denied, response.Status);
		}

		[TestMethod]
		public void ChallengeMasterServer_ZeroServersAvailable_EmptyListOfEndpointsReturned() {
			var masterResponse =
				BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
				Concat(new byte[] { 0 }).
				Concat(BitConverter.GetBytes((int)MasterChallengeValues.ServerBlock)).
				Concat(new byte[] { 0, 0 }).
				Concat(BitConverter.GetBytes((byte)MasterChallengeValues.EndOfServerList)).
				ToArray();

			var socket = this.GetSocket(masterResponse);
			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 0, 0);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(0, response.Servers.Count());
		}

		[TestMethod]
		public void ChallengeMasterServer_OnePageOfServersAvailable_OneServerReturned() {
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

			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 0, 0);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(1, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.First().ToString());
		}

		[TestMethod]
		public void ChallengeMasterServer_TwoPagesOfServersAvailable_TwoServersReturned() {
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

			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, (int)ChallengeValues.MasterChallenge, (short)ChallengeValues.MasterProtocol);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(2, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.ElementAt(0).ToString());
			Assert.AreEqual("10.0.0.2:10667", response.Servers.ElementAt(1).ToString());
		}

		[TestMethod]
		public void GetServerInfo_RequestBooleanValue_ByteconvertedToBoolean() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.GameType).
				Concat(new byte[] { (byte)GameMode.Deathmatch, 0, 1 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.GameType, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));

			var response = api.GetServerInfo(request);

			Assert.AreEqual(false, response.IsInstagib);
			Assert.AreEqual(true, response.IsBuckshot);
		}

		[TestMethod]
		public void GetServerInfo_RequestPWads_PWadsLoaded() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.PWads).
				Concat(new byte[] { 3 }).
				Concat(this.encoding.GetBytes("zdctfmp.wad\0")).
				Concat(this.encoding.GetBytes("zdctfmp2.wad\0")).
				Concat(this.encoding.GetBytes("zdctfmp3-.wad\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.PWads, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));

			var response = api.GetServerInfo(request);

			Assert.AreEqual(3, response.PWadsLoaded);
			Assert.AreEqual(3, response.PWads.Count());
			Assert.AreEqual("zdctfmp.wad", response.PWads.ElementAt(0));
			Assert.AreEqual("zdctfmp2.wad", response.PWads.ElementAt(1));
			Assert.AreEqual("zdctfmp3-.wad", response.PWads.ElementAt(2));
		}

		[TestMethod]
		public void Get_ServerName_ServerNameReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.Name).
				Concat(this.encoding.GetBytes("the best server in the world\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.Name, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("the best server in the world", response.Name);
		}

		[TestMethod]
		public void Get_Url_UrlReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.Url).
				Concat(this.encoding.GetBytes("http://the.wads.com/wads/\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.Url, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("http://the.wads.com/wads/", response.Url);
		}

		[TestMethod]
		public void Get_Email_EmailReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.Email).
				Concat(this.encoding.GetBytes("admin@server.com\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.Email, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("admin@server.com", response.Email);
		}

		private ISocket GetServerSocket(byte[] data) {
			var headerInformation = 
				BitConverter.GetBytes((int)ServerChallengeValues.BeginningOfData).
				Concat(BitConverter.GetBytes(5)).
				Concat(this.encoding.GetBytes("version 1.0\0"));

			var sendData = headerInformation.Concat(data).ToArray();

			return this.GetSocket(sendData);
		}

		private ISocket GetSocket(byte[] data) {
			var socketMock = new Mock<ISocket>();
			socketMock.Setup(
				x => x.ReceiveFrom(
					It.IsAny<byte[]>(),
					It.Is<SocketFlags>(y => y == SocketFlags.None),
					It.Is<IPEndPoint>(y => y.ToString() == "10.0.0.1:15300"))
				).Returns((byte[] b, SocketFlags flags, IPEndPoint endpoint) => this.EncodeData(data, b));

			return socketMock.Object;
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
