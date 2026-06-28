using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Logger<EntryType> : Logger
{
	public Logger(string name)
		: base(name)
	{
	}

	public IEnumerator<EntryType> GetEnumerator()
	{
		if (this.entries == null)
		{
			this.entries = new List<EntryType>();
		}
		return this.entries.GetEnumerator();
	}

	public override int Count
	{
		get
		{
			if (this.entries == null)
			{
				return 0;
			}
			return this.entries.Count;
		}
	}

	public void SetMaxEntries(int new_max)
	{
		this.maxEntries = new_max;
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
	}

	private List<EntryType> entries;

	public Action<EntryType> OnLog;

	private int maxEntries = 35;
}
