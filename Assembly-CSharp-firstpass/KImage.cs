using System;
using UnityEngine;
using UnityEngine.UI;

public class KImage : Image
{
	public KImage.ColorSelector ColorState
	{
		set
		{
			this.colorSelector = value;
			this.ApplyColorStyleSetting();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.ColorState = this.defaultState;
	}

	[ContextMenu("Apply Color Style Settings")]
	private void ApplyColorStyleSetting()
	{
		if (this.colorStyleSetting != null)
		{
			switch (this.colorSelector)
			{
			case KImage.ColorSelector.Active:
				this.color = this.colorStyleSetting.activeColor;
				break;
			case KImage.ColorSelector.Inactive:
				this.color = this.colorStyleSetting.inactiveColor;
				break;
			case KImage.ColorSelector.Disabled:
				this.color = this.colorStyleSetting.disabledColor;
				break;
			case KImage.ColorSelector.Hover:
				this.color = this.colorStyleSetting.hoverColor;
				break;
			}
		}
	}

	public KImage.ColorSelector defaultState = KImage.ColorSelector.Inactive;

	private KImage.ColorSelector colorSelector = KImage.ColorSelector.Inactive;

	public ColorStyleSetting colorStyleSetting;

	public enum ColorSelector
	{
		Active,
		Inactive,
		Disabled,
		Hover
	}
}
