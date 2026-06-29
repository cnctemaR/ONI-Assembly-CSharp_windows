using System;
using Klei.AI;
using STRINGS;

public class MachineTechnician : RoleConfig
{
	public MachineTechnician()
	{
		base.id = MachineTechnician.ID;
		base.name = DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME;
		base.description = DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(MachineTechnician.ID);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseMachineryMedium };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate };
	}

	public static string ID = "MachineTechnician";
}
