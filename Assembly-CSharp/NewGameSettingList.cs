using System;
using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingList : NewGameSettingWidget
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.CycleLeft.onClick += this.DoCycleLeft;
		this.CycleRight.onClick += this.DoCycleRight;
	}

	public void Initialize(ListSettingConfig config)
	{
		this.config = config;
		this.Label.text = config.label;
		this.ToolTip.toolTip = config.tooltip;
	}

	public override void Refresh()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(this.config);
		this.ValueLabel.text = currentQualitySetting.label;
		this.ValueToolTip.toolTip = currentQualitySetting.tooltip;
		this.CycleLeft.isInteractable = !this.config.IsFirstLevel(currentQualitySetting.id);
		this.CycleRight.isInteractable = !this.config.IsLastLevel(currentQualitySetting.id);
	}

	private void DoCycleLeft()
	{
		CustomGameSettings.Instance.CycleSettingLevel(this.config, -1);
		this.Refresh();
	}

	private void DoCycleRight()
	{
		CustomGameSettings.Instance.CycleSettingLevel(this.config, 1);
		this.Refresh();
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

	[SerializeField]
	private Image BG;

	private ListSettingConfig config;
}
