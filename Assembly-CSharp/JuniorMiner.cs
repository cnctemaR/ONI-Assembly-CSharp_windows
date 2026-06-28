using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class JuniorMiner : RoleConfig
{
	public JuniorMiner()
	{
		base.id = JuniorMiner.ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_MINER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION;
		base.roleGroup = "Mining";
		base.hat = Game.Instance.roleManager.GetHat(JuniorMiner.ID);
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Dig };
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Digging };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseDigSpeedSmall,
			RoleManager.rolePerks.CanDigVeryFirm
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Dig };
	}

	public static string ID = "JuniorMiner";
}
