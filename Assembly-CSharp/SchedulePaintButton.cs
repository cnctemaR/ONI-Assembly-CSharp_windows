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
		this.toggle.onClick += delegate
		{
			onClick(this);
		};
		base.gameObject.name = "PaintButton_" + group.Id;
	}

	[SerializeField]
	private LocText label;

	[SerializeField]
	private ImageToggleState toggleState;

	[SerializeField]
	public KToggle toggle;
}
