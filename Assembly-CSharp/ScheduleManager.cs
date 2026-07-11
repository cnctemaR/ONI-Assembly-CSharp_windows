using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ScheduleManager : KMonoBehaviour, ISim33ms
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onSheduleBlocksChanged;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<List<Schedule>> onSchedulesChanged;

	public static void DestroyInstance()
	{
		ScheduleManager.Instance = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.schedules.Count == 0)
		{
			this.SetupDefaultSchedule();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.schedules = new List<Schedule>();
		ScheduleManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		if (this.schedules.Count == 0)
		{
			this.SetupDefaultSchedule();
		}
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			Schedulable component = minionIdentity.GetComponent<Schedulable>();
			if (this.GetSchedule(component) == null)
			{
				this.schedules[0].Assign(component);
			}
		}
		Components.LiveMinionIdentities.OnAdd += this.OnAddDupe;
		Components.LiveMinionIdentities.OnRemove += this.OnRemoveDupe;
	}

	private void OnAddDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		if (this.GetSchedule(component) == null)
		{
			this.schedules[0].Assign(component);
		}
	}

	private void OnRemoveDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		Schedule schedule = this.GetSchedule(component);
		if (schedule != null)
		{
			schedule.Unassign(component);
		}
	}

	private void SetupDefaultSchedule()
	{
		this.AddSchedule(Db.Get().ScheduleGroups.allGroups, UI.SCHEDULESCREEN.SCHEDULE_NAME_DEFAULT);
	}

	public void AddSchedule(List<ScheduleGroup> groups, string name = null)
	{
		this.scheduleNameIncrementor++;
		if (name == null)
		{
			name = string.Format(UI.SCHEDULESCREEN.SCHEDULE_NAME_FORMAT, this.scheduleNameIncrementor.ToString());
		}
		Schedule schedule = new Schedule(name, groups);
		this.schedules.Add(schedule);
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(this.schedules);
		}
	}

	public void DeleteSchedule(Schedule schedule)
	{
		if (this.schedules.Count == 1)
		{
			return;
		}
		List<Ref<Schedulable>> assigned = schedule.GetAssigned();
		this.schedules.Remove(schedule);
		foreach (Ref<Schedulable> @ref in assigned)
		{
			this.schedules[0].Assign(@ref.Get());
		}
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(this.schedules);
		}
	}

	public Schedule GetSchedule(Schedulable schedulable)
	{
		foreach (Schedule schedule in this.schedules)
		{
			if (schedule.IsAssigned(schedulable))
			{
				return schedule;
			}
		}
		return null;
	}

	public List<Schedule> GetSchedules()
	{
		return this.schedules;
	}

	public int GetBlockIdx()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		int num = (int)(currentCycleAsPercentage * 24f);
		return Math.Min(num, 23);
	}

	public bool IsAllowed(Schedulable schedulable, ScheduleBlockType schedule_block_type)
	{
		int blockIdx = ScheduleManager.Instance.GetBlockIdx();
		foreach (ScheduleBlockType scheduleBlockType in this.GetSchedule(schedulable).GetBlocks()[blockIdx].allowed_types)
		{
			if (scheduleBlockType.IdHash == schedule_block_type.IdHash)
			{
				return true;
			}
		}
		return false;
	}

	public void Sim33ms(float dt)
	{
		int blockIdx = this.GetBlockIdx();
		if (blockIdx != this.lastIdx)
		{
			using (List<Schedule>.Enumerator enumerator = this.schedules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ScheduleManager.<Sim33ms>c__AnonStorey1 <Sim33ms>c__AnonStorey = new ScheduleManager.<Sim33ms>c__AnonStorey1();
					<Sim33ms>c__AnonStorey.schedule = enumerator.Current;
					ScheduleBlock scheduleBlock = <Sim33ms>c__AnonStorey.schedule.GetBlocks()[this.lastIdx];
					ScheduleBlock block = <Sim33ms>c__AnonStorey.schedule.GetBlocks()[blockIdx];
					if (!Schedule.AreScheduleTypesIdentical(block.allowed_types, scheduleBlock.allowed_types))
					{
						if (<Sim33ms>c__AnonStorey.schedule.alarm && scheduleBlock.alarm != block.alarm)
						{
							Notification notification = new Notification(string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.NAME, <Sim33ms>c__AnonStorey.schedule.name, block.name), NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.TOOLTIP, <Sim33ms>c__AnonStorey.schedule.name, block.name) + notificationList.ReduceMessages(true), null, true, 0f, null, null);
							base.GetComponent<Notifier>().Add(notification, string.Empty);
							base.StartCoroutine(this.PlayScheduleTone(<Sim33ms>c__AnonStorey.schedule, block.alarm));
						}
						if (this.onSheduleBlocksChanged != null)
						{
							this.onSheduleBlocksChanged();
						}
					}
					string text = string.Empty;
					foreach (ScheduleBlockType scheduleBlockType in block.allowed_types)
					{
						if (text != string.Empty)
						{
							text += ", ";
						}
						text += scheduleBlockType.Name;
					}
				}
			}
			this.lastIdx = blockIdx;
		}
	}

	private IEnumerator PlayScheduleTone(Schedule schedule, bool forwards)
	{
		int[] tones = schedule.GetTones();
		for (int i = 0; i < tones.Length; i++)
		{
			int t = ((!forwards) ? (tones.Length - 1 - i) : i);
			this.PlayTone(tones[t], forwards);
			yield return new WaitForSeconds(TuningData<ScheduleManager.Tuning>.Get().toneSpacingSeconds);
		}
		yield break;
	}

	private void PlayTone(int pitch, bool forwards)
	{
		FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("WorkChime_tone", false), Vector3.zero);
		eventInstance.setParameterValue("WorkChime_pitch", (float)pitch);
		eventInstance.setParameterValue("WorkChime_start", (float)((!forwards) ? 0 : 1));
		KFMOD.EndOneShot(eventInstance);
	}

	public const bool ENABLE_SCHEDULE_MANAGER = true;

	[Serialize]
	private List<Schedule> schedules;

	[Serialize]
	private int lastIdx;

	[Serialize]
	private int scheduleNameIncrementor;

	public static ScheduleManager Instance;

	public class Tuning : TuningData<ScheduleManager.Tuning>
	{
		public float toneSpacingSeconds;

		public int minToneIndex;

		public int maxToneIndex;

		public int firstLastToneSpacing;
	}
}
