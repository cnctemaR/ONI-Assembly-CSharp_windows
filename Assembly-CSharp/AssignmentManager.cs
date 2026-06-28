using System;
using System.Collections.Generic;

public class AssignmentManager
{
	public IEnumerator<Assignable> GetEnumerator()
	{
		return this.assignables.GetEnumerator();
	}

	public static AssignmentManager Instance
	{
		get
		{
			return Singleton<AssignmentManager>.Instance;
		}
	}

	public void Add(Assignable assignable)
	{
		this.assignables.Add(assignable);
	}

	public void Remove(Assignable assignable)
	{
		this.assignables.Remove(assignable);
	}

	private List<Assignable> assignables = new List<Assignable>();
}
