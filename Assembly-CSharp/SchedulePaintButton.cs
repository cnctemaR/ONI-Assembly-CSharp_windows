using System;
using System.Collections.Generic;
using UnityEngine;

public class SchedulePaintButton : KMonoBehaviour
{
	public ScheduleGroup group { get; private set; }

	public void SetGroup(ScheduleGroup group, Dictionary<string, ColorStyleSetting> styles, Action<SchedulePaintButton> onClick)
	{
		this.group = group;
		if (styles.ContainsKey(group.Id))
		{
			this.toggleState.SetColorStyle(styles[group.Id]);
		}
		this.label.text = group.Name;
		MultiToggle multiToggle = this.toggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			onClick(this);
		}));
		this.toolTip.SetSimpleTooltip(group.GetTooltip());
		base.gameObject.name = "PaintButton_" + group.Id;
	}

	public void SetToggle(bool on)
	{
		this.toggle.ChangeState(on ? 1 : 0);
	}

	[SerializeField]
	private LocText label;

	[SerializeField]
	private ImageToggleState toggleState;

	[SerializeField]
	private MultiToggle toggle;

	[SerializeField]
	private ToolTip toolTip;
}
