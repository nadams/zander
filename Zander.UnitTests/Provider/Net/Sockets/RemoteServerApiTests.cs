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
				Concat(new byte[] { (byte)MasterChallengeValues.ServerBlock, 1 }).
				Concat(new byte[] { 10, 0, 0, 1 }).
				Concat(BitConverter.GetBytes((ushort)10666)).
				Concat(new byte[] { 0 }).
				ToArray();

			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, 0, 0);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(1, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.First().ToString());
		}

		[TestMethod]
		public void ChallengeMasterServer_TwoServersInIPAvailable_TwoServersReturned() {
			var masterResponse =
				BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
				Concat(new byte[] { 0 }).
				Concat(new byte[] { (byte)MasterChallengeValues.ServerBlock, 2 }).
				Concat(new byte[] { 10, 0, 0, 1 }).
				Concat(BitConverter.GetBytes((ushort)10666)).
				Concat(BitConverter.GetBytes((ushort)10667)).
				Concat(new byte[] { 0 }).
				ToArray();

			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, (int)ChallengeValues.MasterChallenge, (short)ChallengeValues.MasterProtocol);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(2, response.Servers.Count());
			Assert.AreEqual("10.0.0.1:10666", response.Servers.ElementAt(0).ToString());
			Assert.AreEqual("10.0.0.1:10667", response.Servers.ElementAt(1).ToString());
		}

		[TestMethod]
		public void ChallengeMasterServer_MultipleServersInMultipleAddressRanges_MultipleServersReturned() {
			var masterResponse =
				BitConverter.GetBytes((int)MasterChallengeValues.BeginningOfServerList).
				Concat(new byte[] { 0 }).
				Concat(new byte[] { (byte)MasterChallengeValues.ServerBlock, 2 }).
				Concat(new byte[] { 10, 0, 0, 1 }).
				Concat(BitConverter.GetBytes((ushort)10666)).
				Concat(BitConverter.GetBytes((ushort)10667)).
				Concat(new byte[] { 3 }).
				Concat(new byte[] { 10, 0, 0, 10 }).
				Concat(BitConverter.GetBytes((ushort)10666)).
				Concat(BitConverter.GetBytes((ushort)10667)).
				Concat(BitConverter.GetBytes((ushort)10668)).
				Concat(new byte[] { 0 }).
				ToArray();

			var socket = this.GetSocket(masterResponse);

			var request = new MasterChallengeRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 0, (int)ChallengeValues.MasterChallenge, (short)ChallengeValues.MasterProtocol);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.ChallengeMasterServer(request);

			Assert.AreEqual(5, response.Servers.Count());
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

		[TestMethod]
		public void Get_MapName_MapNameReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.MapName).
				Concat(this.encoding.GetBytes("map21\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.MapName, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("map21", response.MapName);
		}

		[TestMethod]
		public void Get_MaxClients_MaxClientsReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.MaxClients).
				Concat(new byte[] { 16 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.MaxClients, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(16, response.MaxClients);
		}

		[TestMethod]
		public void Get_MaxPlayers_MaxPlayersReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.MaxPlayers).
				Concat(new byte[] { 12 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.MaxPlayers, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(12, response.MaxPlayers);
		}

		[TestMethod]
		public void Get_GameType_GameTypeReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.GameType).
				Concat(new byte[] { (byte)GameMode.Ctf, 1, 0 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.GameType, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual((byte)GameMode.Ctf, response.GameType);
			Assert.AreEqual(true, response.IsInstagib);
			Assert.AreEqual(false, response.IsBuckshot);
		}

		[TestMethod]
		public void Get_GameName_GameNameReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.GameName).
				Concat(this.encoding.GetBytes("DOOM II\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.GameType, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("DOOM II", response.GameName);
		}

		[TestMethod]
		public void Get_IWad_IWadReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.IWad).
				Concat(this.encoding.GetBytes("doom2.wad\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.IWad, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("doom2.wad", response.IWad);
		}

		[TestMethod]
		public void Get_ForcePassword_ForcePasswordReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.ForcePassword).
				Concat(new byte[] { 1 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.ForcePassword, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(true, response.HasPassword);
		}

		[TestMethod]
		public void Get_ForceJoinPassword_ForceJoinPasswordReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.ForceJoinPassword).
				Concat(new byte[] { 1 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.ForceJoinPassword, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(true, response.HasJoinPassword);
		}

		[TestMethod]
		public void Get_GameSkill_GameSkillReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.GameSkill).
				Concat(new byte[] { (byte)Skill.UltraViolence }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.GameSkill, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual((byte)Skill.UltraViolence, response.GameSkill);
		}

		[TestMethod]
		public void Get_BotSkill_BotSkillReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.BotSkill).
				Concat(new byte[] { (byte)BotSkill.Supreme }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.BotSkill, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual((byte)BotSkill.Supreme, response.BotSkill);
		}

		[TestMethod]
		public void Get_Limits_AllLimitsReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.Limits).
				Concat(BitConverter.GetBytes((short)50)).
				Concat(BitConverter.GetBytes((short)10)).
				Concat(BitConverter.GetBytes((short)2)).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)0)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.Limits, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(50, response.FragLimit);
			Assert.AreEqual(10, response.TimeLimit);
			Assert.AreEqual(2, response.TimeLeft);
			Assert.AreEqual(0, response.DuelLimit);
			Assert.AreEqual(0, response.PointLimit);
			Assert.AreEqual(0, response.WinLimit);
		}

		[TestMethod]
		public void Get_TimeLimitIsZero_TimeLeftIsNotIncluded() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.Limits).
				Concat(BitConverter.GetBytes((short)50)).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)0)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.Limits, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(50, response.FragLimit);
			Assert.AreEqual(0, response.TimeLimit);
			Assert.AreEqual(0, response.TimeLeft);
			Assert.AreEqual(0, response.DuelLimit);
			Assert.AreEqual(0, response.PointLimit);
			Assert.AreEqual(0, response.WinLimit);
		}

		[TestMethod]
		public void Get_TeamDamage_TeamDamageReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.TeamDamage).
				Concat(BitConverter.GetBytes((float)22.5)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.TeamDamage, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(22.5, response.TeamDamage);
		}

		[TestMethod]
		public void Get_RequestPlayerInfoOnePlayerExists_OnePlayerInfoReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)(ServerQueryValues.PlayerData | ServerQueryValues.NumberOfPlayers)).
				Concat(new byte[] { 1 }).
				Concat(this.encoding.GetBytes("Lightning Larry\0")).
				Concat(BitConverter.GetBytes((short)22)).
				Concat(BitConverter.GetBytes((short)154)).
				Concat(new byte[] { 0, 0, 255, 8 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(
				new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, 
				(int)(ServerQueryValues.PlayerData | ServerQueryValues.NumberOfPlayers), 
				(int)ChallengeValues.ServerChallenge, 
				5
			);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);
			var player = response.PlayerData.First();

			Assert.AreEqual(1, response.PlayerData.Count());
			Assert.AreEqual("Lightning Larry", player.Name);
			Assert.AreEqual(22, player.PointCount);
			Assert.AreEqual(154, player.Ping);
			Assert.IsFalse(player.IsSpectating);
			Assert.IsFalse(player.IsBot);
			Assert.AreEqual(255, player.TeamId);
			Assert.AreEqual(8, player.TimeOnServer);
		}

		[TestMethod]
		public void Get_RequestPlayerInfoMultiplePlayersExist_MultiplePlayersReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)(ServerQueryValues.PlayerData | ServerQueryValues.NumberOfPlayers)).
				Concat(new byte[] { 2 }).
				Concat(this.encoding.GetBytes("Lightning Larry\0")).
				Concat(BitConverter.GetBytes((short)22)).
				Concat(BitConverter.GetBytes((short)154)).
				Concat(new byte[] { 0, 0, 255, 8 }).
				Concat(this.encoding.GetBytes("derp\0")).
				Concat(BitConverter.GetBytes((short)0)).
				Concat(BitConverter.GetBytes((short)317)).
				Concat(new byte[] { 1, 0, 255, 1 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);

			var request = new ServerRequest(
				new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000,
				(int)(ServerQueryValues.PlayerData | ServerQueryValues.NumberOfPlayers),
				(int)ChallengeValues.ServerChallenge,
				5
			);

			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(2, response.PlayerData.Count());
		}

		[TestMethod]
		public void Get_TeamInfoOneTeamExists_OneTeamReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.TeamInfo).
				Concat(new byte[] { 1 }).
				Concat(this.encoding.GetBytes("Red\0")).
				Concat(BitConverter.GetBytes(45)).
				Concat(BitConverter.GetBytes((short)23)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.TeamInfo, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);
			var team = response.Teams.First();

			Assert.AreEqual(1, response.Teams.Count());
			Assert.AreEqual("Red", team.Name);
			Assert.AreEqual(45, team.Color);
			Assert.AreEqual(23, team.Score);
		}

		[TestMethod]
		public void Get_TeamInfoMultipleTeamsExists_MultipleTeamsReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.TeamInfo).
				Concat(new byte[] { 4 }).
				Concat(this.encoding.GetBytes("Red\0")).
				Concat(this.encoding.GetBytes("Blue\0")).
				Concat(this.encoding.GetBytes("White\0")).
				Concat(this.encoding.GetBytes("Green\0")).
				Concat(BitConverter.GetBytes((short)1)).
				Concat(BitConverter.GetBytes((short)2)).
				Concat(BitConverter.GetBytes((short)3)).
				Concat(BitConverter.GetBytes((short)4)).
				Concat(BitConverter.GetBytes(1)).
				Concat(BitConverter.GetBytes(4)).
				Concat(BitConverter.GetBytes(2)).
				Concat(BitConverter.GetBytes(0)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.TeamInfo, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(4, response.Teams.Count());
		}

		[TestMethod]
		public void Get_IsTestingServer_TestingServerDetailsReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.TestingServer).
				Concat(new byte[] { 1 }).
				Concat(this.encoding.GetBytes("The latest zandronum binary.zip\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.TestingServer, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.IsTrue(response.IsTestingServer);
			Assert.AreEqual("The latest zandronum binary.zip", response.TestingBinaryUrl);
		}

		[TestMethod]
		public void Get_IsNotTestingServer_FalseReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.TestingServer).
				Concat(new byte[] { 0 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.TestingServer, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.IsFalse(response.IsTestingServer);
			Assert.IsNull(response.TestingBinaryUrl);
		}

		[TestMethod]
		public void Get_DataChecksum_ChecksumReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.DataChecksum).
				Concat(this.encoding.GetBytes("38794fhwergfbn234875bgf2387gbe8r\0")).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.DataChecksum, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual("38794fhwergfbn234875bgf2387gbe8r", response.Checksum);
		}

		[TestMethod]
		public void Get_AllFlagsRequested_AllFlagsReturned() {
			var dmflags = (int)(DMFlags.NoExit | DMFlags.SpawnFarthest);
			var dmflags2 = (int)(DMFlags2.LoseFragWhenKilled | DMFlags2.Degeneration | DMFlags2.BarrelsRespawn);
			var dmflags3 = (int)DMFlags3.None;
			var compatflags = (int)(CompatFlags.Invisibility | CompatFlags.LimitDehHelth);
			var compatflags2 = (int)CompatFlags2.None;

			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.AllDMFlags).
				Concat(new byte[] { 5 }).
				Concat(BitConverter.GetBytes(dmflags)).
				Concat(BitConverter.GetBytes(dmflags2)).
				Concat(BitConverter.GetBytes(dmflags3)).
				Concat(BitConverter.GetBytes(compatflags)).
				Concat(BitConverter.GetBytes(compatflags2)).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.AllDMFlags, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(dmflags, response.DMFlags);
			Assert.AreEqual(dmflags2, response.DMFlags2);
			Assert.AreEqual(dmflags3, response.DMFlags3);
			Assert.AreEqual(compatflags, response.CompatFlags);
			Assert.AreEqual(compatflags2, response.CompatFlags2);
		}

		[TestMethod]
		public void Get_SecuritySettings_SecuritySettingReturned() {
			var serverResponse =
				BitConverter.GetBytes((int)ServerQueryValues.SecuritySettings).
				Concat(new byte[] { 1 }).
				ToArray();

			var socket = this.GetServerSocket(serverResponse);
			var request = new ServerRequest(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 15300), 1000, (int)ServerQueryValues.SecuritySettings, (int)ChallengeValues.ServerChallenge, 5);
			var api = new RemoteServerApi(new EmptyCompressor(), new FakeSocketProvider(socket));
			var response = api.GetServerInfo(request);

			Assert.AreEqual(true, response.UsesSecuritySettings);
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
