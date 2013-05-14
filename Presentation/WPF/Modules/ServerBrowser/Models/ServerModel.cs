using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerModel {
		public IPEndPoint Address { get; set; }
		public int MaxClients { get; set; }
		public int MaxPlayers { get; set; }
		public string DisplayName { get; set; }
		public string CurrentMap { get; set; }
		public string IWad { get; set; }
		public string GameName { get; set; }
		public IEnumerable<string> PWads { get; set; }
		public IEnumerable<PlayerModel> CurrentPlayers { get; set; }

		public string Players {
			get {
				return this.CurrentPlayers.Count() + " / " + this.MaxPlayers;
			}
		}

		public ServerModel() {
			this.PWads = Enumerable.Empty<string>();
			this.CurrentPlayers = Enumerable.Empty<PlayerModel>();
		}

        public void CopyData(ServerModel that) {
            this.MaxClients = that.MaxClients;
            this.MaxPlayers = that.MaxPlayers;
            this.DisplayName = that.DisplayName;
            this.CurrentMap = that.CurrentMap;
            this.CurrentPlayers = that.CurrentPlayers;
            this.DisplayName = that.DisplayName;
            this.GameName = that.GameName;
            this.IWad = that.IWad;
            this.MaxClients = that.MaxClients;
            this.MaxPlayers = that.MaxPlayers;
            this.PWads = that.PWads;
        }
	}
}
