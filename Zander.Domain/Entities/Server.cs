using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Zander.Domain.Entities {
	public class Server {
		private readonly IPEndPoint ipEndPoint;
		public IPEndPoint IPEndPoint {
			get {
				return this.ipEndPoint;
			}
		}

		public bool RequiresPassword { get; set; }
		public bool RequiresJoinPassword { get; set; }
		public bool EnforceMasterBanlist { get; set; }
		public bool IsTestingServer { get; set; }
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
		public Wad IWad { get; set; }
		public string GameName { get; set; }
		public Skill Skill { get; set; }
		public BotSkill BotSkill { get; set; }
		public DMFlags DMFlags { get; set; }
		public DMFlags2 DMFlags2 { get; set; }
		public DMFlags3 DMFlags3 { get; set; }
		public CompatFlags CompatFlags { get; set; }
		public CompatFlags2 CompatFlags2 { get; set; }
		public IEnumerable<Wad> PWads { get; set; }
		public IEnumerable<Player> Players { get; set; }
		public IEnumerable<Team> Teams { get; set; }

		public Server(IPEndPoint endPoint) {
			this.PWads = Enumerable.Empty<Wad>();
			this.Players = Enumerable.Empty<Player>();
			this.Teams = Enumerable.Empty<Team>();
			this.ipEndPoint = endPoint;
		}

        public void CopyData(Server other) {
            this.AdminEmail = other.AdminEmail;
            this.BotSkill = other.BotSkill;
            this.CompatFlags = other.CompatFlags;
            this.CompatFlags2 = other.CompatFlags2;
            this.CurrentMap = other.CurrentMap;
            this.DataChecksum = other.DataChecksum;
            this.DisplayName = other.DisplayName;
            this.DMFlags = other.DMFlags;
            this.DMFlags2 = other.DMFlags2;
            this.DMFlags3 = other.DMFlags3;
            this.DuelLimit = other.DuelLimit;
            this.EnforceMasterBanlist = other.EnforceMasterBanlist;
            this.FragLimit = other.FragLimit;
            this.GameName = other.GameName;
            this.IsTestingServer = other.IsTestingServer;
            this.IWad = other.IWad;
            this.MaxClients = other.MaxClients;
            this.MaxPlayers = other.MaxPlayers;
            this.NumberOfTeams = other.NumberOfTeams;
            this.Players = other.Players;
            this.PointLimit = other.PointLimit;
            this.PWads = other.PWads;
            this.RequiresJoinPassword = other.RequiresJoinPassword;
            this.RequiresPassword = other.RequiresPassword;
            this.Skill = other.Skill;
            this.TeamDamage = other.TeamDamage;
            this.Teams = other.Teams;
            this.TestingServer = other.TestingServer;
            this.TimeLeft = other.TimeLeft;
            this.TimeLimit = other.TimeLimit;
            this.WadUrl = other.WadUrl;
            this.WinLimit = other.WinLimit;
        }
	}
}
