using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zander.Domain;
using Zander.Domain.Entities;
using Zander.Domain.Remote;

namespace Zander.Provider.Net.Sockets {
	public class ServerRepository : IServerRepository {

		private readonly IRemoteServerApiProvider serverApiProvider;

		public int ServerChallenge {
			get { return (int)ChallengeValues.ServerChallenge; }
		}

		public ServerRepository(IRemoteServerApiProvider serverApiProvider) {
			this.serverApiProvider = serverApiProvider;
		}

		public Server Get(string address, int timeout, ServerQueryValues query) {

			var endpoint = this.GetIPEndPoint(address);

			var api = this.serverApiProvider.GetInstance();
			var request = new ServerRequest(endpoint, timeout, (int)query, this.ServerChallenge, Environment.TickCount);
			var response = api.GetServerInfo(request);

			var server = new Server(endpoint) {
				AdminEmail = response.Email,
				BotSkill = (BotSkill)response.BotSkill,
				CompatFlags = (CompatFlags)response.CompatFlags,
				CompatFlags2 = (CompatFlags2)response.CompatFlags2,
				CurrentMap = response.MapName,
				DataChecksum = response.Checksum,
				DisplayName = response.Name,
				DMFlags = (DMFlags)response.DMFlags,
				DMFlags2 = (DMFlags2)response.DMFlags2,
				DMFlags3 = (DMFlags3)response.DMFlags3,
				DuelLimit = response.DuelLimit,
				EnforceMasterBanlist = response.UsesSecuritySettings,
				FragLimit = response.FragLimit,
				GameName = response.GameName,
				IsTestingServer = response.IsTestingServer,
				IWad = new Wad { Name = response.IWad },
				MaxClients = response.MaxClients,
				MaxPlayers = response.MaxPlayers,
				NumberOfTeams = response.NumberOfTeams,
				PointLimit = response.PointLimit,
				RequiresJoinPassword = response.HasJoinPassword,
				RequiresPassword = response.HasPassword,
				Skill = (Skill)response.GameSkill,
				TeamDamage = response.TeamDamage,
				TestingServer = response.TestingBinaryUrl,
				TimeLeft = response.TimeLeft,
				TimeLimit = response.TimeLimit,
				WadUrl = response.Url,
				WinLimit = response.WinLimit,
				PWads = response.PWads.Select(x => new Wad {
					Name = x
				}),
			};

			var teams = response.Teams.Select(x => new Team {
				Color = x.Color,
				Name = x.Name,
				Score = x.Score
			}).ToList();

			server.Players = response.PlayerData.Select(x => {
				var player = new Player {
					IsBot = x.IsBot,
					IsSpectating = x.IsSpectating,
					Name = x.Name,
					Ping = x.Ping,
					PointCount = x.PointCount,
					TimeOnServer = x.TimeOnServer
				};

				if(x.TeamId < teams.Count) {
					player.Team = teams[x.TeamId];
				}

				return player;
			});

			server.Teams = teams;

			return server;
		}

		private IPEndPoint GetIPEndPoint(string address) {
			var split = address.Split(':');

			return new IPEndPoint(IPAddress.Parse(split[0]), int.Parse(split[1]));
		}
	}
}
