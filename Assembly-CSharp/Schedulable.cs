using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Schedulable")]
public class Schedulable : KMonoBehaviour
{
	public Schedule GetSchedule()
	{
		return ScheduleManager.Instance.GetSchedule(this);
	}

	public bool IsAllowed(ScheduleBlockType schedule_block_type)
	{
		return VignetteManager.Instance.Get().IsRedAlert() || ScheduleManager.Instance.IsAllowed(this, schedule_block_type);
	}

	public void OnScheduleChanged(Schedule schedule)
	{
		base.Trigger(467134493, schedule);
	}

	public void OnScheduleBlocksTick(Schedule schedule)
	{
		base.Trigger(1714332666, schedule);
	}

	public void OnScheduleBlocksChanged(Schedule schedule)
	{
		base.Trigger(-894023145, schedule);
	}
}
