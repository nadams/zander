using System;

namespace Zander.Domain.Query {

	[Flags]
	public enum ServerQueryEnum : uint {
		Name = 0x1,
		Url = 0x2,
		Email = 0x4,
		MapName = 0x8,
		MaxClients = 0x10,
		MaxPlayers = 0x20,
		PWads = 0x40,
		GameType = 0x80,
		GameName = 0x100,
		IWad = 0x200,
		ForcePassword = 0x400,
		ForceJoinPassword = 0x800,
		GameSkill = 0x1000,
		BotSkill = 0x2000,
		[Obsolete] Dmflags = 0x4000,
		Limits = 0x10000,
		TeamDamage = 0x20000,
		[Obsolete] TeamScores = 0x40000,
		NumberOfPlayers = 0x80000,
		PlayerData = 0x100000,
		TeamInfoNumber = 0x200000,
		TeamInfoName = 0x400000,
		TeamInfoColor = 0x800000,
		TeamInfoScore = 0x1000000,
		TestingServer = 0x2000000,
		DataChecksum = 0x4000000,
		AllDmflags = 0x8000000,
		Securitysettings = 0x10000000,

		AllData = Name 
			| Url
			| Email
			| MapName
			| MaxClients
			| MaxPlayers
			| PWads
			| GameType
			| GameName
			| IWad
			| ForcePassword
			| ForceJoinPassword
			| GameSkill
			| BotSkill
			| Limits
			| TeamDamage
			| NumberOfPlayers
			| PlayerData
			| TeamInfoNumber
			| TeamInfoName
			| TeamInfoColor
			| TeamInfoScore
			| TestingServer
			| DataChecksum 
			| AllDmflags
			| Securitysettings 
	}
}
