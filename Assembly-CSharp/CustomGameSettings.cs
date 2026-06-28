using System;
using System.Collections.Generic;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CustomGameSettings : KMonoBehaviour
{
	public static CustomGameSettings Get()
	{
		return CustomGameSettings.instance;
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		if (this.is_custom_game)
		{
			string empty = string.Empty;
			this.CurrentQualityLevelsBySetting.TryGetValue(setting_id, out empty);
			return this.QualitySettings[setting_id].GetLevel(empty);
		}
		return this.QualitySettings[setting_id].GetLevel(this.QualitySettings[setting_id].default_level_id);
	}

	public string GetSettingLevelLabel(string setting_id, string level_id)
	{
		SettingConfig settingConfig = this.QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.label;
			}
		}
		global::Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	public string GetSettingLevelTooltip(string setting_id, string level_id)
	{
		SettingConfig settingConfig = this.QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.tooltip;
			}
		}
		global::Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	protected override void OnPrefabInit()
	{
		if (CustomGameSettings.instance != null)
		{
			global::UnityEngine.Object.DestroyImmediate(CustomGameSettings.instance.gameObject);
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		CustomGameSettings.instance = this;
		this.AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		this.AddSettingConfig(CustomGameSettingConfigs.Stress);
		this.AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		this.AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		if (DebugHandler.enabled)
		{
			CustomGameSettings.Get().InitWorldGenOptions();
		}
		this.Reset();
	}

	public void AddSettingConfig(SettingConfig config)
	{
		this.QualitySettings.Add(config.id, config);
		if (!this.CurrentQualityLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(this.CurrentQualityLevelsBySetting[config.id]))
		{
			this.CurrentQualityLevelsBySetting[config.id] = config.default_level_id;
		}
	}

	public void InitWorldGenOptions()
	{
		WorldGen.LoadSettings();
		Dictionary<string, global::ProcGen.World> worlds = WorldGen.Settings.GetWorlds();
		if (worlds.Count > 1)
		{
			List<SettingLevel> list = new List<SettingLevel>();
			foreach (KeyValuePair<string, global::ProcGen.World> keyValuePair in worlds)
			{
				list.Add(new SettingLevel(keyValuePair.Key, keyValuePair.Value.name, keyValuePair.Value.description));
			}
			List<string> worldNames = WorldGen.Settings.GetWorldNames();
			SettingConfig settingConfig = new ListSettingConfig("World", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.TOOLTIP, list, worldNames[0]);
			this.AddSettingConfig(settingConfig);
		}
	}

	public void Reset()
	{
		this.is_custom_game = false;
		this.CurrentQualityLevelsBySetting.Clear();
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.CurrentQualityLevelsBySetting.Add(keyValuePair.Value.id, keyValuePair.Value.default_level_id);
		}
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			string text2 = text;
			text = string.Concat(new string[] { text2, keyValuePair.Key, "=", keyValuePair.Value, "," });
		}
		global::Debug.Log(text, null);
	}

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();
}
