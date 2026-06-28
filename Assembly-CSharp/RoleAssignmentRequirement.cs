using System;

public class RoleAssignmentRequirement
{
	public RoleAssignmentRequirement(string id, string description, Func<MinionResume, bool> isSatisfied)
	{
		this.id = id;
		this.description = description;
		this.isSatisfied = isSatisfied;
	}

	public string id { get; protected set; }

	public Func<MinionResume, bool> isSatisfied { get; protected set; }

	public virtual string GetDescription()
	{
		return this.description;
	}

	private string description;
}
