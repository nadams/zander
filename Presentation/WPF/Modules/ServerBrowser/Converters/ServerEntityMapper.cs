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
				Address = e.IPEndPoint,
				CurrentMap = e.CurrentMap,
				DisplayName = e.DisplayName,
				GameName = e.GameName,
				IWad = e.IWad.Name,
				MaxClients = e.MaxClients,
				MaxPlayers = e.MaxPlayers,
				CurrentPlayers = currentPlayers.ToList(),
				PWads = e.PWads.Select(x => x.Name).ToList()
			};
		}

		public Server EntityFromModel(ServerModel e) {
			throw new NotImplementedException();
		}
	}
}
