using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Schedule : ISaveLoadable, IListableOption
{
	public Schedule(string name, List<ScheduleGroup> defaultGroups, bool alarmActivated)
	{
		this.name = name;
		this.alarmActivated = alarmActivated;
		this.blocks = new List<ScheduleBlock>(24);
		this.assigned = new List<Ref<Schedulable>>();
		this.tones = this.GenerateTones();
		this.SetBlocksToGroupDefaults(defaultGroups);
	}

	public static int GetBlockIdx()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		int num = (int)(currentCycleAsPercentage * 24f);
		return Math.Min(num, 23);
	}

	public static int GetLastBlockIdx()
	{
		return (Schedule.GetBlockIdx() + 24 - 1) % 24;
	}

	public void ClearNullReferences()
	{
		this.assigned.RemoveAll((Ref<Schedulable> x) => x.Get() == null);
	}

	public void SetBlocksToGroupDefaults(List<ScheduleGroup> defaultGroups)
	{
		this.blocks.Clear();
		int num = 0;
		for (int i = 0; i < defaultGroups.Count; i++)
		{
			ScheduleGroup scheduleGroup = defaultGroups[i];
			for (int j = 0; j < scheduleGroup.defaultSegments; j++)
			{
				this.blocks.Add(new ScheduleBlock(scheduleGroup.Name, scheduleGroup.allowedTypes, scheduleGroup.Id));
				num++;
			}
		}
		global::Debug.Assert(num == 24);
		this.Changed();
	}

	public void Tick()
	{
		ScheduleBlock block = this.GetBlock(Schedule.GetBlockIdx());
		ScheduleBlock block2 = this.GetBlock(Schedule.GetLastBlockIdx());
		if (!Schedule.AreScheduleTypesIdentical(block.allowed_types, block2.allowed_types))
		{
			ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block.allowed_types);
			ScheduleGroup scheduleGroup2 = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block2.allowed_types);
			if (this.alarmActivated && scheduleGroup2.alarm != scheduleGroup.alarm)
			{
				ScheduleManager.Instance.PlayScheduleAlarm(this, block, scheduleGroup.alarm);
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

	public void SetGroup(int idx, ScheduleGroup group)
	{
		if (0 <= idx && idx < this.blocks.Count)
		{
			this.blocks[idx] = new ScheduleBlock(group.Name, group.allowedTypes, group.Id);
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
		foreach (Ref<Schedulable> @ref in this.GetAssigned())
		{
			if (@ref.Get() == schedulable)
			{
				return true;
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

	public Action<Schedule> onChanged;
}
