using System;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser.Converters {
	public class PlayerEntityMapper : IEntityMapper<Player, PlayerModel> {
		public PlayerModel ModelFromEntity(Player e) {
			return new PlayerModel { 
				IsBot = e.IsBot,
				IsSpectating = e.IsSpectating,
				Name = e.Name,
				Ping = e.Ping,
				PointCount = e.PointCount,
				TimeOnServer = e.TimeOnServer
			};
		}

		public Player EntityFromModel(PlayerModel e) {
			throw new NotImplementedException();
		}
	}
}
