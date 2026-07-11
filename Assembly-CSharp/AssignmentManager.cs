using System;
using System.Collections.Generic;
using STRINGS;

public class AssignmentManager : KMonoBehaviour
{
	public IEnumerator<Assignable> GetEnumerator()
	{
		return this.assignables.GetEnumerator();
	}

	public void Add(Assignable assignable)
	{
		this.assignables.Add(assignable);
	}

	public void Remove(Assignable assignable)
	{
		this.assignables.Remove(assignable);
	}

	public void AddAssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		if (this.assignment_groups.ContainsKey(id))
		{
			return;
		}
		this.assignment_groups.Add(id, new AssignmentGroup(id, members, name));
	}

	public void AddToAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		this.assignment_groups[group_id].AddMember(member);
	}

	public void RemoveFromAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		this.assignment_groups[group_id].RemoveMember(member);
	}

	public void RemoveFromAllGroups(IAssignableIdentity member)
	{
		foreach (Assignable assignable in this.assignables)
		{
			if (assignable.assignee == member)
			{
				assignable.Unassign();
			}
		}
		foreach (KeyValuePair<string, AssignmentGroup> keyValuePair in this.assignment_groups)
		{
			if (keyValuePair.Value.HasMember(member))
			{
				keyValuePair.Value.RemoveMember(member);
			}
		}
	}

	public List<Assignable> GetPreferredAssignables(Assignables owner, AssignableSlot slot)
	{
		this.PreferredAssignableResults.Clear();
		int num = int.MaxValue;
		foreach (Assignable assignable in this.assignables)
		{
			if (assignable.slot == slot)
			{
				if (assignable.assignee != null)
				{
					List<Ownables> owners = assignable.assignee.GetOwners();
					if (owners.Count > 0)
					{
						foreach (Ownables ownables in owners)
						{
							if (ownables.gameObject == owner.gameObject)
							{
								if (assignable.assignee is Room && (assignable.assignee as Room).roomType.priority_building_use)
								{
									this.PreferredAssignableResults.Clear();
									this.PreferredAssignableResults.Add(assignable);
									return this.PreferredAssignableResults;
								}
								if (owners.Count == num)
								{
									this.PreferredAssignableResults.Add(assignable);
								}
								else if (owners.Count < num)
								{
									num = owners.Count;
									this.PreferredAssignableResults.Clear();
									this.PreferredAssignableResults.Add(assignable);
								}
							}
						}
					}
				}
			}
		}
		return this.PreferredAssignableResults;
	}

	private List<Assignable> assignables = new List<Assignable>();

	public Dictionary<string, AssignmentGroup> assignment_groups = new Dictionary<string, AssignmentGroup> { 
	{
		"public",
		new AssignmentGroup("public", new IAssignableIdentity[0], UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.PUBLIC)
	} };

	private List<Assignable> PreferredAssignableResults = new List<Assignable>();
}
