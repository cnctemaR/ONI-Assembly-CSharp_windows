using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Hauler : RoleConfig
{
	public Hauler()
	{
		base.id = "Hauler";
		base.name = DUPLICANTS.ROLES.HAULER.NAME;
		base.description = DUPLICANTS.ROLES.HAULER.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat("Hauler");
		this.experienceRequired = 50f;
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Fetch,
			Db.Get().ChoreTypes.OperateFetch,
			Db.Get().ChoreTypes.BuildFetch,
			Db.Get().ChoreTypes.Transport
		};
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Athletics };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseStrengthSmall,
			RoleManager.rolePerks.IncreaseCarryAmountSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul };
	}

	public const string ID = "Hauler";
}
