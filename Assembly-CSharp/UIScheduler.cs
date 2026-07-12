using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UIScheduler")]
public class UIScheduler : KMonoBehaviour, IScheduler
{
	public static void DestroyInstance()
	{
		UIScheduler.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		UIScheduler.Instance = this;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, time, callback, callback_data, group);
	}

	public SchedulerHandle ScheduleNextFrame(string name, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, 0f, callback, callback_data, group);
	}

	private void Update()
	{
		this.scheduler.Update();
	}

	protected override void OnLoadLevel()
	{
		this.scheduler.FreeResources();
		this.scheduler = null;
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
