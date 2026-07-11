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
			this.Hygene = this.Add("Hygene", 1, UI.SCHEDULEGROUPS.HYGENE.NAME, UI.SCHEDULEGROUPS.HYGENE.DESCRIPTION, UI.SCHEDULEGROUPS.HYGENE.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Hygiene,
				Db.Get().ScheduleBlockTypes.Work
			}, false);
			this.Worktime = this.Add("Worktime", 18, UI.SCHEDULEGROUPS.WORKTIME.NAME, UI.SCHEDULEGROUPS.WORKTIME.DESCRIPTION, UI.SCHEDULEGROUPS.WORKTIME.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType> { Db.Get().ScheduleBlockTypes.Work }, true);
			this.Recreation = this.Add("Recreation", 2, UI.SCHEDULEGROUPS.RECREATION.NAME, UI.SCHEDULEGROUPS.RECREATION.DESCRIPTION, UI.SCHEDULEGROUPS.RECREATION.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Hygiene,
				Db.Get().ScheduleBlockTypes.Eat,
				Db.Get().ScheduleBlockTypes.Recreation,
				Db.Get().ScheduleBlockTypes.Work
			}, false);
			this.Sleep = this.Add("Sleep", 3, UI.SCHEDULEGROUPS.SLEEP.NAME, UI.SCHEDULEGROUPS.SLEEP.DESCRIPTION, UI.SCHEDULEGROUPS.SLEEP.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType> { Db.Get().ScheduleBlockTypes.Sleep }, false);
			int num = 0;
			foreach (ScheduleGroup scheduleGroup in this.allGroups)
			{
				num += scheduleGroup.defaultSegments;
			}
		}

		public ScheduleGroup Add(string id, int defaultSegments, string name, string description, string notificationTooltip, List<ScheduleBlockType> allowedTypes, bool alarm = false)
		{
			ScheduleGroup scheduleGroup = new ScheduleGroup(id, this, defaultSegments, name, description, notificationTooltip, allowedTypes, alarm);
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
