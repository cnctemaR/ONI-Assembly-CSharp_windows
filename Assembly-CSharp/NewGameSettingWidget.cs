using System;
using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public abstract class NewGameSettingWidget : KMonoBehaviour
{
	protected virtual void Initialize(SettingConfig config, NewGameSettingsPanel panel, string disabledDefault)
	{
		this.config = config;
		this.panel = panel;
		this.disabledDefault = disabledDefault;
	}

	public virtual void Refresh()
	{
		bool flag = this.ShouldBeEnabled();
		if (flag == this.widget_enabled)
		{
			return;
		}
		this.widget_enabled = flag;
		if (this.IsEnabled())
		{
			this.BG.color = this.enabledColor;
			CustomGameSettings.Instance.SetQualitySetting(this.config, this.config.GetDefaultLevelId());
			return;
		}
		CustomGameSettings.Instance.SetQualitySetting(this.config, this.disabledDefault);
		this.BG.color = this.disabledColor;
	}

	protected void RefreshAll()
	{
		this.panel.Refresh();
	}

	protected bool IsEnabled()
	{
		return this.widget_enabled;
	}

	private bool ShouldBeEnabled()
	{
		return true;
	}

	[SerializeField]
	private Image BG;

	[SerializeField]
	private Color enabledColor;

	[SerializeField]
	private Color disabledColor;

	private SettingConfig config;

	private NewGameSettingsPanel panel;

	private string disabledDefault;

	private bool widget_enabled = true;
}
