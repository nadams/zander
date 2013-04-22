using System.Collections.Generic;
using System.Linq;

namespace Zander.Domain.Remote {
	public class ServerResponse {
		public byte BotSkill { get; set; }
		public byte GameType { get; set; }
		public byte GameSkill { get; set; }
		public byte MaxClients { get; set; }
		public byte MaxPlayers { get; set; }
		public byte NumberOfPlayers { get; set; }
		public byte NumberOfTeams { get; set; }
		public byte NumberOfFlags { get; set; }
		public byte PWadsLoaded { get; set; }
		public bool HasPassword { get; set; }
		public bool HasJoinPassword { get; set; }
		public bool IsInstagib { get; set; }
		public bool IsBuckshot { get; set; }
		public bool IsTestingServer { get; set; }
		public bool UsesSecuritySettings { get; set; }
		public short DuelLimit { get; set; }
		public short FragLimit { get; set; }
		public short PointLimit { get; set; }
		public short TimeLimit { get; set; }
		public short TimeLeft { get; set; }
		public short WinLimit { get; set; }
		public int CurrentTime { get; set; }
		public int CompatFlags { get; set; }
		public int CompatFlags2 { get; set; }
		public int DMFlags { get; set; }
		public int DMFlags2 { get; set; }
		public int DMFlags3 { get; set; }
		public float TeamDamage { get; set; }
		public string Checksum { get; set; }
		public string Email { get; set; }
		public string GameName { get; set; }
		public string IWad { get; set; }
		public string MapName { get; set; }
		public string Name { get; set; }
		public string ServerVersion { get; set; }
		public string TestingBinaryUrl { get; set; }
		public string Url { get; set; }
		public ServerChallengeValues Status { get; set; }
		public ServerQueryValues QueriedFlags { get; set; }
		public IEnumerable<string> PWads { get; set; }
		public IEnumerable<PlayerDataResponse> PlayerData { get; set; }
		public IEnumerable<TeamInfoResponse> Teams { get; set; }

		public ServerResponse() {
			this.PlayerData = Enumerable.Empty<PlayerDataResponse>();
			this.Teams = Enumerable.Empty<TeamInfoResponse>();
			this.PWads = Enumerable.Empty<string>();
		}
	}

	public class PlayerDataResponse {
		public string Name { get; set; }
		public short PointCount { get; set; }
		public ushort Ping { get; set; }
		public bool IsSpectating { get; set; }
		public byte TeamId { get; set; }
		public byte TimeOnServer { get; set; }
	}

	public class TeamInfoResponse {
		public string Name { get; set; }
		public int Color { get; set; }
		public short Score { get; set; }
	}
}
