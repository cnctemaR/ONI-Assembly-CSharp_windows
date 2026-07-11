using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PriorityButton")]
public class PriorityButton : KMonoBehaviour
{
	public PrioritySetting priority
	{
		get
		{
			return this._priority;
		}
		set
		{
			this._priority = value;
			if (this.its != null)
			{
				if (this.priority.priority_class == PriorityScreen.PriorityClass.high)
				{
					this.its.colorStyleSetting = this.highStyle;
				}
				else
				{
					this.its.colorStyleSetting = this.normalStyle;
				}
				this.its.RefreshColorStyle();
				this.its.ResetColor();
			}
		}
	}

	protected override void OnPrefabInit()
	{
		this.toggle.onClick += this.OnClick;
	}

	private void OnClick()
	{
		if (this.playSelectionSound)
		{
			PriorityScreen.PlayPriorityConfirmSound(this.priority);
		}
		if (this.onClick != null)
		{
			this.onClick(this.priority);
		}
	}

	public KToggle toggle;

	public LocText text;

	public ToolTip tooltip;

	[MyCmpGet]
	private ImageToggleState its;

	public ColorStyleSetting normalStyle;

	public ColorStyleSetting highStyle;

	public bool playSelectionSound = true;

	public Action<PrioritySetting> onClick;

	private PrioritySetting _priority;
}
