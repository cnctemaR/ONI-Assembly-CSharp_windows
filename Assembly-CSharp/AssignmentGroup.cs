using System;
using System.Collections.Generic;

public class AssignmentGroup : IAssignableIdentity
{
	public AssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		this.id = id;
		this.name = name;
		foreach (IAssignableIdentity assignableIdentity in members)
		{
			this.members.Add(assignableIdentity);
		}
	}

	public string id { get; private set; }

	public string name { get; private set; }

	public void AddMember(IAssignableIdentity member)
	{
		if (!this.members.Contains(member))
		{
			this.members.Add(member);
		}
	}

	public void RemoveMember(IAssignableIdentity member)
	{
		this.members.Remove(member);
	}

	public string GetProperName()
	{
		return this.name;
	}

	public bool HasMember(IAssignableIdentity member)
	{
		return this.members.Contains(member);
	}

	public bool IsNull()
	{
		return false;
	}

	public List<Ownables> GetOwners()
	{
		this.current_owners.Clear();
		foreach (IAssignableIdentity assignableIdentity in this.members)
		{
			this.current_owners.AddRange(assignableIdentity.GetOwners());
		}
		return this.current_owners;
	}

	public Ownables GetSoleOwner()
	{
		if (this.members.Count == 1)
		{
			return this.members[0] as Ownables;
		}
		Debug.LogWarningFormat("GetSoleOwner called on AssignmentGroup with {0} members", new object[] { this.members.Count });
		return null;
	}

	private List<IAssignableIdentity> members = new List<IAssignableIdentity>();

	public List<Ownables> current_owners = new List<Ownables>();
}
