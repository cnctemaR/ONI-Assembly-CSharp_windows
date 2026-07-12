using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/CustomGameSettings")]
public class CustomGameSettings : KMonoBehaviour
{
	public static CustomGameSettings Instance
	{
		get
		{
			return CustomGameSettings.instance;
		}
	}

	public event Action<SettingConfig, SettingLevel> OnSettingChanged;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			this.customGameMode = (this.is_custom_game ? CustomGameSettings.CustomGameMode.Custom : CustomGameSettings.CustomGameMode.Survival);
		}
		if (this.CurrentQualityLevelsBySetting.ContainsKey("CarePackages "))
		{
			if (!this.CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
			{
				this.CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.CarePackages.id, this.CurrentQualityLevelsBySetting["CarePackages "]);
			}
			this.CurrentQualityLevelsBySetting.Remove("CarePackages ");
		}
		this.CurrentQualityLevelsBySetting.Remove("Expansion1Active");
		if (!DlcManager.IsExpansion1Active())
		{
			foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
			{
				SettingConfig value = keyValuePair.Value;
				if (!DlcManager.IsVanillaId(value.required_content))
				{
					global::Debug.Assert(value.required_content == "EXPANSION1_ID", "A new expansion setting has been added, but its deserialization has not been implemented.");
					if (this.CurrentQualityLevelsBySetting.ContainsKey(value.id))
					{
						global::Debug.Assert(this.CurrentQualityLevelsBySetting[value.id] == value.missing_content_default, string.Format("This save has Expansion1 content disabled, but its expansion1-dependent setting {0} is set to {1}", value.id, this.CurrentQualityLevelsBySetting[value.id]));
					}
					else
					{
						this.SetQualitySetting(value, value.missing_content_default);
					}
				}
			}
		}
		string clusterDefaultName;
		this.CurrentQualityLevelsBySetting.TryGetValue(CustomGameSettingConfigs.ClusterLayout.id, out clusterDefaultName);
		if (clusterDefaultName.IsNullOrWhiteSpace())
		{
			DebugUtil.DevAssert(!DlcManager.IsExpansion1Active(), "Deserializing CustomGameSettings.ClusterLayout: ClusterLayout is blank, using default cluster instead", null);
			clusterDefaultName = WorldGenSettings.ClusterDefaultName;
			this.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, clusterDefaultName);
		}
		if (!SettingsCache.clusterLayouts.clusterCache.ContainsKey(clusterDefaultName))
		{
			global::Debug.Log("Deserializing CustomGameSettings.ClusterLayout: '" + clusterDefaultName + "' doesn't exist in the clusterCache, trying to rewrite path to scoped path.");
			string text = SettingsCache.GetScope("EXPANSION1_ID") + clusterDefaultName;
			if (SettingsCache.clusterLayouts.clusterCache.ContainsKey(text))
			{
				global::Debug.Log(string.Concat(new string[] { "Deserializing CustomGameSettings.ClusterLayout: Success in rewriting ClusterLayout '", clusterDefaultName, "' to '", text, "'" }));
				this.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, text);
			}
			else
			{
				global::Debug.LogWarning("Deserializing CustomGameSettings.ClusterLayout: Failed to find cluster '" + clusterDefaultName + "' including the scoped path, setting to default cluster name.");
				global::Debug.Log("ClusterCache: " + string.Join(",", SettingsCache.clusterLayouts.clusterCache.Keys));
				this.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, WorldGenSettings.ClusterDefaultName);
			}
		}
		this.CheckCustomGameMode();
	}

	protected override void OnPrefabInit()
	{
		bool flag = DlcManager.IsExpansion1Active();
		CustomGameSettings.instance = this;
		this.AddSettingConfig(CustomGameSettingConfigs.ClusterLayout);
		this.AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		this.AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		this.AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		this.AddSettingConfig(CustomGameSettingConfigs.Morale);
		this.AddSettingConfig(CustomGameSettingConfigs.Durability);
		if (flag)
		{
			this.AddSettingConfig(CustomGameSettingConfigs.Radiation);
		}
		this.AddSettingConfig(CustomGameSettingConfigs.Stress);
		this.AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		this.AddSettingConfig(CustomGameSettingConfigs.CarePackages);
		this.AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		this.AddSettingConfig(CustomGameSettingConfigs.FastWorkersMode);
		if (SaveLoader.GetCloudSavesAvailable())
		{
			this.AddSettingConfig(CustomGameSettingConfigs.SaveToCloud);
		}
		if (flag)
		{
			this.AddSettingConfig(CustomGameSettingConfigs.Teleporters);
		}
		this.VerifySettingCoordinates();
	}

	public void SetSurvivalDefaults()
	{
		this.customGameMode = CustomGameSettings.CustomGameMode.Survival;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.SetQualitySetting(keyValuePair.Value, keyValuePair.Value.GetDefaultLevelId());
		}
	}

	public void SetNosweatDefaults()
	{
		this.customGameMode = CustomGameSettings.CustomGameMode.Nosweat;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.SetQualitySetting(keyValuePair.Value, keyValuePair.Value.GetNoSweatDefaultLevelId());
		}
	}

	public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
	{
		this.SetQualitySetting(config, config.CycleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id], direction));
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
	{
		this.SetQualitySetting(config, config.ToggleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id]));
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
	}

	public void SetQualitySetting(SettingConfig config, string value)
	{
		this.CurrentQualityLevelsBySetting[config.id] = value;
		this.CheckCustomGameMode();
		if (this.OnSettingChanged != null)
		{
			this.OnSettingChanged(config, this.GetCurrentQualitySetting(config));
		}
	}

	private void CheckCustomGameMode()
	{
		bool flag = true;
		bool flag2 = true;
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			if (!this.QualitySettings.ContainsKey(keyValuePair.Key))
			{
				DebugUtil.LogWarningArgs(new object[] { "Quality settings missing " + keyValuePair.Key });
			}
			else if (this.QualitySettings[keyValuePair.Key].triggers_custom_game)
			{
				if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].GetDefaultLevelId())
				{
					flag = false;
				}
				if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].GetNoSweatDefaultLevelId())
				{
					flag2 = false;
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
		}
		CustomGameSettings.CustomGameMode customGameMode;
		if (flag)
		{
			customGameMode = CustomGameSettings.CustomGameMode.Survival;
		}
		else if (flag2)
		{
			customGameMode = CustomGameSettings.CustomGameMode.Nosweat;
		}
		else
		{
			customGameMode = CustomGameSettings.CustomGameMode.Custom;
		}
		if (customGameMode != this.customGameMode)
		{
			DebugUtil.LogArgs(new object[] { "Game mode changed from", this.customGameMode, "to", customGameMode });
			this.customGameMode = customGameMode;
		}
	}

	public SettingLevel GetCurrentQualitySetting(SettingConfig setting)
	{
		return this.GetCurrentQualitySetting(setting.id);
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		SettingConfig settingConfig = this.QualitySettings[setting_id];
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Survival && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.GetDefaultLevelId());
		}
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.GetNoSweatDefaultLevelId());
		}
		if (!this.CurrentQualityLevelsBySetting.ContainsKey(setting_id))
		{
			this.CurrentQualityLevelsBySetting[setting_id] = this.QualitySettings[setting_id].GetDefaultLevelId();
		}
		string text = (DlcManager.IsContentActive(settingConfig.required_content) ? this.CurrentQualityLevelsBySetting[setting_id] : settingConfig.GetDefaultLevelId());
		return this.QualitySettings[setting_id].GetLevel(text);
	}

	public string GetCurrentQualitySettingLevelId(SettingConfig config)
	{
		return this.CurrentQualityLevelsBySetting[config.id];
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
		global::Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id);
		return "";
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
		global::Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id);
		return "";
	}

	public void AddSettingConfig(SettingConfig config)
	{
		this.QualitySettings.Add(config.id, config);
		if (!this.CurrentQualityLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(this.CurrentQualityLevelsBySetting[config.id]))
		{
			this.CurrentQualityLevelsBySetting[config.id] = config.GetDefaultLevelId();
		}
	}

	public void LoadClusters()
	{
		Dictionary<string, ClusterLayout> clusterCache = SettingsCache.clusterLayouts.clusterCache;
		List<SettingLevel> list = new List<SettingLevel>(clusterCache.Count);
		foreach (KeyValuePair<string, ClusterLayout> keyValuePair in clusterCache)
		{
			StringEntry stringEntry;
			string text = (Strings.TryGet(new StringKey(keyValuePair.Value.name), out stringEntry) ? stringEntry.ToString() : keyValuePair.Value.name);
			string text2 = (Strings.TryGet(new StringKey(keyValuePair.Value.description), out stringEntry) ? stringEntry.ToString() : keyValuePair.Value.description);
			list.Add(new SettingLevel(keyValuePair.Key, text, text2, 0, null));
		}
		CustomGameSettingConfigs.ClusterLayout.StompLevels(list, WorldGenSettings.ClusterDefaultName, WorldGenSettings.ClusterDefaultName);
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			text = string.Concat(new string[] { text, keyValuePair.Key, "=", keyValuePair.Value, "," });
		}
		global::Debug.Log(text);
	}

	private bool AllValuesMatch(Dictionary<string, string> data, CustomGameSettings.CustomGameMode mode)
	{
		bool flag = true;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			if (!(keyValuePair.Key == CustomGameSettingConfigs.WorldgenSeed.id))
			{
				string text = null;
				if (mode != CustomGameSettings.CustomGameMode.Survival)
				{
					if (mode == CustomGameSettings.CustomGameMode.Nosweat)
					{
						text = keyValuePair.Value.GetNoSweatDefaultLevelId();
					}
				}
				else
				{
					text = keyValuePair.Value.GetDefaultLevelId();
				}
				if (data.ContainsKey(keyValuePair.Key) && data[keyValuePair.Key] != text)
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
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
		CustomGameSettings.MetricSettingsData metricSettingsData = new CustomGameSettings.MetricSettingsData
		{
			Name = "CustomGameModeActual",
			Value = CustomGameSettings.CustomGameMode.Custom.ToString()
		};
		foreach (object obj in Enum.GetValues(typeof(CustomGameSettings.CustomGameMode)))
		{
			CustomGameSettings.CustomGameMode customGameMode = (CustomGameSettings.CustomGameMode)obj;
			if (customGameMode != CustomGameSettings.CustomGameMode.Custom && this.AllValuesMatch(this.CurrentQualityLevelsBySetting, customGameMode))
			{
				metricSettingsData.Value = customGameMode.ToString();
				break;
			}
		}
		list.Add(metricSettingsData);
		return list;
	}

	public bool VerifySettingCoordinates()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		bool flag = false;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			if (keyValuePair.Value.coordinate_dimension < 0 || keyValuePair.Value.coordinate_dimension_width < 0)
			{
				if (keyValuePair.Value.coordinate_dimension >= 0 || keyValuePair.Value.coordinate_dimension_width >= 0)
				{
					flag = true;
					global::Debug.Assert(false, keyValuePair.Value.id + ": Both coordinate dimension props must be unset (-1) if either is unset.");
				}
			}
			else
			{
				List<SettingLevel> levels = keyValuePair.Value.GetLevels();
				if (keyValuePair.Value.coordinate_dimension_width < levels.Count)
				{
					flag = true;
					global::Debug.Assert(false, string.Concat(new string[]
					{
						keyValuePair.Value.id,
						": Range between coordinate min and max insufficient for all levels (",
						keyValuePair.Value.coordinate_dimension_width.ToString(),
						"<",
						levels.Count.ToString(),
						")"
					}));
				}
				foreach (SettingLevel settingLevel in levels)
				{
					int num = keyValuePair.Value.coordinate_dimension * settingLevel.coordinate_offset;
					string text = keyValuePair.Value.id + " > " + settingLevel.id;
					if (settingLevel.coordinate_offset < 0)
					{
						flag = true;
						global::Debug.Assert(false, text + ": Level coordinate offset must be >= 0");
					}
					else if (settingLevel.coordinate_offset == 0)
					{
						if (settingLevel.id != keyValuePair.Value.GetDefaultLevelId())
						{
							flag = true;
							global::Debug.Assert(false, text + ": Only the default level should have a coordinate offset of 0");
						}
					}
					else if (settingLevel.coordinate_offset > keyValuePair.Value.coordinate_dimension_width)
					{
						flag = true;
						global::Debug.Assert(false, text + ": level coordinate must be <= dimension width");
					}
					else
					{
						string text2;
						bool flag2 = !dictionary.TryGetValue(num, out text2);
						dictionary[num] = text;
						if (settingLevel.id == keyValuePair.Value.GetDefaultLevelId())
						{
							flag = true;
							global::Debug.Assert(false, text + ": Default level must be coordinate 0");
						}
						if (!flag2)
						{
							flag = true;
							global::Debug.Assert(false, text + ": Combined coordinate conflicts with another coordinate (" + text2 + "). Ensure this SettingConfig's min and max don't overlap with another SettingConfig's");
						}
					}
				}
			}
		}
		return flag;
	}

	public static string[] ParseSettingCoordinate(string coord)
	{
		Match match = new Regex("(.*)-(.*)-(.*)").Match(coord);
		string[] array = new string[match.Groups.Count];
		for (int i = 0; i < match.Groups.Count; i++)
		{
			array[i] = match.Groups[i].Value;
		}
		return array;
	}

	public string GetSettingsCoordinate()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout);
		if (currentQualitySetting == null)
		{
			DebugUtil.DevLogError("GetSettingsCoordinate: clusterLayoutSetting is null, returning '0' coordinate");
			CustomGameSettings.Instance.Print();
			global::Debug.Log("ClusterCache: " + string.Join(",", SettingsCache.clusterLayouts.clusterCache.Keys));
			return "0-0-0";
		}
		ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = this.GetOtherSettingsCode();
		return string.Format("{0}-{1}-{2}", clusterData.GetCoordinatePrefix(), currentQualitySetting2.id, otherSettingsCode);
	}

	public void ParseAndApplySettingsCode(string code)
	{
		int num = this.Base36toBase10(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig = this.QualitySettings[keyValuePair.Key];
			if (settingConfig.coordinate_dimension >= 0 && settingConfig.coordinate_dimension_width >= 0)
			{
				int num2 = 0;
				int num3 = settingConfig.coordinate_dimension * settingConfig.coordinate_dimension_width;
				int num4 = num;
				if (num4 >= num3)
				{
					int num5 = num4 / num3 * num3;
					num4 -= num5;
				}
				if (num4 >= settingConfig.coordinate_dimension)
				{
					num2 = num4 / settingConfig.coordinate_dimension;
				}
				foreach (SettingLevel settingLevel in settingConfig.GetLevels())
				{
					if (settingLevel.coordinate_offset == num2)
					{
						dictionary[settingConfig] = settingLevel.id;
						break;
					}
				}
			}
		}
		foreach (KeyValuePair<SettingConfig, string> keyValuePair2 in dictionary)
		{
			this.SetQualitySetting(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	private string GetOtherSettingsCode()
	{
		int num = 0;
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig;
			this.QualitySettings.TryGetValue(keyValuePair.Key, out settingConfig);
			if (settingConfig != null && settingConfig.coordinate_dimension >= 0 && settingConfig.coordinate_dimension_width >= 0)
			{
				SettingLevel level = settingConfig.GetLevel(keyValuePair.Value);
				int num2 = settingConfig.coordinate_dimension * level.coordinate_offset;
				num += num2;
			}
		}
		return this.Base10toBase36(num);
	}

	private int Base36toBase10(string input)
	{
		if (input == "0")
		{
			return 0;
		}
		int num = 0;
		for (int i = input.Length - 1; i >= 0; i--)
		{
			num *= 36;
			int num2 = this.hexChars.IndexOf(input[i]);
			num += num2;
		}
		return num;
	}

	private string Base10toBase36(int input)
	{
		if (input == 0)
		{
			return "0";
		}
		int i = input;
		string text = "";
		while (i > 0)
		{
			text += this.hexChars[i % 36].ToString();
			i /= 36;
		}
		return text;
	}

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameSettings.CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	private string hexChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
