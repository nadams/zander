using System.Collections.Generic;
using System.Linq;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerModel {
		public int MaxClients { get; set; }
		public int MaxPlayers { get; set; }
		public string DisplayName { get; set; }
		public string CurrentMap { get; set; }
		public string IWad { get; set; }
		public string GameName { get; set; }
		public string Address { get; set; }
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
	}
}
