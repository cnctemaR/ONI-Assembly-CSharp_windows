using System;
using UnityEngine;

public struct SchedulerEntry
{
	public SchedulerEntry.Details details { readonly get; private set; }

	public SchedulerEntry(string name, float time, float time_interval, Action<object> callback, object callback_data, GameObject profiler_obj)
	{
		this.time = time;
		this.details = new SchedulerEntry.Details(name, callback, callback_data, time_interval, profiler_obj);
	}

	public void FreeResources()
	{
		this.details = null;
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
		return this.time.ToString();
	}

	public void Clear()
	{
		this.details.callback = null;
	}

	public float time;

	public class Details
	{
		public Details(string name, Action<object> callback, object callback_data, float time_interval, GameObject profiler_obj)
		{
			this.timeInterval = time_interval;
			this.callback = callback;
			this.callbackData = callback_data;
		}

		public Action<object> callback;

		public object callbackData;

		public float timeInterval;
	}
}
