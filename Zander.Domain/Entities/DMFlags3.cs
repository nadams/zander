using System;

namespace Zander.Domain.Entities {

	[Flags]
	public enum DMFlags3 : uint {
		NoTargetIdentify = 1 << 0,
		ApplyLmsSpectatorSettings = 1 << 1,
		NoCoopInfo = 1 << 2,
		NoUnlagged = 1 << 3,
		UnblockPlayers = 1 << 4
	}
}
