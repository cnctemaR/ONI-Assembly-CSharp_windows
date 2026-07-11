using System;
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
				while (i < count)
				{
					if (time < this.entries.Peek().Key)
					{
						break;
					}
					SchedulerEntry value = this.entries.Dequeue().Value;
					if (value.callback != null)
					{
						value.callback(value.callbackData);
					}
					i++;
				}
			}
		}
	}

	public FloatHOTQueue<SchedulerEntry> entries = new FloatHOTQueue<SchedulerEntry>();

	private SchedulerClock clock;

	private float previousTime = float.NegativeInfinity;
}
