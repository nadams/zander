using System;

namespace Zander.Domain.Entities {

	[Flags]
	public enum CompatFlags2 : uint {
		NetScriptsAreClientSide = 1 << 0,
		ClientSendsFullButtonInfo = 1 << 1,
		NoLand = 1 << 2,
		UseOldNumberGenerator = 1 << 3,
		NoGravitySpheres = 1 << 4,
		DoNotStopPlayerScriptsOnDisconnect = 1 << 5,
		OldExplosionThrust = 1 << 6,
		OldBridgeDrops = 1 << 7,
		UseOldZDoomJumpPhysics = 1 << 8
	}
}
