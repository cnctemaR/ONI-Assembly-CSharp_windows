using System;
using Klei.AI;
using STRINGS;

public class Handyman : RoleConfig
{
	public Handyman()
	{
		base.id = Handyman.ID;
		base.name = DUPLICANTS.ROLES.HANDYMAN.NAME;
		base.description = DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION;
		base.roleGroup = "Basekeeping";
		base.hat = Game.Instance.roleManager.GetHat(Handyman.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseStrengthGroundskeeper };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Basekeep };
	}

	public static string ID = "Handyman";
}
