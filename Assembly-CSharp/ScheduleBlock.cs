using System;
using System.Collections.Generic;
using KSerialization;

[Serializable]
public class ScheduleBlock
{
	public ScheduleBlock(string name, List<ScheduleBlockType> allowed_types, bool alarm)
	{
		this.name = name;
		this.allowed_types = allowed_types;
		this.alarm = alarm;
	}

	[Serialize]
	public string name;

	[Serialize]
	public List<ScheduleBlockType> allowed_types;

	[Serialize]
	public bool alarm;
}
