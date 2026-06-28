using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Researcher : RoleConfig
{
	public Researcher()
	{
		base.id = Researcher.ID;
		base.name = DUPLICANTS.ROLES.RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(Researcher.ID);
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Research);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Learning };
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Research };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseLearningMedium,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorResearcher
		};
	}

	public static string ID = "Researcher";
}
