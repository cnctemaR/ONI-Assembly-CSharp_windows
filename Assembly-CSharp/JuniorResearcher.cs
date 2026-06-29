using System;
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
		this.relevantAttributes = new Klei.AI.Attribute[] { Db.Get().Attributes.Learning };
		base.perks = new RolePerk[]
		{
			RoleManager.rolePerks.IncreaseLearningSmall,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[] { Game.Instance.roleManager.roleAssignmentRequirements.Can_Research };
	}

	public static string ID = "JuniorResearcher";
}
