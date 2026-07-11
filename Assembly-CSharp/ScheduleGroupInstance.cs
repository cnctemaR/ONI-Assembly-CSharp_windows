using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ScheduleGroupInstance
{
	public ScheduleGroupInstance(ScheduleGroup scheduleGroup)
	{
		this.scheduleGroup = scheduleGroup;
		this.segments = scheduleGroup.defaultSegments;
	}

	public ScheduleGroup scheduleGroup
	{
		get
		{
			return Db.Get().ScheduleGroups.Get(this.scheduleGroupID);
		}
		set
		{
			this.scheduleGroupID = value.Id;
		}
	}

	[Serialize]
	private string scheduleGroupID;

	[Serialize]
	public int segments;
}
