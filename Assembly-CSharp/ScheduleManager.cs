using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleManager")]
public class ScheduleManager : KMonoBehaviour, ISim33ms
{
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
			this.AddDefaultSchedule(true);
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
			this.AddDefaultSchedule(true);
		}
		foreach (Schedule schedule in this.schedules)
		{
			schedule.ClearNullReferences();
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

	public void OnStoredDupeDestroyed(StoredMinionIdentity dupe)
	{
		foreach (Schedule schedule in this.schedules)
		{
			schedule.Unassign(dupe.gameObject.GetComponent<Schedulable>());
		}
	}

	public void AddDefaultSchedule(bool alarmOn)
	{
		Schedule schedule = this.AddSchedule(Db.Get().ScheduleGroups.allGroups, UI.SCHEDULESCREEN.SCHEDULE_NAME_DEFAULT, alarmOn);
		if (Game.Instance.FastWorkersModeActive)
		{
			for (int i = 0; i < 21; i++)
			{
				schedule.SetGroup(i, Db.Get().ScheduleGroups.Worktime);
			}
			schedule.SetGroup(21, Db.Get().ScheduleGroups.Recreation);
			schedule.SetGroup(22, Db.Get().ScheduleGroups.Recreation);
			schedule.SetGroup(23, Db.Get().ScheduleGroups.Sleep);
		}
	}

	public Schedule AddSchedule(List<ScheduleGroup> groups, string name = null, bool alarmOn = false)
	{
		this.scheduleNameIncrementor++;
		if (name == null)
		{
			name = string.Format(UI.SCHEDULESCREEN.SCHEDULE_NAME_FORMAT, this.scheduleNameIncrementor.ToString());
		}
		Schedule schedule = new Schedule(name, groups, alarmOn);
		this.schedules.Add(schedule);
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(this.schedules);
		}
		return schedule;
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

	public bool IsAllowed(Schedulable schedulable, ScheduleBlockType schedule_block_type)
	{
		int blockIdx = Schedule.GetBlockIdx();
		return this.GetSchedule(schedulable).GetBlock(blockIdx).IsAllowed(schedule_block_type);
	}

	public void Sim33ms(float dt)
	{
		int blockIdx = Schedule.GetBlockIdx();
		if (blockIdx != this.lastIdx)
		{
			foreach (Schedule schedule in this.schedules)
			{
				schedule.Tick();
			}
			this.lastIdx = blockIdx;
		}
	}

	public void PlayScheduleAlarm(Schedule schedule, ScheduleBlock block, bool forwards)
	{
		Notification notification = new Notification(string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.NAME, schedule.name, block.name), NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.TOOLTIP, schedule.name, block.name, Db.Get().ScheduleGroups.Get(block.GroupId).notificationTooltip), null, true, 0f, null, null, null, true);
		base.GetComponent<Notifier>().Add(notification, "");
		base.StartCoroutine(this.PlayScheduleTone(schedule, forwards));
	}

	private IEnumerator PlayScheduleTone(Schedule schedule, bool forwards)
	{
		int[] tones = schedule.GetTones();
		int num2;
		for (int i = 0; i < tones.Length; i = num2 + 1)
		{
			int num = (forwards ? i : (tones.Length - 1 - i));
			this.PlayTone(tones[num], forwards);
			yield return new WaitForSeconds(TuningData<ScheduleManager.Tuning>.Get().toneSpacingSeconds);
			num2 = i;
		}
		yield break;
	}

	private void PlayTone(int pitch, bool forwards)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("WorkChime_tone", false), Vector3.zero, 1f);
		eventInstance.setParameterValue("WorkChime_pitch", (float)pitch);
		eventInstance.setParameterValue("WorkChime_start", (float)(forwards ? 1 : 0));
		KFMOD.EndOneShot(eventInstance);
	}

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
