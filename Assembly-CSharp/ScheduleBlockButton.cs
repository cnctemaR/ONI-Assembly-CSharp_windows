using System;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleBlockButton : KMonoBehaviour
{
	public int idx { get; private set; }

	public void Setup(int idx, Dictionary<string, ColorStyleSetting> paintStyles)
	{
		this.idx = idx;
		this.paintStyles = paintStyles;
		base.gameObject.name = "ScheduleBlock_" + idx.ToString();
	}

	public void SetBlockTypes(List<ScheduleBlockType> blockTypes)
	{
		ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(blockTypes);
		if (scheduleGroup != null && this.paintStyles.ContainsKey(scheduleGroup.Id))
		{
			this.image.colorStyleSetting = this.paintStyles[scheduleGroup.Id];
			this.image.ApplyColorStyleSetting();
			this.toolTip.SetSimpleTooltip(scheduleGroup.GetTooltip());
		}
		else
		{
			this.toolTip.SetSimpleTooltip("UNKNOWN");
		}
	}

	[SerializeField]
	private KImage image;

	[SerializeField]
	private ToolTip toolTip;

	private Dictionary<string, ColorStyleSetting> paintStyles;
}
