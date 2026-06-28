using System;
using UnityEngine;

public struct SchedulerEntry
{
	public SchedulerEntry(Guid id, string name, float time, float time_interval, Action<object> callback, object callback_data, GameObject profiler_obj)
	{
		this.time = time;
		this.details = new SchedulerEntry.Details(id, name, callback, callback_data, time_interval, profiler_obj);
	}

	public Guid id
	{
		get
		{
			return this.details.id;
		}
		set
		{
			this.details.id = value;
		}
	}

	public Action<object> callback
	{
		get
		{
			return this.details.callback;
		}
	}

	public object callbackData
	{
		get
		{
			return this.details.callbackData;
		}
	}

	public float timeInterval
	{
		get
		{
			return this.details.timeInterval;
		}
	}

	public override string ToString()
	{
		return this.time + ": id = " + this.details.id.ToString();
	}

	public float time;

	private SchedulerEntry.Details details;

	private class Details
	{
		public Details(Guid id, string name, Action<object> callback, object callback_data, float time_interval, GameObject profiler_obj)
		{
			this.id = id;
			this.timeInterval = time_interval;
			this.callback = callback;
			this.callbackData = callback_data;
		}

		public Guid id;

		public Action<object> callback;

		public object callbackData;

		public float timeInterval;
	}
}
