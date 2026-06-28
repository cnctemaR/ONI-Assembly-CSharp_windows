using System;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : IScheduler
{
	public Scheduler(SchedulerClock clock)
	{
		this.clock = clock;
	}

	public int entryCount { get; private set; }

	public int Count
	{
		get
		{
			return this.entries.Length;
		}
	}

	private SchedulerHandle Schedule(string name, float time, float time_interval, Action<object> callback, object callback_data, Guid id)
	{
		SchedulerEntry schedulerEntry = new SchedulerEntry(id, name, time + this.clock.GetTime(), time_interval, callback, callback_data);
		if (this.entryCount == this.entries.Length)
		{
			SchedulerEntry[] array = new SchedulerEntry[this.entries.Length * 2];
			Array.Copy(this.entries, array, this.entryCount);
			this.entries = array;
		}
		this.entries[this.entryCount++] = schedulerEntry;
		this.dirty = true;
		SchedulerHandle schedulerHandle = new SchedulerHandle(this, schedulerEntry);
		return schedulerHandle;
	}

	public SchedulerHandle SchedulePeriodic(string name, float interval, Action<object> callback, object callback_data = null, SchedulerGroup group = null, float time_offset = 0f)
	{
		if (group != null && group.scheduler != this)
		{
			Debug.LogError("Scheduler group mismatch!");
		}
		Guid nextId = this.GetNextId();
		SchedulerHandle schedulerHandle = this.Schedule(name, interval + time_offset, interval, callback, callback_data, nextId);
		if (group != null)
		{
			group.Add(nextId);
		}
		return schedulerHandle;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		if (group != null && group.scheduler != this)
		{
			Debug.LogError("Scheduler group mismatch!");
		}
		Guid nextId = this.GetNextId();
		SchedulerHandle schedulerHandle = this.Schedule(name, time, -1f, callback, callback_data, nextId);
		if (group != null)
		{
			group.Add(nextId);
		}
		return schedulerHandle;
	}

	public Guid GetNextId()
	{
		return Guid.NewGuid();
	}

	public void Clear(Guid id)
	{
		if (id == Guid.Empty)
		{
			return;
		}
		for (int i = 0; i < this.entryCount; i++)
		{
			if (this.entries[i].id == id)
			{
				this.entries[i] = new SchedulerEntry(this.entries[i].id, this.entries[i].name, 0f, -1f, null, null);
			}
		}
		SystemScheduler.instance.RemoveTask(id);
	}

	public void Clear(SchedulerHandle handle)
	{
		this.Clear(handle.entry.id);
	}

	public void Update()
	{
		if (this.entryCount == 0)
		{
			return;
		}
		if (this.dirty)
		{
			this.dirty = false;
			Array.Sort<SchedulerEntry>(this.entries, 0, this.entryCount, Scheduler.comparer);
		}
		float time = this.clock.GetTime();
		int entryCount = this.entryCount;
		int i;
		for (i = 0; i < entryCount; i++)
		{
			SchedulerEntry schedulerEntry = this.entries[i];
			if (time < schedulerEntry.time)
			{
				break;
			}
			if (schedulerEntry.callback != null)
			{
				SystemScheduler.instance.AddTask(schedulerEntry.id, SystemScheduler.Priority.Default, schedulerEntry.callback, schedulerEntry.callbackData, schedulerEntry.name);
				if (this.entries[i].timeInterval >= 0f)
				{
					this.Schedule(schedulerEntry.name, schedulerEntry.timeInterval, schedulerEntry.timeInterval, schedulerEntry.callback, schedulerEntry.callbackData, schedulerEntry.id);
				}
			}
		}
		this.entryCount -= i;
		Array.Copy(this.entries, i, this.entries, 0, this.entryCount);
	}

	public global::Logger GetAddRemoveLog()
	{
		return this.addRemovelog;
	}

	public global::Logger GetLog()
	{
		return this.log;
	}

	public SchedulerEntry[] entries = new SchedulerEntry[1024];

	private LoggerFSSF log = new LoggerFSSF("Scheduler");

	private LoggerFSSF addRemovelog = new LoggerFSSF("Scheduler");

	private SchedulerClock clock;

	private bool dirty;

	private static Scheduler.EntryComparer comparer = new Scheduler.EntryComparer();

	private class EntryComparer : IComparer<SchedulerEntry>
	{
		public int Compare(SchedulerEntry a, SchedulerEntry b)
		{
			if (a.time < b.time)
			{
				return -1;
			}
			if (a.time > b.time)
			{
				return 1;
			}
			return 0;
		}
	}
}
