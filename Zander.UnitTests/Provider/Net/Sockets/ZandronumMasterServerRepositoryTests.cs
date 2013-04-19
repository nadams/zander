using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Exceptions;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class ZandronumMasterServerRepositoryTests {

		[TestMethod]
		public void Get_Address_SameAsPassedIn() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(() => new MasterChallengeResponse { 
				Status = MasterChallengeStatus.BeginningOfServerList,
				ServerBlock = 0,
				PacketNumber = 0
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);
			var address = "master.server:15000";

			var masterServer = repo.Get("master.server:15000");

			Assert.AreEqual(masterServer.Address, address);
		}

		[TestMethod]
		[ExpectedException(typeof(ObsoleteProtocolException))]
		public void Get_ClientUsingObsoleteProtocol_ObsoleteProtocolExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				Status = MasterChallengeStatus.ObsoleteProtocol
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientBannedException))]
		public void Get_ClientHasBeenBanned_ClientBannedExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				Status = MasterChallengeStatus.Banned
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientIgnoredException))]
		public void Get_ClientHasMadeTooManyRequests_ClientIgnoredExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				Status = MasterChallengeStatus.Denied
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownMasterServerResponseException))]
		public void Get_ClientGetUnknownStatus_UnknownMasterServerResponseExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				Status = MasterChallengeStatus.Unknown
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}
	}
}
