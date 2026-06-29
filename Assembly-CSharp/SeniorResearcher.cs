using System;
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
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Learning };
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
