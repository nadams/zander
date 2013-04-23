using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class ServerRepositoryTests {
		
		[TestMethod]
		public void Get_ConvertIntToFlags_FlagsConverted() {
			var dmflags = DMFlags.CoopHalveAmmo | DMFlags.FallingDamageHexen | DMFlags.NoArmor;
			var dmflags2 = DMFlags2.BarrelsRespawn | DMFlags2.Degeneration;
			var dmflags3 = DMFlags3.NoUnlagged;
			var compatFlags = CompatFlags.BoomScroll | CompatFlags.LimitDehHelth;
			var compatFlags2 = CompatFlags2.None;

			var remoteApiMock = new Mock<IRemoteServerApi>();
			remoteApiMock.Setup(x => x.GetServerInfo(It.Is<ServerRequest>(y => y.Query == (int)ServerQueryValues.AllDmflags))).Returns(new ServerResponse { 
				DMFlags = (int)dmflags,
				DMFlags2 = (int)dmflags2,
				DMFlags3 = (int)dmflags3,
				CompatFlags = (int)compatFlags,
				CompatFlags2 = (int)compatFlags2
			});

			Assert.Fail();

			//var api = new ServerRepository(new FakeServerProvider(remoteApiMock.Object));
			//var response = api.Get(new IPEndPoint(IPAddress.Any, 0), ServerQueryValues.AllDmflags);

			//Assert.AreEqual(dmflags, response.DMFlags);
			//Assert.AreEqual(dmflags2, response.DMFlags2);
			//Assert.AreEqual(dmflags3, response.DMFlags3);
			//Assert.AreEqual(compatFlags, response.CompatFlags);
			//Assert.AreEqual(compatFlags2, response.CompatFlags2);
		}

		private class FakeServerProvider : IRemoteServerApiProvider {

			private readonly IRemoteServerApi api;

			public FakeServerProvider(IRemoteServerApi api) {
				this.api = api;
			}

			public IRemoteServerApi GetInstance(string address, int timeout) {
				return this.api;
			}
		}
	}
}
