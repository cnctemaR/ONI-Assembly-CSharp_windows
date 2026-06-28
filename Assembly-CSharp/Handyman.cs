using System;
using System.Collections.Generic;
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
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Mop,
			Db.Get().ChoreTypes.Repair,
			Db.Get().ChoreTypes.Disinfect,
			Db.Get().ChoreTypes.CleanToilet,
			Db.Get().ChoreTypes.Toggle,
			Db.Get().ChoreTypes.Transport,
			Db.Get().ChoreTypes.FlipCompost
		};
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseStrengthSmall };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Basekeep,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_NoRole
		};
	}

	public static string ID = "Handyman";
}
