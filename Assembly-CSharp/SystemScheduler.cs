using System;
using System.Collections.Generic;
using System.Diagnostics;

public class SystemScheduler
{
	private SystemScheduler()
	{
		this.timer = new Stopwatch();
		this.timer.Start();
	}

	public static void Initialize()
	{
		SystemScheduler.instance = new SystemScheduler();
		for (int i = 0; i < 3; i++)
		{
			SystemScheduler.instance.prioritizedEntries[i] = new List<SystemScheduler.Entry>();
		}
	}

	public static void Shutdown()
	{
		SystemScheduler.instance = null;
	}

	public void Update()
	{
		long elapsedMilliseconds = this.timer.ElapsedMilliseconds;
		long num;
		if (elapsedMilliseconds <= 16L)
		{
			num = Math.Min(8L, 16L - elapsedMilliseconds);
		}
		else
		{
			num = Math.Min(16L, 33L - elapsedMilliseconds);
		}
		num = Math.Max(3L, num);
		this.timer.Reset();
		for (int i = 0; i < 3; i++)
		{
			List<SystemScheduler.Entry> list = this.prioritizedEntries[i];
			if (list.Count > 0)
			{
				int j;
				for (j = 0; j < list.Count; j++)
				{
					SystemScheduler.Entry entry = list[j];
					if (entry.details.callback != null)
					{
						entry.details.callback(entry.details.callbackData);
					}
					if (this.timer.ElapsedMilliseconds >= num)
					{
						break;
					}
				}
				list.RemoveRange(0, j);
				if (this.timer.ElapsedMilliseconds >= num)
				{
					break;
				}
			}
		}
		this.timer.Reset();
	}

	public Guid AddTask(SystemScheduler.Priority priority, SchedulerEntry.Details details)
	{
		List<SystemScheduler.Entry> list = this.prioritizedEntries[(int)priority];
		list.Add(new SystemScheduler.Entry
		{
			details = details
		});
		return details.id;
	}

	public void Clear()
	{
		for (int i = 0; i < this.prioritizedEntries.Length; i++)
		{
			this.prioritizedEntries[i].Clear();
		}
	}

	public static SystemScheduler instance;

	private List<SystemScheduler.Entry>[] prioritizedEntries = new List<SystemScheduler.Entry>[3];

	private Stopwatch timer;

	public enum Priority
	{
		Highest,
		Default,
		Lowest,
		Count
	}

	[DebuggerDisplay("{data}, {name}, {guid}")]
	private struct Entry
	{
		public SchedulerEntry.Details details;
	}
}
