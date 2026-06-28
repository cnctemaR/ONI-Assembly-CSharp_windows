using System;
using System.Runtime.Serialization;
using KSerialization;

public class ScheduleManager : KMonoBehaviour
{
	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.schedule != null && (this.schedule.GetBlocks() == null || this.schedule.GetBlocks().Length != 24))
		{
			this.schedule = null;
		}
		if (this.schedule == null)
		{
			this.SetupDefaultSchedule();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ScheduleManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		if (this.schedule == null)
		{
			this.SetupDefaultSchedule();
		}
	}

	private void EnableAll(Schedule schedule, int idx)
	{
		foreach (ScheduleBlockType scheduleBlockType in Db.Get().ScheduleBlockTypes)
		{
			schedule.Add(idx, scheduleBlockType);
		}
	}

	private void DisableAll(Schedule schedule, int idx)
	{
		foreach (ScheduleBlockType scheduleBlockType in Db.Get().ScheduleBlockTypes)
		{
			schedule.Remove(idx, scheduleBlockType);
		}
	}

	private void SetupDefaultSchedule()
	{
		this.schedule = new Schedule(24);
		for (int i = 0; i < 21; i++)
		{
			this.EnableAll(this.schedule, i);
			this.schedule.Remove(i, Db.Get().ScheduleBlockTypes.Sleep);
		}
		for (int j = 21; j < 24; j++)
		{
			this.schedule.Add(j, Db.Get().ScheduleBlockTypes.Sleep);
		}
	}

	public Schedule GetSchedule()
	{
		return this.schedule;
	}

	public int GetBlockIdx()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		int num = (int)(currentCycleAsPercentage * 24f);
		return Math.Min(num, 23);
	}

	public bool IsAllowed(ScheduleBlockType schedule_block_type)
	{
		int blockIdx = ScheduleManager.Instance.GetBlockIdx();
		return this.GetSchedule().GetBlocks()[blockIdx].Contains(schedule_block_type);
	}

	public const bool ENABLE_SCHEDULE_MANAGER = true;

	[Serialize]
	private Schedule schedule;

	public static ScheduleManager Instance;

	private ScheduleBlockType previousBlock;
}
