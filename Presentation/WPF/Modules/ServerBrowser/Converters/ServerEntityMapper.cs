using System;
using System.Linq;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser.Converters {
	public class ServerEntityMapper : IEntityMapper<Server, ServerModel> {

		public ServerModel ModelFromEntity(Server e) {
			var playerMapper = new PlayerEntityMapper();
			var currentPlayers = e.Players.Select(x => playerMapper.ModelFromEntity(x));

			return new ServerModel {
				Address = e.IPEndPoint.ToString(),
				CurrentMap = e.CurrentMap,
				DisplayName = e.DisplayName,
				GameName = e.GameName,
				IWad = e.IWad.Name,
				CurrentPlayers = currentPlayers,
				MaxClients = e.MaxClients,
				MaxPlayers = e.MaxPlayers,
				PWads = e.PWads.Select(x => x.Name)
			};
		}

		public Server EntityFromModel(ServerModel e) {
			throw new NotImplementedException();
		}

        public void CopyModel(ServerModel m1, ServerModel m2) {
            m2.Address = m1.Address;
            m2.CurrentMap = m1.CurrentMap;
            m2.CurrentPlayers = m1.CurrentPlayers;
            m2.DisplayName = m1.DisplayName;
            m2.GameName = m1.GameName;
            m2.IWad = m1.IWad;
            m2.MaxClients = m1.MaxClients;
            m2.MaxPlayers = m1.MaxPlayers;
            m2.PWads = m1.PWads.Select(x => x);
        }
	}
}
