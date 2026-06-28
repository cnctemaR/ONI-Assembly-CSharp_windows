using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Logger<EntryType> : Logger
{
	public Logger(string name, int new_max = 35)
		: base(name)
	{
		this.SetMaxEntries(new_max);
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
			int num;
			if (this.entries == null)
			{
				num = 0;
			}
			else
			{
				num = this.entries.Count;
			}
			return num;
		}
	}

	public void SetMaxEntries(int new_max)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
	}

	private List<EntryType> entries;

	public Action<EntryType> OnLog;
}
