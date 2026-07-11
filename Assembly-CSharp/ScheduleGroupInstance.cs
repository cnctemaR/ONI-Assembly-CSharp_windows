using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ScheduleGroupInstance
{
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

	public ScheduleGroupInstance(ScheduleGroup scheduleGroup)
	{
		this.scheduleGroup = scheduleGroup;
		this.segments = scheduleGroup.defaultSegments;
	}

	[Serialize]
	private string scheduleGroupID;

	[Serialize]
	public int segments;
}
