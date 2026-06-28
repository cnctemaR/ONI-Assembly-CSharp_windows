using System;

public class Schedulable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Schedule schedule = this.GetSchedule();
		Schedule schedule2 = schedule;
		schedule2.onChanged = (global::System.Action)Delegate.Combine(schedule2.onChanged, new global::System.Action(this.OnScheduleChanged));
	}

	protected override void OnCleanUp()
	{
		Schedule schedule = this.GetSchedule();
		Schedule schedule2 = schedule;
		schedule2.onChanged = (global::System.Action)Delegate.Remove(schedule2.onChanged, new global::System.Action(this.OnScheduleChanged));
	}

	public Schedule GetSchedule()
	{
		return ScheduleManager.Instance.GetSchedule();
	}

	public bool IsAllowed(ScheduleBlockType schedule_block_type)
	{
		return RedAlertManager.Instance.Get().IsOn() || ScheduleManager.Instance.IsAllowed(schedule_block_type);
	}

	private void OnScheduleChanged()
	{
		this.Trigger(467134493, null);
	}
}
