using System;

namespace Zander.Domain.Entities {

	[Flags]
	public enum DMFlags2 : uint {
		DrawWeapon = 1 << 1,
		NoRunes = 1 << 2,
		InstantFlagReturn = 1 << 3,
		NoTeamSwitch = 1 << 4,
		NoTeamSelect = 1 << 5,
		DoubleAmmo = 1 << 6,
		Degeneration = 1 << 7,
		BfgFreeAim = 1 << 8,
		BarrelsRespawn = 1 << 9,
		NoRespawnInvulnerability = 1 << 10,
		CoopStartWithShotgun = 1 << 11,
		SpawnWhereDied = 1 << 12,
		KeepTeams = 1 << 13,
		KeepFrags = 1 << 14,
		NoRespawn = 1 << 15,
		LoseFragWhenKilled = 1 << 16,
		InfiniteInventory = 1 << 17,
		KillAllMonstersBeforeExit = 1 << 22,
		NoAutomap = 1 << 23,
		NoAutomapAllies = 1 << 24,
		DisallowSpying = 1 << 25,
		Chasecam = 1 << 26,
		NoSuicide = 1 << 27,
		NoAutoaim = 1 << 28,
		ForceGlDefaults = 1 << 18,
		NoRocketJumping = 1 << 19,
		AwardDamageInsteadOfKills = 1 << 20,
		ForceAlpha = 1 << 21,
		CoopSinglePlayerActorSpawn = 1 << 29
	}
}
