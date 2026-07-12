using System;
using Klei.CustomSettings;
using UnityEngine;

public class CustomGameSettingToggleWidget : CustomGameSettingWidget
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle toggle = this.Toggle;
		toggle.onClick = (global::System.Action)Delegate.Combine(toggle.onClick, new global::System.Action(this.ToggleSetting));
	}

	public void Initialize(ToggleSettingConfig config, Func<SettingConfig, SettingLevel> getCurrentSettingCallback, Func<ToggleSettingConfig, SettingLevel> toggleCallback)
	{
		this.config = config;
		this.Label.text = config.label;
		this.ToolTip.toolTip = config.tooltip;
		this.getCurrentSettingCallback = getCurrentSettingCallback;
		this.toggleCallback = toggleCallback;
	}

	public override void Refresh()
	{
		base.Refresh();
		SettingLevel settingLevel = this.getCurrentSettingCallback(this.config);
		this.Toggle.ChangeState(this.config.IsOnLevel(settingLevel.id) ? 1 : 0);
		this.ToggleToolTip.toolTip = settingLevel.tooltip;
	}

	public void ToggleSetting()
	{
		this.toggleCallback(this.config);
		base.Notify();
	}

	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private MultiToggle Toggle;

	[SerializeField]
	private ToolTip ToggleToolTip;

	private ToggleSettingConfig config;

	protected Func<SettingConfig, SettingLevel> getCurrentSettingCallback;

	protected Func<ToggleSettingConfig, SettingLevel> toggleCallback;
}
