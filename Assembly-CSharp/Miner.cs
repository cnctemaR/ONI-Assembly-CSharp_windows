using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Miner : RoleConfig
{
	public Miner()
	{
		base.id = Miner.ID;
		base.name = DUPLICANTS.ROLES.MINER.NAME;
		base.description = DUPLICANTS.ROLES.MINER.DESCRIPTION;
		base.roleGroup = "Mining";
		base.hat = Game.Instance.roleManager.GetHat(Miner.ID);
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Dig };
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Digging };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseDigSpeedMedium,
			RoleManager.rolePerks.CanDigVeryFirm,
			RoleManager.rolePerks.CanDigNearlyImpenetrable
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Dig,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorMiner
		};
	}

	public static string ID = "Miner";
}
