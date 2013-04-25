using System;

namespace Zander.Domain.Entities {

	[Flags]
	public enum CompatFlags {
		None = 0,
		UseShortestTexture = 1 << 0,
		UseOldStairIndex = 1 << 1,
		LimitPainElemental = 1 << 2,
		SilentPickups = 1 << 3,
		InfinitelyTallActors = 1 << 4,
		MagicSilence = 1 << 5,
		Wallrun = 1 << 6,
		SpawnItemsOnFloow = 1 << 7,
		UseBlocking = 1 << 8,
		NoDoorLight = 1 << 9,
		UseRavenScroll = 1 << 10,
		OldSoundCode = 1 << 11,
		LimitDehHelth = 1 << 12,
		Trace = 1 << 13,
		Dropoff = 1 << 14,
		BoomScroll = 1 << 15,
		Invisibility = 1 << 16,
		SilentInstantFloows = 1 << 27,
		UseOldSectorSoundOrigin = 1 << 28,
		UseOldMissileClip = 1 << 29,
		MonsterDropoff = 1 << 30,
		LimitedAirMovement = 1 << 17,
		PlasmaBumpBug = 1 << 18,
		AllowInstantRespawn = 1 << 19,
		DisableTaunts = 1 << 20,
		UseOriginalSoundCurve = 1 << 21,
		UseOldIntermission = 1 << 22,
		DisableStealthMonsters = 1 << 23,
		UseOldRadiusDamage = 1 << 24,
		NoCrosshair = 1 << 25,
		OldWeaponSwitch = 1 << 26
	}
}
