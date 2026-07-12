using System;
using Klei.CustomSettings;
using UnityEngine;

public class CustomGameSettingListWidget : CustomGameSettingWidget
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.CycleLeft.onClick += this.DoCycleLeft;
		this.CycleRight.onClick += this.DoCycleRight;
	}

	public void Initialize(ListSettingConfig config, Func<SettingConfig, SettingLevel> getCallback, Func<ListSettingConfig, int, SettingLevel> cycleCallback)
	{
		this.config = config;
		this.Label.text = config.label;
		this.ToolTip.toolTip = config.tooltip;
		this.getCallback = getCallback;
		this.cycleCallback = cycleCallback;
	}

	public override void Refresh()
	{
		base.Refresh();
		SettingLevel settingLevel = this.getCallback(this.config);
		this.ValueLabel.text = settingLevel.label;
		this.ValueToolTip.toolTip = settingLevel.tooltip;
		this.CycleLeft.isInteractable = !this.config.IsFirstLevel(settingLevel.id);
		this.CycleRight.isInteractable = !this.config.IsLastLevel(settingLevel.id);
	}

	private void DoCycleLeft()
	{
		this.cycleCallback(this.config, -1);
		base.Notify();
	}

	private void DoCycleRight()
	{
		this.cycleCallback(this.config, 1);
		base.Notify();
	}

	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private LocText ValueLabel;

	[SerializeField]
	private ToolTip ValueToolTip;

	[SerializeField]
	private KButton CycleLeft;

	[SerializeField]
	private KButton CycleRight;

	private ListSettingConfig config;

	protected Func<ListSettingConfig, int, SettingLevel> cycleCallback;

	protected Func<SettingConfig, SettingLevel> getCallback;
}
