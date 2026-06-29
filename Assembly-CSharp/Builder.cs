using System;
using Klei.AI;
using STRINGS;

public class Builder : RoleConfig
{
	public Builder()
	{
		base.id = Builder.ID;
		base.name = DUPLICANTS.ROLES.BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(Builder.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Construction };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseConstructionMedium };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Build,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorBuilder
		};
	}

	public static string ID = "Builder";
}
