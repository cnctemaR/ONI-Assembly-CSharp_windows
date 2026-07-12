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

	public Schedule GetDefaultBionicSchedule()
	{
		return this.schedules.Find((Schedule match) => match.isDefaultForBionics);
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
		List<ScheduleBlock> scheduleBlocksFromGroupDefaults = Schedule.GetScheduleBlocksFromGroupDefaults(Db.Get().ScheduleGroups.allGroups);
		foreach (Schedule schedule2 in this.schedules)
		{
			List<ScheduleBlock> blocks = schedule2.GetBlocks();
			for (int i = 0; i < blocks.Count; i++)
			{
				ScheduleBlock scheduleBlock = blocks[i];
				if (Db.Get().ScheduleGroups.FindGroupForScheduleTypes(scheduleBlock.allowed_types) == null)
				{
					ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(scheduleBlocksFromGroupDefaults[i].allowed_types);
					schedule2.SetBlockGroup(i, scheduleGroup);
				}
			}
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
		Schedule schedule = this.schedules[0];
		if (minion.model == GameTags.Minions.Models.Bionic)
		{
			if (this.GetDefaultBionicSchedule() == null)
			{
				if (!this.hasDeletedDefaultBionicSchedule)
				{
					Schedule schedule2 = this.AddSchedule(Db.Get().ScheduleGroups.allGroups, UI.SCHEDULESCREEN.SCHEDULE_NAME_DEFAULT_BIONIC, true);
					schedule2.AddTimetable(Schedule.GetScheduleBlocksFromGroupDefaults(Db.Get().ScheduleGroups.allGroups));
					schedule2.AddTimetable(Schedule.GetScheduleBlocksFromGroupDefaults(Db.Get().ScheduleGroups.allGroups));
					for (int i = 0; i < schedule2.GetBlocks().Count; i++)
					{
						schedule2.SetBlockGroup(i, Db.Get().ScheduleGroups.Worktime);
					}
					for (int j = 1; j <= 6; j++)
					{
						schedule2.SetBlockGroup(schedule2.GetBlocks().Count - j, Db.Get().ScheduleGroups.Sleep);
					}
					for (int k = 7; k <= 12; k++)
					{
						schedule2.SetBlockGroup(schedule2.GetBlocks().Count - k, Db.Get().ScheduleGroups.Recreation);
					}
					schedule = schedule2;
					schedule2.isDefaultForBionics = true;
				}
			}
			else
			{
				schedule = this.GetDefaultBionicSchedule();
			}
		}
		else if (this.GetSchedule(component) != null)
		{
			schedule = this.GetSchedule(component);
		}
		schedule.Assign(component);
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
				schedule.SetBlockGroup(i, Db.Get().ScheduleGroups.Worktime);
			}
			schedule.SetBlockGroup(21, Db.Get().ScheduleGroups.Recreation);
			schedule.SetBlockGroup(22, Db.Get().ScheduleGroups.Recreation);
			schedule.SetBlockGroup(23, Db.Get().ScheduleGroups.Sleep);
		}
	}

	public Schedule AddSchedule(List<ScheduleGroup> groups, string name = null, bool alarmOn = false)
	{
		if (name == null)
		{
			this.scheduleNameIncrementor++;
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

	public Schedule DuplicateSchedule(Schedule source)
	{
		if (base.name == null)
		{
			this.scheduleNameIncrementor++;
			base.name = string.Format(UI.SCHEDULESCREEN.SCHEDULE_NAME_FORMAT, this.scheduleNameIncrementor.ToString());
		}
		Schedule schedule = new Schedule("copy of " + source.name, source.GetBlocks(), source.alarmActivated);
		schedule.ProgressTimetableIdx = source.ProgressTimetableIdx;
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
		if (schedule.isDefaultForBionics)
		{
			this.hasDeletedDefaultBionicSchedule = true;
		}
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
		Schedule schedule = this.GetSchedule(schedulable);
		return schedule != null && schedule.GetCurrentScheduleBlock().IsAllowed(schedule_block_type);
	}

	public static int GetCurrentHour()
	{
		return Math.Min((int)(GameClock.Instance.GetCurrentCycleAsPercentage() * 24f), 23);
	}

	public void Sim33ms(float dt)
	{
		int currentHour = ScheduleManager.GetCurrentHour();
		if (ScheduleManager.GetCurrentHour() != this.lastHour)
		{
			foreach (Schedule schedule in this.schedules)
			{
				schedule.Tick();
			}
			this.lastHour = currentHour;
		}
	}

	public void PlayScheduleAlarm(Schedule schedule, ScheduleBlock block, bool forwards)
	{
		Notification notification = new Notification(string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.NAME, schedule.name, block.name), NotificationType.Good, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SCHEDULE_CHANGED.TOOLTIP.Replace("{0}", schedule.name).Replace("{1}", block.name).Replace("{2}", Db.Get().ScheduleGroups.Get(block.GroupId).notificationTooltip), null, true, 0f, null, null, null, true, false, false);
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
			yield return SequenceUtil.WaitForSeconds(TuningData<ScheduleManager.Tuning>.Get().toneSpacingSeconds);
			num2 = i;
		}
		yield break;
	}

	private void PlayTone(int pitch, bool forwards)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("WorkChime_tone", false), Vector3.zero, 1f);
		eventInstance.setParameterByName("WorkChime_pitch", (float)pitch, false);
		eventInstance.setParameterByName("WorkChime_start", (float)(forwards ? 1 : 0), false);
		KFMOD.EndOneShot(eventInstance);
	}

	[Serialize]
	private List<Schedule> schedules;

	[Serialize]
	private int lastHour;

	[Serialize]
	private int scheduleNameIncrementor;

	public static ScheduleManager Instance;

	[Serialize]
	private bool hasDeletedDefaultBionicSchedule;

	public class Tuning : TuningData<ScheduleManager.Tuning>
	{
		public float toneSpacingSeconds;

		public int minToneIndex;

		public int maxToneIndex;

		public int firstLastToneSpacing;
	}
}
