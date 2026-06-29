using System;
using Klei.AI;
using STRINGS;

public class JuniorBuilder : RoleConfig
{
	public JuniorBuilder()
	{
		base.id = JuniorBuilder.ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(JuniorBuilder.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Construction };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseConstructionSmall };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Build };
	}

	public static string ID = "JuniorBuilder";
}
