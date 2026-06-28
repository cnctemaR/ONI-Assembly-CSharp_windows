using System;
using STRINGS;

public class PreviousRoleAssignmentRequirement : RoleAssignmentRequirement
{
	public PreviousRoleAssignmentRequirement(string previousRoleID)
		: base("HasExperience_" + previousRoleID, UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_EXPERIENCE.DESCRIPTION, (MinionResume resume) => previousRoleID == "NoRole" || resume.ExperienceByRoleID[previousRoleID] >= Game.Instance.roleManager.GetRole(previousRoleID).experienceRequired)
	{
		this.previousRoleID = previousRoleID;
	}

	public override string GetDescription()
	{
		return base.GetDescription().Replace("{0}", Game.Instance.roleManager.GetRole(this.previousRoleID).name);
	}

	public string previousRoleID;
}
