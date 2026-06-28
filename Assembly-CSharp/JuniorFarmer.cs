using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class JuniorFarmer : RoleConfig
{
	public JuniorFarmer()
	{
		base.id = "JuniorFarmer";
		base.name = DUPLICANTS.ROLES.JUNIOR_FARMER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("JuniorFarmer");
		this.favoredChoreTypes = new List<ChoreType>
		{
			Db.Get().ChoreTypes.Harvest,
			Db.Get().ChoreTypes.CropTend,
			Db.Get().ChoreTypes.FlipCompost,
			Db.Get().ChoreTypes.Uproot
		};
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Farming);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Botanist };
		base.perks = new RolePerk[] { RoleManager.rolePerks.IncreaseBotanySmall };
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming };
	}

	public const string ID = "JuniorFarmer";
}
