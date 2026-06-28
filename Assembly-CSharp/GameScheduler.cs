using System;
using UnityEngine;

public class GameScheduler : KMonoBehaviour, IScheduler
{
	protected override void OnPrefabInit()
	{
		GameScheduler.Instance = this;
		StateMachineManager.Instance.RegisterScheduler(this.scheduler);
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, time, callback, callback_data, group);
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

	private Scheduler scheduler = new Scheduler(new GameScheduler.GameSchedulerClock());

	public static GameScheduler Instance;

	public class GameSchedulerClock : SchedulerClock
	{
		public override float GetTime()
		{
			return Time.time;
		}
	}
}
