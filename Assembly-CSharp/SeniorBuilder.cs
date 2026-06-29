using System;
using Klei.AI;
using STRINGS;

public class SeniorBuilder : RoleConfig
{
	public SeniorBuilder()
	{
		base.id = SeniorBuilder.ID;
		base.name = DUPLICANTS.ROLES.SENIOR_BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(SeniorBuilder.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Construction };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseConstructionLarge };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Build,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Builder
		};
	}

	public static string ID = "SeniorBuilder";
}
