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

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ContextMenu("Apply Color Style Settings")]
	public void ApplyColorStyleSetting()
	{
		if (this.colorStyleSetting != null)
		{
			switch (this.colorSelector)
			{
			case KImage.ColorSelector.Active:
				this.color = this.colorStyleSetting.activeColor;
				return;
			case KImage.ColorSelector.Inactive:
				this.color = this.colorStyleSetting.inactiveColor;
				return;
			case KImage.ColorSelector.Disabled:
				this.color = this.colorStyleSetting.disabledColor;
				return;
			case KImage.ColorSelector.Hover:
				this.color = this.colorStyleSetting.hoverColor;
				break;
			default:
				return;
			}
		}
	}

	public KImage.ColorSelector defaultState = KImage.ColorSelector.Inactive;

	private KImage.ColorSelector colorSelector = KImage.ColorSelector.Inactive;

	public ColorStyleSetting colorStyleSetting;

	public bool clearMaskOnDisable = true;

	public enum ColorSelector
	{
		Active,
		Inactive,
		Disabled,
		Hover
	}
}
