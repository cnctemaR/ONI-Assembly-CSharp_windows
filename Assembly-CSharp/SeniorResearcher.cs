using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class SeniorResearcher : RoleConfig
{
	public SeniorResearcher()
	{
		base.id = SeniorResearcher.ID;
		base.name = DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(SeniorResearcher.ID);
		this.preferredChoreTags.Add(GameTags.ChoreTypes.Research);
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Learning };
		this.favoredChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Research };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseLearningLarge,
			RoleManager.rolePerks.AllowAdvancedResearch,
			RoleManager.rolePerks.CanStudyWorldObjects
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Researcher
		};
	}

	public static string ID = "SeniorResearcher";
}
