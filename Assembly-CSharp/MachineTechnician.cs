using System;
using System.Collections.Generic;
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
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Fabricate,
			Db.Get().ChoreTypes.GeneratePower,
			Db.Get().ChoreTypes.LiquidCooledFan,
			Db.Get().ChoreTypes.ScrubOre,
			Db.Get().ChoreTypes.Toggle
		};
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Fabricating);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Machinery };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseMachineryMedium };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_NoRole
		};
	}

	public static string ID = "MachineTechnician";
}
