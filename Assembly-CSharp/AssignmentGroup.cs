using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class AssignmentGroup : IAssignableIdentity
{
	public string id { get; private set; }

	public string name { get; private set; }

	public AssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		this.id = id;
		this.name = name;
		foreach (IAssignableIdentity assignableIdentity in members)
		{
			this.members.Add(assignableIdentity);
		}
		if (Game.Instance != null)
		{
			Game.Instance.assignmentManager.assignment_groups.Add(id, this);
			Game.Instance.Trigger(-1123234494, this);
		}
	}

	public void AddMember(IAssignableIdentity member)
	{
		if (!this.members.Contains(member))
		{
			this.members.Add(member);
		}
		Game.Instance.Trigger(-1123234494, this);
	}

	public void RemoveMember(IAssignableIdentity member)
	{
		this.members.Remove(member);
		Game.Instance.Trigger(-1123234494, this);
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

	public ReadOnlyCollection<IAssignableIdentity> GetMembers()
	{
		return this.members.AsReadOnly();
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

	public bool HasOwner(Assignables owner)
	{
		using (List<IAssignableIdentity>.Enumerator enumerator = this.members.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.HasOwner(owner))
				{
					return true;
				}
			}
		}
		return false;
	}

	public int NumOwners()
	{
		int num = 0;
		foreach (IAssignableIdentity assignableIdentity in this.members)
		{
			num += assignableIdentity.NumOwners();
		}
		return num;
	}

	private List<IAssignableIdentity> members = new List<IAssignableIdentity>();

	public List<Ownables> current_owners = new List<Ownables>();
}
