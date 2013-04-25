using System.Collections.Generic;
using System.Linq;
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
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse { 
				PacketNumber = 0, 
				ServerBlock = MasterChallengeValues.ServerBlock,
				Status = MasterChallengeValues.BeginningOfServerList
			});

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);
			var address = "10.0.0.1:15000";

			var masterServer = repo.Get("10.0.0.1:15000", 1000);

			Assert.AreEqual(address, masterServer.Address);
		}

		[TestMethod]
		[ExpectedException(typeof(ObsoleteProtocolException))]
		public void Get_ClientUsingObsoleteProtocol_ObsoleteProtocolExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse { Status = MasterChallengeValues.ObsoleteProtocol });

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			repo.Get("10.0.0.1:15000", 1000);
		}

		[TestMethod]
		[ExpectedException(typeof(ClientBannedException))]
		public void Get_ClientHasBeenBanned_ClientBannedExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse { Status = MasterChallengeValues.Banned });

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			repo.Get("10.0.0.1:15000", 1000);
		}

		[TestMethod]
		[ExpectedException(typeof(ClientIgnoredException))]
		public void Get_ClientHasMadeTooManyRequests_ClientIgnoredExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse { Status = MasterChallengeValues.Denied });

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			repo.Get("10.0.0.1:15000", 1000);
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownMasterServerResponseException))]
		public void Get_ClientGetUnknownStatus_UnknownMasterServerResponseExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>()))
				.Returns(new MasterChallengeResponse { Status = MasterChallengeValues.Unknown });

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			repo.Get("10.0.0.1:15000", 1000);
		}

		[TestMethod]
		public void Get_NoServersOnMaster_EmptyServerListReturned() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse { 
				PacketNumber = 0,
				ServerBlock = MasterChallengeValues.ServerBlock,
				Status = MasterChallengeValues.BeginningOfServerList,
				Servers = Enumerable.Empty<ServerListResponse>()
			});

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			var masterServer = repo.Get("10.0.0.1:15000", 1000);

			Assert.AreEqual(0, masterServer.Servers.Count());
		}

		[TestMethod]
		public void Get_ServersOnMaster_ResponseConvertedToIPEndPoints() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				PacketNumber = 0,
				ServerBlock = MasterChallengeValues.ServerBlock,
				Status = MasterChallengeValues.BeginningOfServerList,
				Servers = new List<ServerListResponse> { 
					new ServerListResponse(10, 0, 0, 1, 10666)
				}
			});

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			var masterServer = repo.Get("10.0.0.1:15000", 1000);

			Assert.AreEqual(1, masterServer.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", masterServer.Servers.First().ToString());
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownMasterServerResponseException))]
		public void Get_ServersOnMasterInvalidServerBlockValue_UnknownMasterServerResponseExceptionThrown() {
			var apiMock = new Mock<IRemoteServerApi>();
			apiMock.Setup(x => x.ChallengeMasterServer(It.IsAny<MasterChallengeRequest>())).Returns(new MasterChallengeResponse {
				PacketNumber = 0,
				ServerBlock = MasterChallengeValues.Unknown,
				Status = MasterChallengeValues.BeginningOfServerList,
				Servers = new List<ServerListResponse> { 
					new ServerListResponse(10, 0, 0, 1, 10666)
				}
			});

			var apiProviderMock = new Mock<IRemoteServerApiProvider>();
			apiProviderMock.Setup(x => x.GetInstance()).Returns(apiMock.Object);

			var repo = new ZandronumMasterServerRepository(apiProviderMock.Object);

			var masterServer = repo.Get("10.0.0.1:15000", 1000);
		}
	}
}
