using System;
using Klei.AI;
using STRINGS;

public class Rancher : RoleConfig
{
	public Rancher()
	{
		base.id = "Rancher";
		base.name = DUPLICANTS.ROLES.RANCHER.NAME;
		base.description = DUPLICANTS.ROLES.RANCHER.DESCRIPTION;
		base.roleGroup = "Ranching";
		base.hat = Game.Instance.roleManager.GetHat("Rancher");
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Ranching };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.CanWrangleCreatures,
			RoleManager.rolePerks.CanUseRanchStation,
			RoleManager.rolePerks.IncreaseRanchingSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Ranching,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorFarmer
		};
	}

	public const string ID = "Rancher";
}
