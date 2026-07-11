using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using ProcGenGame;

[SerializationConfig(MemberSerialization.OptIn)]
public class CustomGameSettings : KMonoBehaviour
{
	public static CustomGameSettings Instance
	{
		get
		{
			return CustomGameSettings.instance;
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			this.customGameMode = ((!this.is_custom_game) ? CustomGameSettings.CustomGameMode.Survival : CustomGameSettings.CustomGameMode.Custom);
		}
	}

	protected override void OnPrefabInit()
	{
		CustomGameSettings.instance = this;
		this.AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		this.AddSettingConfig(CustomGameSettingConfigs.Stress);
		this.AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		this.AddSettingConfig(CustomGameSettingConfigs.Morale);
		this.AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		this.AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		this.AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		this.InitWorldGenOptions();
		this.AddSettingConfig(CustomGameSettingConfigs.World);
	}

	public void SetSurvivalDefaults()
	{
		this.customGameMode = CustomGameSettings.CustomGameMode.Survival;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.SetQualitySetting(keyValuePair.Value, keyValuePair.Value.default_level_id);
		}
	}

	public void SetNosweatDefaults()
	{
		this.customGameMode = CustomGameSettings.CustomGameMode.Nosweat;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.SetQualitySetting(keyValuePair.Value, keyValuePair.Value.nosweat_default_level_id);
		}
	}

	public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
	{
		this.CurrentQualityLevelsBySetting[config.id] = config.CycleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id], direction);
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
	{
		this.CurrentQualityLevelsBySetting[config.id] = config.ToggleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id]);
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
	}

	public void SetQualitySetting(SettingConfig config, string value)
	{
		this.CurrentQualityLevelsBySetting[config.id] = value;
	}

	public SettingLevel GetCurrentQualitySetting(SettingConfig setting)
	{
		return this.GetCurrentQualitySetting(setting.id);
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Survival)
		{
			return this.QualitySettings[setting_id].GetLevel(this.QualitySettings[setting_id].default_level_id);
		}
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat)
		{
			return this.QualitySettings[setting_id].GetLevel(this.QualitySettings[setting_id].nosweat_default_level_id);
		}
		if (!this.CurrentQualityLevelsBySetting.ContainsKey(setting_id))
		{
			this.CurrentQualityLevelsBySetting[setting_id] = this.QualitySettings[setting_id].default_level_id;
		}
		string text = this.CurrentQualityLevelsBySetting[setting_id];
		return this.QualitySettings[setting_id].GetLevel(text);
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
		Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id, null);
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
		Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
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
			ListSettingConfig listSettingConfig = (ListSettingConfig)CustomGameSettingConfigs.World;
			listSettingConfig.StompLevels(list, "worlds/Default", "worlds/Default");
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
		Debug.Log(text, null);
	}

	public List<CustomGameSettings.MetricSettingsData> GetSettingsForMetrics()
	{
		List<CustomGameSettings.MetricSettingsData> list = new List<CustomGameSettings.MetricSettingsData>();
		list.Add(new CustomGameSettings.MetricSettingsData
		{
			Name = "CustomGameMode",
			Value = this.customGameMode.ToString()
		});
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			list.Add(new CustomGameSettings.MetricSettingsData
			{
				Name = keyValuePair.Key,
				Value = keyValuePair.Value
			});
		}
		return list;
	}

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameSettings.CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	public enum CustomGameMode
	{
		Survival,
		Nosweat,
		Custom = 255
	}

	public struct MetricSettingsData
	{
		public string Name;

		public string Value;
	}
}
