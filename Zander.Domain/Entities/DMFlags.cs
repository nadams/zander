using System;

namespace Zander.Domain.Entities {

	[Flags]
	public enum DMFlags : uint {
		NoHealth = 1 << 0,
		NoItems = 1 << 1,
		WeaponsStay = 1 << 2,
		FallingDamageZDoom = 1 << 3,
		FallingDamageHexen = 2 << 3,
		FallingDamageSkulltag = 3 << 3,
		StayOnSameLevel = 1 << 6,
		SpawnFarthest = 1 << 7,
		ForceRespawn = 1 << 8,
		NoArmor = 1 << 9,
		NoExit = 1 << 10,
		InfiniteAmmo = 1 << 11,
		NoMonsters = 1 << 12,
		MonstersRespawn = 1 << 13,
		ItemsRespawn = 1 << 14,
		FastMonsters = 1 << 15,
		NoJump = 1 << 16,
		YesJump = 1 << 29,
		NoFreelook = 1 << 17,
		RespawnSuperItems = 1 << 18,
		NoFov = 1 << 19,
		NoCoopWeaponSpawn = 1 << 20,
		NoCrouch = 1 << 21,
		YesCrouch = 1 << 30,
		CoopLoseInventory = 1 << 22,
		CoopLoseKeys = 1 << 23,
		CoopLoseWeapons = 1 << 24,
		CoopLoseArmor = 1 << 25,
		CoopLosePowerups = 1 << 26,
		CoopLoseAmmo = 1 << 27,
		CoopHalveAmmo = 1 << 28,
	}
}
