using System;
using System.Collections.Generic;
using UnityEngine;

public class SchedulerLog
{
	public IEnumerator<SchedulerLog.Entry> GetEnumerator()
	{
		return this.entries.GetEnumerator();
	}

	public int Count
	{
		get
		{
			return this.entries.Count;
		}
	}

	public void Log(SchedulerEntry scheduler_entry)
	{
		SchedulerLog.Entry entry = new SchedulerLog.Entry(scheduler_entry.name, Time.frameCount);
		this.entries.Insert(0, entry);
		if (this.entries.Count > 100)
		{
			this.entries.RemoveAt(this.entries.Count - 1);
		}
	}

	private List<SchedulerLog.Entry> entries = new List<SchedulerLog.Entry>();

	public struct Entry
	{
		public Entry(string name, int frame)
		{
			this.name = name;
			this.frame = frame;
		}

		public override string ToString()
		{
			return this.frame + ": " + this.name;
		}

		public string name;

		public int frame;
	}
}
