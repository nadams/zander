using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Exceptions;
using Zander.Provider.Net.Sockets;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class ZandronumMasterServerRepositoryTests {

		[TestMethod]
		public void Get_Address_SameAsPassedIn() {
			var sockets = new Mock<ISocketApi>().Object;
			var repo = new ZandronumMasterServerRepository(sockets);
			var address = "master.server:15000";

			var masterServer = repo.Get("master.server:15000");

			Assert.AreEqual(masterServer.Address, address);
		}

		[TestMethod]
		[ExpectedException(typeof(ClientBannedException))]
		public void Get_ClientHasBeenBanned_ClientBannedExceptionThrown() {
			var sockets = new Mock<ISocketApi>().Object;
			var repoMock = new Mock<ZandronumMasterServerRepository>(sockets);
			repoMock.CallBase = true;
			repoMock.SetupGet(x => x.Challenge).Returns(0L);
			var repo = repoMock.Object;

			var address = "test";

			var masterServer = repo.Get(address);
		}

		[TestMethod]
		[ExpectedException(typeof(ClientIgnoredException))]
		public void Get_ClientHasMadeTooManyRequests_ClientIgnoredExceptionThrown() {
			var sockets = new Mock<ISocketApi>().Object;
			var repo = new ZandronumMasterServerRepository(sockets);
			var address = "test";

			var masterServer = repo.Get(address);
		}

		[TestMethod]
		[ExpectedException(typeof(ObsoleteProtocolVersionException))]
		public void Get_ClientUsingObsoleteProtocol_ObsoleteProtocolVersionExceptionThrown() {
			var sockets = new Mock<ISocketApi>().Object;
			var repoMock = new Mock<ZandronumMasterServerRepository>(sockets);
			repoMock.CallBase = true;
			repoMock.SetupGet(x => x.ProtocolVersion).Returns(1);
			var repo = repoMock.Object;

			var address = "test";

			var masterServer = repo.Get(address);
		}
	}
}
