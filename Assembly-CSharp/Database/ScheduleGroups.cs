using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ScheduleGroups : ResourceSet<ScheduleGroup>
	{
		public ScheduleGroups(ResourceSet parent)
			: base("ScheduleGroups", parent)
		{
			this.allGroups = new List<ScheduleGroup>();
			this.Hygene = this.Add("Hygene", 1, UI.SCHEDULEGROUPS.HYGENE.NAME, UI.SCHEDULEGROUPS.HYGENE.DESCRIPTION, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Hygiene,
				Db.Get().ScheduleBlockTypes.Work
			}, false, false);
			this.Worktime = this.Add("Worktime", 18, UI.SCHEDULEGROUPS.WORKTIME.NAME, UI.SCHEDULEGROUPS.WORKTIME.DESCRIPTION, new List<ScheduleBlockType> { Db.Get().ScheduleBlockTypes.Work }, true, true);
			this.Recreation = this.Add("Recreation", 2, UI.SCHEDULEGROUPS.RECREATION.NAME, UI.SCHEDULEGROUPS.RECREATION.DESCRIPTION, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Work,
				Db.Get().ScheduleBlockTypes.Eat,
				Db.Get().ScheduleBlockTypes.Recreation,
				Db.Get().ScheduleBlockTypes.Hygiene
			}, false, false);
			this.Sleep = this.Add("Sleep", 3, UI.SCHEDULEGROUPS.SLEEP.NAME, UI.SCHEDULEGROUPS.SLEEP.DESCRIPTION, new List<ScheduleBlockType> { Db.Get().ScheduleBlockTypes.Sleep }, false, false);
			int num = 0;
			foreach (ScheduleGroup scheduleGroup in this.allGroups)
			{
				num += scheduleGroup.defaultSegments;
			}
		}

		public ScheduleGroup Add(string id, int defaultSegments, string name, string description, List<ScheduleBlockType> allowedTypes, bool flexible = false, bool alarm = false)
		{
			ScheduleGroup scheduleGroup = new ScheduleGroup(id, this, defaultSegments, name, description, allowedTypes, flexible, alarm);
			this.allGroups.Add(scheduleGroup);
			return scheduleGroup;
		}

		public ScheduleGroup FindGroupForScheduleTypes(List<ScheduleBlockType> types)
		{
			foreach (ScheduleGroup scheduleGroup in this.allGroups)
			{
				if (Schedule.AreScheduleTypesIdentical(scheduleGroup.allowedTypes, types))
				{
					return scheduleGroup;
				}
			}
			return null;
		}

		public List<ScheduleGroup> allGroups;

		public ScheduleGroup Hygene;

		public ScheduleGroup Worktime;

		public ScheduleGroup Recreation;

		public ScheduleGroup Sleep;
	}
}
