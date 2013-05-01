using System;
using System.Linq;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser.Converters {
	public class ServerEntityMapper : IEntityMapper<Server, ServerModel> {

		public ServerModel ModelFromEntity(Server e) {
			return new ServerModel {
				Address = e.IPEndPoint.ToString(),
				CurrentMap = e.CurrentMap,
				DisplayName = e.DisplayName,
				GameName = e.DisplayName,
				IWad = e.IWad.Name,
				MaxClients = e.MaxClients,
				MaxPlayers = e.MaxPlayers,
				PWads = e.PWads.Select(x => x.Name)
			};
		}

		public Server EntityFromModel(ServerModel e) {
			throw new NotImplementedException();
		}
	}
}
