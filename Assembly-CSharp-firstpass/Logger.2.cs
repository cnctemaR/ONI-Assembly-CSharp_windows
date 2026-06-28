using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Logger<EntryType> : global::Logger
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

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
		if (this.entries == null)
		{
			this.entries = new List<EntryType>();
		}
		if (this.OnLog != null)
		{
			this.OnLog(entry);
		}
		this.entries.Add(entry);
		if (this.entries.Count > this.maxEntries)
		{
			this.entries.RemoveAt(0);
		}
		if (base.enableConsoleLogging)
		{
			global::UnityEngine.Debug.Log(entry.ToString());
		}
		if (base.breakOnLog)
		{
			int num = 0;
			num++;
		}
	}

	private List<EntryType> entries;

	public Action<EntryType> OnLog;

	private int maxEntries = 35;
}
