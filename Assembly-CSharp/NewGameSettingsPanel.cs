using System;
using System.Collections.Generic;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using ProcGenGame;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NewGameSettingsPanel")]
public class NewGameSettingsPanel : KMonoBehaviour
{
	public void SetCloseAction(global::System.Action onClose)
	{
		if (this.closeButton != null)
		{
			this.closeButton.onClick += onClose;
		}
		if (this.background != null)
		{
			this.background.onClick += onClose;
		}
	}

	public void Init()
	{
		Global.Instance.modManager.Load(Content.LayerableFiles);
		SettingsCache.Clear();
		WorldGen.LoadSettings();
		CustomGameSettings.Instance.LoadClusters();
		Global.Instance.modManager.Report(base.gameObject);
		this.settings = CustomGameSettings.Instance;
		this.widgets = new List<NewGameSettingWidget>();
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.settings.QualitySettings)
		{
			if ((!keyValuePair.Value.debug_only || DebugHandler.enabled) && (!keyValuePair.Value.editor_only || Application.isEditor) && DlcManager.IsContentActive(keyValuePair.Value.required_content))
			{
				ListSettingConfig listSettingConfig = keyValuePair.Value as ListSettingConfig;
				if (listSettingConfig != null)
				{
					NewGameSettingList newGameSettingList = global::Util.KInstantiateUI<NewGameSettingList>(this.prefab_cycle_setting, this.content.gameObject, true);
					newGameSettingList.Initialize(listSettingConfig, this, keyValuePair.Value.missing_content_default);
					this.widgets.Add(newGameSettingList);
				}
				else
				{
					ToggleSettingConfig toggleSettingConfig = keyValuePair.Value as ToggleSettingConfig;
					if (toggleSettingConfig != null)
					{
						NewGameSettingToggle newGameSettingToggle = global::Util.KInstantiateUI<NewGameSettingToggle>(this.prefab_checkbox_setting, this.content.gameObject, true);
						newGameSettingToggle.Initialize(toggleSettingConfig, this, keyValuePair.Value.missing_content_default);
						this.widgets.Add(newGameSettingToggle);
					}
					else
					{
						SeedSettingConfig seedSettingConfig = keyValuePair.Value as SeedSettingConfig;
						if (seedSettingConfig != null)
						{
							NewGameSettingSeed newGameSettingSeed = global::Util.KInstantiateUI<NewGameSettingSeed>(this.prefab_seed_input_setting, this.content.gameObject, true);
							newGameSettingSeed.Initialize(seedSettingConfig);
							this.widgets.Add(newGameSettingSeed);
						}
					}
				}
			}
		}
		this.Refresh();
	}

	public void Refresh()
	{
		foreach (NewGameSettingWidget newGameSettingWidget in this.widgets)
		{
			newGameSettingWidget.Refresh();
		}
	}

	public void ConsumeSettingsCode(string code)
	{
		this.settings.ParseAndApplySettingsCode(code);
	}

	public void SetSetting(SettingConfig setting, string level)
	{
		this.settings.SetQualitySetting(setting, level);
	}

	public string GetSetting(SettingConfig setting)
	{
		return this.settings.GetCurrentQualitySetting(setting).id;
	}

	public void Cancel()
	{
		Global.Instance.modManager.Unload(Content.LayerableFiles);
		SettingsCache.Clear();
	}

	[SerializeField]
	private Transform content;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton background;

	[Header("Prefab UI Refs")]
	[SerializeField]
	private GameObject prefab_cycle_setting;

	[SerializeField]
	private GameObject prefab_slider_setting;

	[SerializeField]
	private GameObject prefab_checkbox_setting;

	[SerializeField]
	private GameObject prefab_seed_input_setting;

	private CustomGameSettings settings;

	private List<NewGameSettingWidget> widgets;
}
