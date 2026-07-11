using System;
using Klei.AI;
using STRINGS;

public class AstronautTrainee : RoleConfig
{
	public AstronautTrainee()
	{
		base.id = AstronautTrainee.ID;
		base.name = DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME;
		base.description = DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(AstronautTrainee.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[] { RoleManager.rolePerks.CanTrainToBeAstronaut };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_SeniorResearcher,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_SuitExpert,
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate
		};
	}

	public static string ID = "AstronautTrainee";
}
