using System;
using STRINGS;

public class ChoreGroupEnabledRequirement : RoleAssignmentRequirement
{
	public ChoreGroupEnabledRequirement(string choreGroupID)
		: base("ChoreGroupEnabled_" + choreGroupID, string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.CHOREGROUP_ENABLED.DESCRIPTION, Db.Get().ChoreGroups.Get(choreGroupID).Name), (MinionResume resume) => resume.GetComponent<ChoreConsumer>().IsEnabled(Db.Get().ChoreGroups.Get(choreGroupID)))
	{
		this.choreGroupID = choreGroupID;
	}

	public string choreGroupID;
}
