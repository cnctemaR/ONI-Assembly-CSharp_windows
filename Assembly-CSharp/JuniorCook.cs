using System;
using Klei.AI;
using STRINGS;

public class JuniorCook : RoleConfig
{
	public JuniorCook()
	{
		base.id = JuniorCook.ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_COOK.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION;
		base.roleGroup = "Cooking";
		base.hat = Game.Instance.roleManager.GetHat(JuniorCook.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Cooking };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseCookingSmall,
			RoleManager.rolePerks.CanElectricGrill
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Cook };
	}

	public static string ID = "JuniorCook";
}
