using System;
using UnityEngine;

public class UIScheduler : KMonoBehaviour, IScheduler
{
	protected override void OnPrefabInit()
	{
		UIScheduler.Instance = this;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, time, callback, callback_data, group);
	}

	public SchedulerHandle SchedulePeriodic(string name, float interval, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.SchedulePeriodic(name, interval, callback, callback_data, group, 0f);
	}

	private void Update()
	{
		this.scheduler.Update();
	}

	public SchedulerGroup CreateGroup()
	{
		return new SchedulerGroup(this.scheduler);
	}

	public Scheduler GetScheduler()
	{
		return this.scheduler;
	}

	private Scheduler scheduler = new Scheduler(new UIScheduler.UISchedulerClock());

	public static UIScheduler Instance;

	public class UISchedulerClock : SchedulerClock
	{
		public override float GetTime()
		{
			return Time.unscaledTime;
		}
	}
}
