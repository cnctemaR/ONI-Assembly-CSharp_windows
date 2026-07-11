using System;
using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingToggle : NewGameSettingWidget
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle toggle = this.Toggle;
		toggle.onClick = (global::System.Action)Delegate.Combine(toggle.onClick, new global::System.Action(this.ToggleSetting));
	}

	public void Initialize(ToggleSettingConfig config)
	{
		this.config = config;
		this.Label.text = config.label;
		this.ToolTip.toolTip = config.tooltip;
	}

	public override void Refresh()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(this.config);
		this.Toggle.ChangeState(this.config.IsOnLevel(currentQualitySetting.id) ? 1 : 0);
		this.ToggleToolTip.toolTip = currentQualitySetting.tooltip;
	}

	public void ToggleSetting()
	{
		CustomGameSettings.Instance.ToggleSettingLevel(this.config);
		this.Refresh();
	}

	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private MultiToggle Toggle;

	[SerializeField]
	private ToolTip ToggleToolTip;

	[SerializeField]
	private Image BG;

	private ToggleSettingConfig config;
}
