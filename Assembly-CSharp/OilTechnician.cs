using System;
using Klei.AI;
using STRINGS;

public class OilTechnician : RoleConfig
{
	public OilTechnician()
	{
		base.id = "OilTechnician";
		base.name = DUPLICANTS.ROLES.OIL_TECHNICIAN.NAME;
		base.description = DUPLICANTS.ROLES.OIL_TECHNICIAN.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat("OilTechnician");
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[] { RoleManager.rolePerks.ExosuitExpertise };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MachineTechnician
		};
	}

	public const string ID = "OilTechnician";
}
