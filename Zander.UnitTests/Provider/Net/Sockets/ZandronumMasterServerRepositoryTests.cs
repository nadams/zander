using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class ZandronumMasterServerRepositoryTests {

		private readonly MasterChallengeResponse goodMasterChallenge;

		public ZandronumMasterServerRepositoryTests() {
			this.goodMasterChallenge = new MasterChallengeResponse(MasterChallengeValues.BeginningOfServerList, MasterChallengeValues.ServerBlock, 0);
		}

		[TestMethod]
		public void Get_Address_SameAsPassedIn() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(this.goodMasterChallenge);

			var repo = new ZandronumMasterServerRepository(apiMock.Object);
			var address = "master.server:15000";

			var masterServer = repo.Get("master.server:15000");

			Assert.AreEqual(address, masterServer.Address);
		}

		[TestMethod]
		[ExpectedException(typeof(ObsoleteProtocolException))]
		public void Get_ClientUsingObsoleteProtocol_ObsoleteProtocolExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse(MasterChallengeValues.ObsoleteProtocol, 0, 0));

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientBannedException))]
		public void Get_ClientHasBeenBanned_ClientBannedExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse(MasterChallengeValues.Banned, 0, 0));

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientIgnoredException))]
		public void Get_ClientHasMadeTooManyRequests_ClientIgnoredExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse(MasterChallengeValues.Denied, 0, 0));

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownMasterServerResponseException))]
		public void Get_ClientGetUnknownStatus_UnknownMasterServerResponseExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse(MasterChallengeValues.Unknown, 0, 0));

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		public void Get_NoServersOnMaster_EmptyServerListReturned() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(this.goodMasterChallenge);
			apiMock.Setup(x => x.GetServerList()).Returns(Enumerable.Empty<ServerListResponse>());

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			var masterServer = repo.Get("test");

			Assert.AreEqual(0, masterServer.Servers.Count());
		}

		[TestMethod]
		public void Get_ServersOnMaster_ResponseConvertedToIPEndPoints() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(this.goodMasterChallenge);
			apiMock.Setup(x => x.GetServerList()).Returns(new List<ServerListResponse> {
				new ServerListResponse(10, 0, 0, 1, 10666)
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			var masterServer = repo.Get("test");

			Assert.AreEqual(1, masterServer.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", masterServer.Servers.First().ToString());
		}
	}
}
