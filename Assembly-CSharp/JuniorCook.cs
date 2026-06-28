using System;
using System.Collections.Generic;
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
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Cooking);
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Mush,
			Db.Get().ChoreTypes.Cook
		};
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Cooking };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseCookingSmall,
			RoleManager.rolePerks.CanElectricGrill
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Cook,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_NoRole
		};
	}

	public static string ID = "JuniorCook";
}
