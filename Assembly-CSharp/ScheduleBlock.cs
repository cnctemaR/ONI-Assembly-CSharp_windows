using System;
using System.Collections.Generic;
using KSerialization;

[Serializable]
public class ScheduleBlock
{
	public string GroupId
	{
		get
		{
			if (this._groupId == null)
			{
				ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(this.allowed_types);
				if (scheduleGroup != null)
				{
					this._groupId = scheduleGroup.Id;
				}
			}
			return this._groupId;
		}
		set
		{
			this._groupId = value;
		}
	}

	public ScheduleBlock(string name, List<ScheduleBlockType> allowed_types, string groupId)
	{
		this.name = name;
		this.allowed_types = allowed_types;
		this._groupId = groupId;
	}

	public bool IsAllowed(ScheduleBlockType type)
	{
		if (this.allowed_types != null)
		{
			foreach (ScheduleBlockType scheduleBlockType in this.allowed_types)
			{
				if (type.IdHash == scheduleBlockType.IdHash)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	[Serialize]
	public string name;

	[Serialize]
	public List<ScheduleBlockType> allowed_types;

	[Serialize]
	private string _groupId;
}
