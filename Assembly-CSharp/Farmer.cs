using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Farmer : RoleConfig
{
	public Farmer()
	{
		base.id = "Farmer";
		base.name = DUPLICANTS.ROLES.FARMER.NAME;
		base.description = DUPLICANTS.ROLES.FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("Farmer");
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Farming);
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Harvest,
			Db.Get().ChoreTypes.CropTend,
			Db.Get().ChoreTypes.FlipCompost,
			Db.Get().ChoreTypes.Uproot
		};
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Botanist };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseBotanyMedium,
			RoleManager.rolePerks.CanFarmTinker
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorFarmer
		};
	}

	public const string ID = "Farmer";
}
