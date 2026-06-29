using System;
using STRINGS;

public class NoRole : RoleConfig
{
	public NoRole()
	{
		base.id = "NoRole";
		base.name = DUPLICANTS.ROLES.NO_ROLE.NAME;
		base.description = DUPLICANTS.ROLES.NO_ROLE.DESCRIPTION;
		this.experienceRequired = -1f;
		base.perks = new RolePerk[0];
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[0];
	}

	public const string ID = "NoRole";
}
