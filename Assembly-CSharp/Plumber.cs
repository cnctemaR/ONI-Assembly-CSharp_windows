using System;
using Klei.AI;
using STRINGS;

public class Plumber : RoleConfig
{
	public Plumber()
	{
		base.id = Plumber.ID;
		base.name = DUPLICANTS.ROLES.PLUMBER.NAME;
		base.description = DUPLICANTS.ROLES.PLUMBER.DESCRIPTION;
		base.roleGroup = "Basekeeping";
		base.hat = Game.Instance.roleManager.GetHat(Plumber.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseStrengthSmall,
			RoleManager.rolePerks.CanDoPlumbing
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Basekeep,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Handyman
		};
	}

	public static string ID = "Plumber";
}
