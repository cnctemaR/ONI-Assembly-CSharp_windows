using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Schedule : ISaveLoadable, IListableOption
{
	public int ProgressTimetableIdx
	{
		get
		{
			return this.progressTimetableIdx;
		}
		set
		{
			this.progressTimetableIdx = value;
		}
	}

	public ScheduleBlock GetCurrentScheduleBlock()
	{
		return this.GetBlock(this.GetCurrentBlockIdx());
	}

	public int GetCurrentBlockIdx()
	{
		return Math.Min((int)(GameClock.Instance.GetCurrentCycleAsPercentage() * 24f), 23) + this.progressTimetableIdx * 24;
	}

	public ScheduleBlock GetPreviousScheduleBlock()
	{
		return this.GetBlock(this.GetPreviousBlockIdx());
	}

	public int GetPreviousBlockIdx()
	{
		int num = this.GetCurrentBlockIdx() - 1;
		if (num == -1)
		{
			num = this.blocks.Count - 1;
		}
		return num;
	}

	public void ClearNullReferences()
	{
		this.assigned.RemoveAll((Ref<Schedulable> x) => x.Get() == null);
	}

	public Schedule(string name, List<ScheduleGroup> defaultGroups, bool alarmActivated)
	{
		this.name = name;
		this.alarmActivated = alarmActivated;
		this.blocks = new List<ScheduleBlock>(defaultGroups.Count);
		this.assigned = new List<Ref<Schedulable>>();
		this.tones = this.GenerateTones();
		this.SetBlocksToGroupDefaults(defaultGroups);
	}

	public Schedule(string name, List<ScheduleBlock> sourceBlocks, bool alarmActivated)
	{
		this.name = name;
		this.alarmActivated = alarmActivated;
		this.blocks = new List<ScheduleBlock>();
		for (int i = 0; i < sourceBlocks.Count; i++)
		{
			this.blocks.Add(new ScheduleBlock(sourceBlocks[i].name, sourceBlocks[i].GroupId));
		}
		this.assigned = new List<Ref<Schedulable>>();
		this.tones = this.GenerateTones();
		this.Changed();
	}

	public void SetBlocksToGroupDefaults(List<ScheduleGroup> defaultGroups)
	{
		this.blocks = Schedule.GetScheduleBlocksFromGroupDefaults(defaultGroups);
		global::Debug.Assert(this.blocks.Count == 24);
		this.Changed();
	}

	public static List<ScheduleBlock> GetScheduleBlocksFromGroupDefaults(List<ScheduleGroup> defaultGroups)
	{
		List<ScheduleBlock> list = new List<ScheduleBlock>();
		for (int i = 0; i < defaultGroups.Count; i++)
		{
			ScheduleGroup scheduleGroup = defaultGroups[i];
			for (int j = 0; j < scheduleGroup.defaultSegments; j++)
			{
				list.Add(new ScheduleBlock(scheduleGroup.Name, scheduleGroup.Id));
			}
		}
		return list;
	}

	public void Tick()
	{
		ScheduleBlock currentScheduleBlock = this.GetCurrentScheduleBlock();
		ScheduleBlock block = this.GetBlock(this.GetPreviousBlockIdx());
		global::Debug.Assert(block != currentScheduleBlock);
		if (this.GetCurrentBlockIdx() % 24 == 0)
		{
			this.progressTimetableIdx++;
			if (this.progressTimetableIdx >= this.blocks.Count / 24)
			{
				this.progressTimetableIdx = 0;
			}
			if (ScheduleScreen.Instance != null)
			{
				ScheduleScreen.Instance.OnChangeCurrentTimetable();
			}
		}
		if (!Schedule.AreScheduleTypesIdentical(currentScheduleBlock.allowed_types, block.allowed_types))
		{
			ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(currentScheduleBlock.allowed_types);
			ScheduleGroup scheduleGroup2 = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block.allowed_types);
			if (this.alarmActivated && scheduleGroup2.alarm != scheduleGroup.alarm)
			{
				ScheduleManager.Instance.PlayScheduleAlarm(this, currentScheduleBlock, scheduleGroup.alarm);
			}
			foreach (Ref<Schedulable> @ref in this.GetAssigned())
			{
				@ref.Get().OnScheduleBlocksChanged(this);
			}
		}
		foreach (Ref<Schedulable> ref2 in this.GetAssigned())
		{
			ref2.Get().OnScheduleBlocksTick(this);
		}
	}

	string IListableOption.GetProperName()
	{
		return this.name;
	}

	public int[] GenerateTones()
	{
		int minToneIndex = TuningData<ScheduleManager.Tuning>.Get().minToneIndex;
		int maxToneIndex = TuningData<ScheduleManager.Tuning>.Get().maxToneIndex;
		int firstLastToneSpacing = TuningData<ScheduleManager.Tuning>.Get().firstLastToneSpacing;
		int[] array = new int[4];
		array[0] = global::UnityEngine.Random.Range(minToneIndex, maxToneIndex - firstLastToneSpacing + 1);
		array[1] = global::UnityEngine.Random.Range(minToneIndex, maxToneIndex + 1);
		array[2] = global::UnityEngine.Random.Range(minToneIndex, maxToneIndex + 1);
		array[3] = global::UnityEngine.Random.Range(array[0] + firstLastToneSpacing, maxToneIndex + 1);
		return array;
	}

	public List<Ref<Schedulable>> GetAssigned()
	{
		if (this.assigned == null)
		{
			this.assigned = new List<Ref<Schedulable>>();
		}
		return this.assigned;
	}

	public int[] GetTones()
	{
		if (this.tones == null)
		{
			this.tones = this.GenerateTones();
		}
		return this.tones;
	}

	public void SetBlockGroup(int idx, ScheduleGroup group)
	{
		if (0 <= idx && idx < this.blocks.Count)
		{
			this.blocks[idx] = new ScheduleBlock(group.Name, group.Id);
			this.Changed();
		}
	}

	private void Changed()
	{
		foreach (Ref<Schedulable> @ref in this.GetAssigned())
		{
			@ref.Get().OnScheduleChanged(this);
		}
		if (this.onChanged != null)
		{
			this.onChanged(this);
		}
	}

	public List<ScheduleBlock> GetBlocks()
	{
		return this.blocks;
	}

	public ScheduleBlock GetBlock(int idx)
	{
		return this.blocks[idx];
	}

	public void InsertTimetable(int timetableIdx, List<ScheduleBlock> newBlocks)
	{
		this.blocks.InsertRange(timetableIdx * 24, newBlocks);
		if (timetableIdx <= this.progressTimetableIdx)
		{
			this.progressTimetableIdx++;
		}
	}

	public void AddTimetable(List<ScheduleBlock> newBlocks)
	{
		this.blocks.AddRange(newBlocks);
	}

	public void RemoveTimetable(int TimetableToRemoveIdx)
	{
		int num = TimetableToRemoveIdx * 24;
		int num2 = this.blocks.Count / 24;
		this.blocks.RemoveRange(num, 24);
		bool flag = TimetableToRemoveIdx == this.progressTimetableIdx;
		bool flag2 = this.progressTimetableIdx == num2 - 1;
		if (TimetableToRemoveIdx < this.progressTimetableIdx || (flag && flag2))
		{
			this.progressTimetableIdx--;
		}
		ScheduleScreen.Instance.OnChangeCurrentTimetable();
	}

	public void Assign(Schedulable schedulable)
	{
		if (!this.IsAssigned(schedulable))
		{
			this.GetAssigned().Add(new Ref<Schedulable>(schedulable));
		}
		this.Changed();
	}

	public void Unassign(Schedulable schedulable)
	{
		for (int i = 0; i < this.GetAssigned().Count; i++)
		{
			if (this.GetAssigned()[i].Get() == schedulable)
			{
				this.GetAssigned().RemoveAt(i);
				break;
			}
		}
		this.Changed();
	}

	public bool IsAssigned(Schedulable schedulable)
	{
		using (List<Ref<Schedulable>>.Enumerator enumerator = this.GetAssigned().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Get() == schedulable)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool AreScheduleTypesIdentical(List<ScheduleBlockType> a, List<ScheduleBlockType> b)
	{
		if (a.Count != b.Count)
		{
			return false;
		}
		foreach (ScheduleBlockType scheduleBlockType in a)
		{
			bool flag = false;
			foreach (ScheduleBlockType scheduleBlockType2 in b)
			{
				if (scheduleBlockType.IdHash == scheduleBlockType2.IdHash)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public bool ShiftTimetable(bool up, int timetableToShiftIdx = 0)
	{
		if (timetableToShiftIdx == 0 && up)
		{
			return false;
		}
		if (timetableToShiftIdx == this.blocks.Count / 24 - 1 && !up)
		{
			return false;
		}
		int num = timetableToShiftIdx * 24;
		List<ScheduleBlock> list = new List<ScheduleBlock>();
		List<ScheduleBlock> list2 = new List<ScheduleBlock>();
		if (up)
		{
			list = this.blocks.GetRange(num, 24);
			list2 = this.blocks.GetRange(num - 24, 24);
			this.blocks.RemoveRange(num - 24, 48);
			this.blocks.InsertRange(num - 24, list2);
			this.blocks.InsertRange(num - 24, list);
		}
		else
		{
			list = this.blocks.GetRange(num, 24);
			list2 = this.blocks.GetRange(num + 24, 24);
			this.blocks.RemoveRange(num, 48);
			this.blocks.InsertRange(num, list);
			this.blocks.InsertRange(num, list2);
		}
		this.Changed();
		return true;
	}

	public void RotateBlocks(bool directionLeft, int timetableToRotateIdx = 0)
	{
		List<ScheduleBlock> list = new List<ScheduleBlock>();
		int num = timetableToRotateIdx * 24;
		list = this.blocks.GetRange(num, 24);
		if (!directionLeft)
		{
			ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.Get(list[list.Count - 1].GroupId);
			for (int i = list.Count - 1; i >= 1; i--)
			{
				ScheduleGroup scheduleGroup2 = Db.Get().ScheduleGroups.Get(list[i - 1].GroupId);
				list[i].GroupId = scheduleGroup2.Id;
			}
			list[0].GroupId = scheduleGroup.Id;
		}
		else
		{
			ScheduleGroup scheduleGroup3 = Db.Get().ScheduleGroups.Get(list[0].GroupId);
			for (int j = 0; j < list.Count - 1; j++)
			{
				ScheduleGroup scheduleGroup4 = Db.Get().ScheduleGroups.Get(list[j + 1].GroupId);
				list[j].GroupId = scheduleGroup4.Id;
			}
			list[list.Count - 1].GroupId = scheduleGroup3.Id;
		}
		this.blocks.RemoveRange(num, 24);
		this.blocks.InsertRange(num, list);
		this.Changed();
	}

	[Serialize]
	private List<ScheduleBlock> blocks;

	[Serialize]
	private List<Ref<Schedulable>> assigned;

	[Serialize]
	public string name;

	[Serialize]
	public bool alarmActivated = true;

	[Serialize]
	private int[] tones;

	[Serialize]
	public bool isDefaultForBionics;

	[Serialize]
	private int progressTimetableIdx;

	public Action<Schedule> onChanged;
}
