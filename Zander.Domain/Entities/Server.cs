using System.Collections.Generic;

namespace Zander.Domain.Entities {
	public abstract class Server {
		public abstract int Challenge { get; }

		public bool RequiresPassword { get; set; }
		public bool RequiresJoinPassword { get; set; }
		public bool EnforceMasterBanlist { get; set; }
		public int MaxClients { get; set; }
		public int MaxPlayers { get; set; }
		public int FragLimit { get; set; }
		public int TimeLimit { get; set; }
		public int TimeLeft { get; set; }
		public int DuelLimit { get; set; }
		public int PointLimit { get; set; }
		public int WinLimit { get; set; }
		public int NumberOfTeams { get; set; }
		public float TeamDamage { get; set; }
		public string WadUrl { get; set; }
		public string DisplayName { get; set; }
		public string AdminEmail { get; set; }
		public string CurrentMap { get; set; }
		public string TestingServer { get; set; }
		public string DataChecksum { get; set; }
		public string IWad { get; set; }
		public string GameName { get; set; }
		public Skill Skill { get; set; }
		public BotSkill BotSkill { get; set; }
		public DMFlags DMFlags { get; set; }
		public DMFlags2 DMFlags2 { get; set; }
		public DMFlags3 DMFlags3 { get; set; }
		public CompatFlags CompatFlags { get; set; }
		public CompatFlags2 CompatFlags2 { get; set; }
		public IEnumerable<string> PWads { get; set; }
		public IEnumerable<Player> Players { get; set; }
		public IEnumerable<Team> Teams { get; set; }
	}
}
