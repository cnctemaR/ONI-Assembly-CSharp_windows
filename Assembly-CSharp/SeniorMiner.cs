using System;
using Klei.AI;
using STRINGS;

public class SeniorMiner : RoleConfig
{
	public SeniorMiner()
	{
		base.id = SeniorMiner.ID;
		base.name = DUPLICANTS.ROLES.SENIOR_MINER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION;
		base.roleGroup = "Mining";
		base.hat = Game.Instance.roleManager.GetHat(SeniorMiner.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Digging };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseDigSpeedLarge,
			RoleManager.rolePerks.CanDigVeryFirm,
			RoleManager.rolePerks.CanDigNearlyImpenetrable
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Dig,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Miner
		};
	}

	public static string ID = "SeniorMiner ";
}
