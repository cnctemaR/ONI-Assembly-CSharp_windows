using System;

public class Schedulable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		ScheduleManager instance = ScheduleManager.Instance;
		instance.onSheduleBlocksChanged += this.OnScheduleBlocksChanged;
	}

	protected override void OnCleanUp()
	{
		ScheduleManager instance = ScheduleManager.Instance;
		instance.onSheduleBlocksChanged -= this.OnScheduleBlocksChanged;
	}

	public Schedule GetSchedule()
	{
		return ScheduleManager.Instance.GetSchedule(this);
	}

	public bool IsAllowed(ScheduleBlockType schedule_block_type)
	{
		return RedAlertManager.Instance.Get().IsOn() || ScheduleManager.Instance.IsAllowed(this, schedule_block_type);
	}

	public void OnScheduleChanged(Schedule schedule)
	{
		base.Trigger(467134493, schedule);
	}

	private void OnScheduleBlocksChanged()
	{
		base.Trigger(-894023145, null);
	}
}
