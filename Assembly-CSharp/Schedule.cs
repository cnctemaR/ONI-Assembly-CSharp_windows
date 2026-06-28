using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Schedule : ISaveLoadable
{
	public Schedule(int time_slots)
	{
		this.blocks = new List<ScheduleBlockType>[time_slots];
		for (int i = 0; i < this.blocks.Length; i++)
		{
			this.blocks[i] = new List<ScheduleBlockType>();
		}
	}

	public void Add(int idx, ScheduleBlockType type)
	{
		if (!this.blocks[idx].Contains(type))
		{
			this.blocks[idx].Add(type);
			if (this.onChanged != null)
			{
				this.onChanged();
			}
		}
	}

	public void Remove(int idx, ScheduleBlockType type)
	{
		if (this.blocks[idx].Remove(type))
		{
			if (this.onChanged != null)
			{
				this.onChanged();
			}
		}
	}

	public List<ScheduleBlockType>[] GetBlocks()
	{
		return this.blocks;
	}

	[OnSerializing]
	private void OnSerialize()
	{
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.savedBlocks != null)
		{
		}
	}

	private List<ScheduleBlockType>[] blocks;

	[Serialize]
	private List<ResourceRef<ScheduleBlockType>>[] savedBlocks;

	public global::System.Action onChanged;
}
