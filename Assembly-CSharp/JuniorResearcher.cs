using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class JuniorResearcher : RoleConfig
{
	public JuniorResearcher()
	{
		base.id = JuniorResearcher.ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(JuniorResearcher.ID);
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Research);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Learning };
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Research };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseLearningSmall,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_NoRole
		};
	}

	public static string ID = "JuniorResearcher";
}
