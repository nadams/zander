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
			var api = new Mock<IRemoteServerApi>().Object;
			var repo = new ZandronumMasterServerRepository(api);
			var address = "master.server:15000";

			var masterServer = repo.Get("master.server:15000");

			Assert.AreEqual(masterServer.Address, address);
		}

		[TestMethod]
		[ExpectedException(typeof(ObsoleteProtocolException))]
		public void Get_ClientUsingObsoleteProtocol_ObsoleteProtocolExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(() => {
				var response = new MasterChallengeResponse();
				response.Status = MasterChallengeStatus.ObsoleteProtocol;

				return response;
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientBannedException))]
		public void Get_ClientHasBeenBanned_ClientBannedExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(() => {
				var response = new MasterChallengeResponse();
				response.Status = MasterChallengeStatus.Banned;

				return response;
			});

			var repo = new ZandronumMasterServerRepository(apiMock.Object);

			repo.Get("test");
		}

		[TestMethod]
		[ExpectedException(typeof(ClientIgnoredException))]
		public void Get_ClientHasMadeTooManyRequests_ClientIgnoredExceptionThrown() {

		}
	}
}
