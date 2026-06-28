using System;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : IScheduler
{
	public Scheduler(SchedulerClock clock)
	{
		this.clock = clock;
	}

	public int Count
	{
		get
		{
			return this.entries.Count;
		}
	}

	public float GetTime()
	{
		return this.clock.GetTime();
	}

	private SchedulerHandle Schedule(SchedulerEntry entry)
	{
		this.entries.Enqueue(entry.time, entry);
		SchedulerHandle schedulerHandle = new SchedulerHandle(this, entry);
		return schedulerHandle;
	}

	private SchedulerHandle Schedule(string name, float time, float time_interval, Action<object> callback, object callback_data, GameObject profiler_obj)
	{
		SchedulerEntry schedulerEntry = new SchedulerEntry(name, time + this.clock.GetTime(), time_interval, callback, callback_data, profiler_obj);
		return this.Schedule(schedulerEntry);
	}

	public void FreeResources()
	{
		this.log = null;
		this.addRemovelog = null;
		this.clock = null;
		if (this.entries != null)
		{
			while (this.entries.Count > 0)
			{
				this.entries.Dequeue().Value.FreeResources();
			}
		}
		this.entries = null;
	}

	public SchedulerHandle SchedulePeriodic(string name, float interval, Action<object> callback, object callback_data = null, SchedulerGroup group = null, float time_offset = 0f, GameObject profiler_obj = null)
	{
		if (group != null && group.scheduler != this)
		{
			global::Debug.LogError("Scheduler group mismatch!", null);
		}
		SchedulerHandle schedulerHandle = this.Schedule(name, interval + time_offset, interval, callback, callback_data, profiler_obj);
		if (group != null)
		{
			group.Add(schedulerHandle);
		}
		return schedulerHandle;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		if (group != null && group.scheduler != this)
		{
			global::Debug.LogError("Scheduler group mismatch!", null);
		}
		SchedulerHandle schedulerHandle = this.Schedule(name, time, -1f, callback, callback_data, null);
		if (group != null)
		{
			group.Add(schedulerHandle);
		}
		return schedulerHandle;
	}

	public void Clear(SchedulerHandle handle)
	{
		handle.entry.Clear();
	}

	public void Update()
	{
		if (this.Count == 0)
		{
			return;
		}
		int count = this.Count;
		int i = 0;
		using (new KProfiler.Region("Scheduler.Update", null))
		{
			float time = this.clock.GetTime();
			if (this.previousTime != time)
			{
				this.previousTime = time;
				this.entriesToRun.Clear();
				while (i < count)
				{
					if (time < this.entries.Peek().Key)
					{
						break;
					}
					SchedulerEntry value = this.entries.Dequeue().Value;
					if (value.callback != null)
					{
						this.entriesToRun.Add(value);
					}
					i++;
				}
				for (int j = 0; j < this.entriesToRun.Count; j++)
				{
					SchedulerEntry schedulerEntry = this.entriesToRun[j];
					if (schedulerEntry.callback != null)
					{
						schedulerEntry.callback(schedulerEntry.callbackData);
						if (schedulerEntry.timeInterval >= 0f && schedulerEntry.callback != null)
						{
							SchedulerEntry schedulerEntry2 = schedulerEntry;
							schedulerEntry2.time = this.clock.GetTime() + schedulerEntry2.timeInterval;
							this.Schedule(schedulerEntry2);
						}
					}
				}
			}
		}
	}

	public global::Logger GetAddRemoveLog()
	{
		return this.addRemovelog;
	}

	public global::Logger GetLog()
	{
		return this.log;
	}

	public FloatHOTQueue<SchedulerEntry> entries = new FloatHOTQueue<SchedulerEntry>();

	private LoggerFSSF log = new LoggerFSSF("Scheduler");

	private LoggerFSSF addRemovelog = new LoggerFSSF("Scheduler");

	private SchedulerClock clock;

	private float previousTime = float.NegativeInfinity;

	private List<SchedulerEntry> entriesToRun = new List<SchedulerEntry>();
}
