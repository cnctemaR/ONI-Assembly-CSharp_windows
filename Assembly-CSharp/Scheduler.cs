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

	public float GetTime()
	{
		return this.clock.GetTime();
	}

	private SchedulerHandle Schedule(SchedulerEntry entry)
	{
		if (this.entryCount == this.entries.Length)
		{
			SchedulerEntry[] array = new SchedulerEntry[this.entries.Length * 2];
			Array.Copy(this.entries, array, this.entryCount);
			this.entries = array;
		}
		this.entries[this.entryCount++] = entry;
		this.dirty = true;
		SchedulerHandle schedulerHandle = new SchedulerHandle(this, entry);
		return schedulerHandle;
	}

	private SchedulerHandle Schedule(string name, float time, float time_interval, Action<object> callback, object callback_data, Guid id, GameObject profiler_obj)
	{
		SchedulerEntry schedulerEntry = new SchedulerEntry(id, name, time + this.clock.GetTime(), time_interval, callback, callback_data, profiler_obj);
		return this.Schedule(schedulerEntry);
	}

	public SchedulerHandle SchedulePeriodic(string name, float interval, Action<object> callback, object callback_data = null, SchedulerGroup group = null, float time_offset = 0f, GameObject profiler_obj = null)
	{
		if (group != null && group.scheduler != this)
		{
			global::Debug.LogError("Scheduler group mismatch!", null);
		}
		Guid nextId = this.GetNextId();
		SchedulerHandle schedulerHandle = this.Schedule(name, interval + time_offset, interval, callback, callback_data, nextId, profiler_obj);
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
			global::Debug.LogError("Scheduler group mismatch!", null);
		}
		Guid nextId = this.GetNextId();
		SchedulerHandle schedulerHandle = this.Schedule(name, time, -1f, callback, callback_data, nextId, null);
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
				string text = null;
				GameObject gameObject = null;
				this.entries[i] = new SchedulerEntry(this.entries[i].id, text, 0f, -1f, null, null, gameObject);
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
		int entryCount = this.entryCount;
		int i = 0;
		using (new KProfiler.Region("Scheduler.Update", null))
		{
			float time = this.clock.GetTime();
			if (this.previousTime == time)
			{
				return;
			}
			this.previousTime = time;
			while (i < entryCount)
			{
				SchedulerEntry schedulerEntry = this.entries[i];
				if (time < schedulerEntry.time)
				{
					break;
				}
				if (schedulerEntry.callback != null)
				{
					string text = null;
					GameObject gameObject = null;
					SystemScheduler.instance.AddTask(schedulerEntry.id, SystemScheduler.Priority.Default, schedulerEntry.callback, schedulerEntry.callbackData, text, gameObject);
					if (this.entries[i].timeInterval >= 0f)
					{
						SchedulerEntry schedulerEntry2 = this.entries[i];
						schedulerEntry2.time = this.clock.GetTime() + schedulerEntry2.timeInterval;
						this.Schedule(schedulerEntry2);
					}
				}
				i++;
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

	private float previousTime = float.NegativeInfinity;

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
