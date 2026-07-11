using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class ScheduleBlockTypes : ResourceSet<ScheduleBlockType>
	{
		public ScheduleBlockTypes(ResourceSet parent)
			: base("ScheduleBlockTypes", parent)
		{
			this.Sleep = base.Add(new ScheduleBlockType("Sleep", this, UI.SCHEDULEBLOCKTYPES.SLEEP.NAME, UI.SCHEDULEBLOCKTYPES.SLEEP.DESCRIPTION, new Color(0.9843137f, 0.99215686f, 0.27058825f)));
			this.Eat = base.Add(new ScheduleBlockType("Eat", this, UI.SCHEDULEBLOCKTYPES.EAT.NAME, UI.SCHEDULEBLOCKTYPES.EAT.DESCRIPTION, new Color(0.80784315f, 0.5294118f, 0.11372549f)));
			this.Work = base.Add(new ScheduleBlockType("Work", this, UI.SCHEDULEBLOCKTYPES.WORK.NAME, UI.SCHEDULEBLOCKTYPES.WORK.DESCRIPTION, new Color(0.9372549f, 0.12941177f, 0.12941177f)));
			this.Hygiene = base.Add(new ScheduleBlockType("Hygiene", this, UI.SCHEDULEBLOCKTYPES.HYGIENE.NAME, UI.SCHEDULEBLOCKTYPES.HYGIENE.DESCRIPTION, new Color(0.45882353f, 0.1764706f, 0.34509805f)));
			this.Recreation = base.Add(new ScheduleBlockType("Recreation", this, UI.SCHEDULEBLOCKTYPES.RECREATION.NAME, UI.SCHEDULEBLOCKTYPES.RECREATION.DESCRIPTION, new Color(0.45882353f, 0.37254903f, 0.1882353f)));
		}

		public ScheduleBlockType Sleep;

		public ScheduleBlockType Eat;

		public ScheduleBlockType Work;

		public ScheduleBlockType Hygiene;

		public ScheduleBlockType Recreation;
	}
}
