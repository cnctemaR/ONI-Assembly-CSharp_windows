using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Schedule : ISaveLoadable, IListableOption
{
	public Schedule(string name, List<ScheduleGroup> defaultGroups)
	{
		this.name = name;
		this.blocks = new List<ScheduleBlock>(24);
		this.assigned = new List<Ref<Schedulable>>();
		this.alarm = true;
		this.tones = this.GenerateTones();
		int num = 0;
		for (int i = 0; i < defaultGroups.Count; i++)
		{
			ScheduleGroup scheduleGroup = defaultGroups[i];
			for (int j = 0; j < scheduleGroup.defaultSegments; j++)
			{
				this.blocks.Add(new ScheduleBlock(scheduleGroup.Name, scheduleGroup.allowedTypes, scheduleGroup.alarm));
				num++;
			}
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
		this.blocks[idx] = new ScheduleBlock(group.Name, group.allowedTypes, group.alarm);
		this.Changed();
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
	public bool alarm;

	[Serialize]
	private int[] tones;

	public Action<Schedule> onChanged;
}
