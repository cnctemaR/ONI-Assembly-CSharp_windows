using System;
using Klei.AI;
using STRINGS;

public class SeniorRancher : RoleConfig
{
	public SeniorRancher()
	{
		base.id = "SeniorRancher";
		base.name = DUPLICANTS.ROLES.SENIOR_RANCHER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION;
		base.roleGroup = "Ranching";
		base.hat = Game.Instance.roleManager.GetHat("SeniorRancher");
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Ranching };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanWrangleCreatures,
			RoleManager.rolePerks.CanUseRanchStation,
			RoleManager.rolePerks.IncreaseRanchingMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Ranching,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Rancher
		};
	}

	public const string ID = "SeniorRancher";
}
