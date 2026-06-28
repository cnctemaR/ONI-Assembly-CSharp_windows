using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{name}")]
public class SimUpdateArray
{
	public SimUpdateArray(int sort_key, string name)
	{
		this.name = name;
		this.NameHash = name.GetHashCode();
		this.SortKey = sort_key;
	}

	public void Add(KMonoBehaviour cmp, SimUpdateFn update_fn)
	{
		SimUpdateArray.Entry entry = new SimUpdateArray.Entry
		{
			cmp = cmp,
			update = update_fn
		};
		this.entries.Add(entry);
	}

	public void Update(float dt)
	{
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			MonoBehaviour cmp = this.entries[i].cmp;
			if (cmp != null)
			{
				this.entries[i].update(dt);
			}
		}
	}

	public void Remove(KMonoBehaviour cmp)
	{
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.entries[i].cmp == cmp)
			{
				this.entries.RemoveAt(i);
				break;
			}
		}
	}

	public int SortKey;

	public int NameHash;

	private string name;

	private List<SimUpdateArray.Entry> entries = new List<SimUpdateArray.Entry>();

	private struct Entry
	{
		public KMonoBehaviour cmp;

		public SimUpdateFn update;
	}
}
