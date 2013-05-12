using System.Collections.Generic;
using System.Linq;
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
			remoteApiMock.Setup(x => x.GetServerInfo(It.Is<ServerRequest>(y => y.Query == (int)ServerQueryValues.AllDMFlags))).Returns(new ServerResponse { 
				DMFlags = (int)dmflags,
				DMFlags2 = (int)dmflags2,
				DMFlags3 = (int)dmflags3,
				CompatFlags = (int)compatFlags,
				CompatFlags2 = (int)compatFlags2
			});

			var api = new ServerRepository(new FakeServerProvider(remoteApiMock.Object));
			var response = api.Get("10.0.0.1:15300", 1000, ServerQueryValues.AllDMFlags);

			Assert.AreEqual(dmflags, response.DMFlags);
			Assert.AreEqual(dmflags2, response.DMFlags2);
			Assert.AreEqual(dmflags3, response.DMFlags3);
			Assert.AreEqual(compatFlags, response.CompatFlags);
			Assert.AreEqual(compatFlags2, response.CompatFlags2);
		}

		[TestMethod]
		public void Get_TeamsAndPlayers_TeamsAreAssignedToPlayers() {
			var remoteApiMock = new Mock<IRemoteServerApi>();

			remoteApiMock.Setup(x => x.GetServerInfo(It.IsAny<ServerRequest>())).Returns(() => {
				var teams = new List<TeamInfoResponse> {
					new TeamInfoResponse {
						Color = 1,
						Name = "red",
						Score = 3
					},
					new TeamInfoResponse {
						Color = 2,
						Name = "blue",
						Score = 2
					}
				};

				var players = new List<PlayerDataResponse> { 
					new PlayerDataResponse {
						Name = "Bob",
						Ping = 45,
						PointCount = 1,
						TeamId = 0,
						TimeOnServer = 5
					},
					new PlayerDataResponse {
						Name = "Joe",
						Ping = 243,
						PointCount = 2,
						TeamId = 0,
						TimeOnServer = 10
					},
					new PlayerDataResponse {
						Name = "Lightning Larry",
						Ping = 22,
						PointCount = 2,
						TeamId = 1,
						TimeOnServer = 10
					}
				};

				var serverResponse = new ServerResponse { 
					Teams = teams,
					PlayerData = players
				};

				return serverResponse;
			});

			var api = new ServerRepository(new FakeServerProvider(remoteApiMock.Object));
			var response = api.Get("10.0.0.1:15300", 1000, ServerQueryValues.PlayerData | ServerQueryValues.TeamInfo);

			var redTeam = response.Teams.ElementAt(0);
			var blueTeam = response.Teams.ElementAt(1);
			var bob = response.Players.ElementAt(0);
			var joe = response.Players.ElementAt(1);
			var lightningLarry = response.Players.ElementAt(2);

			Assert.AreSame(redTeam, bob.Team);
			Assert.AreSame(redTeam, joe.Team);
			Assert.AreSame(blueTeam, lightningLarry.Team);
		}

		[TestMethod]
		public void Get_NoTeamsExist_TeamsOnPlayerAreNull() {
			var remoteApiMock = new Mock<IRemoteServerApi>();

			remoteApiMock.Setup(x => x.GetServerInfo(It.IsAny<ServerRequest>())).Returns(() => {
				var teams = Enumerable.Empty<TeamInfoResponse>();

				var players = new List<PlayerDataResponse> { 
					new PlayerDataResponse {
						Name = "Bob",
						Ping = 45,
						PointCount = 1,
						TimeOnServer = 5
					},
					new PlayerDataResponse {
						Name = "Joe",
						Ping = 243,
						PointCount = 2,
						TimeOnServer = 10
					}
				};

				var serverResponse = new ServerResponse {
					Teams = teams,
					PlayerData = players
				};

				return serverResponse;
			});

			var api = new ServerRepository(new FakeServerProvider(remoteApiMock.Object));
			var response = api.Get("10.0.0.1:15300", 1000, ServerQueryValues.PlayerData | ServerQueryValues.TeamInfo);

			var bob = response.Players.ElementAt(0);
			var joe = response.Players.ElementAt(1);

			Assert.IsNull(bob.Team);
			Assert.IsNull(joe.Team);
		}

		[TestMethod]
		public void Get_PWads_PWadsConvertedToWad() {
			var remoteApiMock = new Mock<IRemoteServerApi>();

			remoteApiMock.Setup(x => x.GetServerInfo(It.Is<ServerRequest>(y => ((ServerQueryValues)y.Query) == ServerQueryValues.PWads))).Returns(() => {
				var pwads = new List<string> {
					"av.wad",
					"av20.wad",
					"brutaldoom.pk3",
				};

				var serverResponse = new ServerResponse {
					PWads = pwads,
				};

				return serverResponse;
			});

			var api = new ServerRepository(new FakeServerProvider(remoteApiMock.Object));
			var response = api.Get("10.0.0.1:15300", 1000, ServerQueryValues.PWads);

			Assert.AreEqual("av.wad", response.PWads.ElementAt(0).Name);
			Assert.AreEqual("av20.wad", response.PWads.ElementAt(1).Name);
			Assert.AreEqual("brutaldoom.pk3", response.PWads.ElementAt(2).Name);
		}

		private class FakeServerProvider : IRemoteServerApiProvider {

			private readonly IRemoteServerApi api;

			public FakeServerProvider(IRemoteServerApi api) {
				this.api = api;
			}

			public IRemoteServerApi GetInstance() {
				return this.api;
			}
		}
	}
}
