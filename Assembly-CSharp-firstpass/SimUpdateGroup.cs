using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{Name}")]
public class SimUpdateGroup
{
	public SimUpdateGroup(string name)
	{
		this.Name = name;
	}

	public void Add(SimUpdateArray sim_update_array)
	{
		if (sim_update_array != null)
		{
			this.UpdateArrays.Add(sim_update_array);
			this.Dirty = true;
		}
	}

	public void Update(float dt)
	{
		if (this.Dirty)
		{
			this.UpdateArrays.Sort(new Comparison<SimUpdateArray>(SimUpdateGroup.UpdateArraySorter));
			this.Dirty = false;
		}
		int count = this.UpdateArrays.Count;
		for (int i = 0; i < count; i++)
		{
			this.UpdateArrays[i].Update(dt);
		}
	}

	private static int UpdateArraySorter(SimUpdateArray a, SimUpdateArray b)
	{
		int num = a.SortKey.CompareTo(b.SortKey);
		if (num == 0)
		{
			num = a.NameHash.CompareTo(b.NameHash);
		}
		return num;
	}

	private string Name;

	private List<SimUpdateArray> UpdateArrays = new List<SimUpdateArray>();

	private bool Dirty;
}
