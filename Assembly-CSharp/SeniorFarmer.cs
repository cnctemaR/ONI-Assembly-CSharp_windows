using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class SeniorFarmer : RoleConfig
{
	public SeniorFarmer()
	{
		base.id = "SeniorFarmer";
		base.name = DUPLICANTS.ROLES.SENIOR_FARMER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("SeniorFarmer");
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Farming);
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Harvest,
			Db.Get().ChoreTypes.CropTend,
			Db.Get().ChoreTypes.FlipCompost,
			Db.Get().ChoreTypes.Uproot
		};
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Farming);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Botanist };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseBotanyLarge,
			RoleManager.rolePerks.CanFarmTinker
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Farmer
		};
	}

	public const string ID = "SeniorFarmer";
}
