using System;
using Klei.AI;
using STRINGS;

public class Astronaut : RoleConfig
{
	public Astronaut()
	{
		base.id = Astronaut.ID;
		base.name = DUPLICANTS.ROLES.ASTRONAUT.NAME;
		base.description = DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(Astronaut.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[] { RoleManager.rolePerks.CanUseRockets };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_AstronautTrainee };
	}

	public static string ID = "Astronaut";
}
