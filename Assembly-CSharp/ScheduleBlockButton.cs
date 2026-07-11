using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ScheduleBlockButton : KMonoBehaviour
{
	public int idx { get; private set; }

	public void Setup(int idx, Dictionary<string, ColorStyleSetting> paintStyles, int totalBlocks)
	{
		this.idx = idx;
		this.paintStyles = paintStyles;
		if (idx < TRAITS.EARLYBIRD_SCHEDULEBLOCK)
		{
			base.GetComponent<HierarchyReferences>().GetReference<RectTransform>("MorningIcon").gameObject.SetActive(true);
		}
		else if (idx >= totalBlocks - 3)
		{
			base.GetComponent<HierarchyReferences>().GetReference<RectTransform>("NightIcon").gameObject.SetActive(true);
		}
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
			return;
		}
		this.toolTip.SetSimpleTooltip("UNKNOWN");
	}

	[SerializeField]
	private KImage image;

	[SerializeField]
	private ToolTip toolTip;

	private Dictionary<string, ColorStyleSetting> paintStyles;
}
