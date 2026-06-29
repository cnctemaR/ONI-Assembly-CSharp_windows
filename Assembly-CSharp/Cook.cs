using System;
using Klei.AI;
using STRINGS;

public class Cook : RoleConfig
{
	public Cook()
	{
		base.id = Cook.ID;
		base.name = DUPLICANTS.ROLES.COOK.NAME;
		base.description = DUPLICANTS.ROLES.COOK.DESCRIPTION;
		base.roleGroup = "Cooking";
		base.hat = Game.Instance.roleManager.GetHat(Cook.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Cooking };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseCookingMedium,
			RoleManager.rolePerks.CanElectricGrill
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Cook,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorCook
		};
	}

	public static string ID = "Cook";
}
