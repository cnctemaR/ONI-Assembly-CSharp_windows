using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Database;
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

	public IReadOnlyDictionary<string, string> CurrentStoryLevelsBySetting
	{
		get
		{
			return this.currentStoryLevelsBySetting;
		}
	}

	public event Action<SettingConfig, SettingLevel> OnQualitySettingChanged;

	public event Action<SettingConfig, SettingLevel> OnStorySettingChanged;

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
		this.AddQualitySettingConfig(CustomGameSettingConfigs.ClusterLayout);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.CalorieBurn);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.Morale);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.Durability);
		if (flag)
		{
			this.AddQualitySettingConfig(CustomGameSettingConfigs.Radiation);
		}
		this.AddQualitySettingConfig(CustomGameSettingConfigs.Stress);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.StressBreaks);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.CarePackages);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.SandboxMode);
		this.AddQualitySettingConfig(CustomGameSettingConfigs.FastWorkersMode);
		if (SaveLoader.GetCloudSavesAvailable())
		{
			this.AddQualitySettingConfig(CustomGameSettingConfigs.SaveToCloud);
		}
		if (flag)
		{
			this.AddQualitySettingConfig(CustomGameSettingConfigs.Teleporters);
		}
		foreach (Story story in Db.Get().Stories.resources)
		{
			long num = (long)((story.kleiUseOnlyCoordinateOffset == -1) ? (-1) : global::Util.IntPow(3, story.kleiUseOnlyCoordinateOffset));
			int num2 = ((story.kleiUseOnlyCoordinateOffset == -1) ? (-1) : 3);
			SettingConfig settingConfig = new ListSettingConfig(story.Id, "", "", new List<SettingLevel>
			{
				new SettingLevel("Disabled", "", "", 0L, null),
				new SettingLevel("Guaranteed", "", "", 1L, null)
			}, "Disabled", "Disabled", num, (long)num2, false, false, "", "", false);
			this.AddStorySettingConfig(settingConfig);
		}
		this.VerifySettingCoordinates();
	}

	public void DisableAllStories()
	{
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.StorySettings)
		{
			this.SetStorySetting(keyValuePair.Value, false);
		}
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
		if (this.OnQualitySettingChanged != null)
		{
			this.OnQualitySettingChanged(config, this.GetCurrentQualitySetting(config));
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

	public string GetQualitySettingLevelTooltip(string setting_id, string level_id)
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

	public void AddQualitySettingConfig(SettingConfig config)
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
			list.Add(new SettingLevel(keyValuePair.Key, text, text2, 0L, null));
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
		text = "Story Settings: ";
		foreach (KeyValuePair<string, string> keyValuePair2 in this.currentStoryLevelsBySetting)
		{
			text = string.Concat(new string[] { text, keyValuePair2.Key, "=", keyValuePair2.Value, "," });
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
		bool flag = this.VerifySettingsDictionary(this.QualitySettings);
		bool flag2 = this.VerifySettingsDictionary(this.StorySettings);
		return flag || flag2;
	}

	private bool VerifySettingsDictionary(Dictionary<string, SettingConfig> configs)
	{
		Dictionary<long, string> dictionary = new Dictionary<long, string>();
		bool flag = false;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in configs)
		{
			if (keyValuePair.Value.coordinate_dimension < 0L || keyValuePair.Value.coordinate_dimension_width < 0L)
			{
				if (keyValuePair.Value.coordinate_dimension >= 0L || keyValuePair.Value.coordinate_dimension_width >= 0L)
				{
					flag = true;
					global::Debug.Assert(false, keyValuePair.Value.id + ": Both coordinate dimension props must be unset (-1) if either is unset.");
				}
			}
			else
			{
				List<SettingLevel> levels = keyValuePair.Value.GetLevels();
				if (keyValuePair.Value.coordinate_dimension_width < (long)levels.Count)
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
					long num = keyValuePair.Value.coordinate_dimension * settingLevel.coordinate_offset;
					string text = keyValuePair.Value.id + " > " + settingLevel.id;
					if (settingLevel.coordinate_offset < 0L)
					{
						flag = true;
						global::Debug.Assert(false, text + ": Level coordinate offset must be >= 0");
					}
					else if (settingLevel.coordinate_offset == 0L)
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
		string[] array = CustomGameSettings.ParseCoordinate(coord, "(.*)-(\\d*)-(.*)-(.*)");
		if (array.Length == 1)
		{
			array = CustomGameSettings.ParseCoordinate(coord, "(.*)-(\\d*)-(.*)");
		}
		return array;
	}

	private static string[] ParseCoordinate(string coord, string pattern)
	{
		Match match = new Regex(pattern).Match(coord);
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
			return "0-0-0-0";
		}
		ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = this.GetOtherSettingsCode();
		string storyTraitSettingsCode = this.GetStoryTraitSettingsCode();
		return string.Format("{0}-{1}-{2}-{3}", new object[]
		{
			clusterData.GetCoordinatePrefix(),
			currentQualitySetting2.id,
			otherSettingsCode,
			storyTraitSettingsCode
		});
	}

	public void ParseAndApplySettingsCode(string code)
	{
		long num = this.Base36toBase10(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig = this.QualitySettings[keyValuePair.Key];
			if (settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
			{
				long num2 = 0L;
				long num3 = settingConfig.coordinate_dimension * settingConfig.coordinate_dimension_width;
				long num4 = num;
				if (num4 >= num3)
				{
					long num5 = num4 / num3 * num3;
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
		long num = 0L;
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig;
			this.QualitySettings.TryGetValue(keyValuePair.Key, out settingConfig);
			if (settingConfig != null && settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
			{
				SettingLevel level = settingConfig.GetLevel(keyValuePair.Value);
				long num2 = settingConfig.coordinate_dimension * level.coordinate_offset;
				num += num2;
			}
		}
		return this.Base10toBase36(num);
	}

	private long Base36toBase10(string input)
	{
		if (input == "0")
		{
			return 0L;
		}
		long num = 0L;
		for (int i = input.Length - 1; i >= 0; i--)
		{
			num *= 36L;
			long num2 = (long)this.hexChars.IndexOf(input[i]);
			num += num2;
		}
		DebugUtil.LogArgs(new object[]
		{
			"tried converting",
			input,
			", got",
			num,
			"and returns to",
			this.Base10toBase36(num)
		});
		return num;
	}

	private string Base10toBase36(long input)
	{
		if (input == 0L)
		{
			return "0";
		}
		long num = input;
		string text = "";
		while (num > 0L)
		{
			text += this.hexChars[(int)(num % 36L)].ToString();
			num /= 36L;
		}
		return text;
	}

	public void AddStorySettingConfig(SettingConfig config)
	{
		this.StorySettings.Add(config.id, config);
		if (!this.currentStoryLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(this.currentStoryLevelsBySetting[config.id]))
		{
			this.currentStoryLevelsBySetting[config.id] = config.GetDefaultLevelId();
		}
	}

	public void SetStorySetting(SettingConfig config, string value)
	{
		this.SetStorySetting(config, value == "Guaranteed");
	}

	public void SetStorySetting(SettingConfig config, bool value)
	{
		this.currentStoryLevelsBySetting[config.id] = (value ? "Guaranteed" : "Disabled");
		if (this.OnStorySettingChanged != null)
		{
			this.OnStorySettingChanged(config, this.GetCurrentStoryTraitSetting(config));
		}
	}

	public void ParseAndApplyStoryTraitSettingsCode(string code)
	{
		long num = this.Base36toBase10(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
		{
			SettingConfig settingConfig = this.StorySettings[keyValuePair.Key];
			if (settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
			{
				long num2 = 0L;
				long num3 = settingConfig.coordinate_dimension * settingConfig.coordinate_dimension_width;
				long num4 = num;
				if (num4 >= num3)
				{
					long num5 = num4 / num3 * num3;
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
			this.SetStorySetting(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	private string GetStoryTraitSettingsCode()
	{
		long num = 0L;
		foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
		{
			SettingConfig settingConfig;
			this.StorySettings.TryGetValue(keyValuePair.Key, out settingConfig);
			if (settingConfig != null && settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
			{
				SettingLevel level = settingConfig.GetLevel(keyValuePair.Value);
				long num2 = settingConfig.coordinate_dimension * level.coordinate_offset;
				num += num2;
			}
		}
		return this.Base10toBase36(num);
	}

	public SettingLevel GetCurrentStoryTraitSetting(SettingConfig setting)
	{
		return this.GetCurrentStoryTraitSetting(setting.id);
	}

	public SettingLevel GetCurrentStoryTraitSetting(string settingId)
	{
		SettingConfig settingConfig = this.StorySettings[settingId];
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Survival && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.GetDefaultLevelId());
		}
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.GetNoSweatDefaultLevelId());
		}
		if (!this.currentStoryLevelsBySetting.ContainsKey(settingId))
		{
			this.currentStoryLevelsBySetting[settingId] = this.StorySettings[settingId].GetDefaultLevelId();
		}
		string text = (DlcManager.IsContentActive(settingConfig.required_content) ? this.currentStoryLevelsBySetting[settingId] : settingConfig.GetDefaultLevelId());
		return this.StorySettings[settingId].GetLevel(text);
	}

	public List<string> GetCurrentStories()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
		{
			if (this.IsStoryActive(keyValuePair.Key, keyValuePair.Value))
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	public bool IsStoryActive(string id, string level)
	{
		SettingConfig settingConfig;
		return this.StorySettings.TryGetValue(id, out settingConfig) && settingConfig != null && level == "Guaranteed";
	}

	private static CustomGameSettings instance;

	private const int NUM_STORY_LEVELS = 3;

	public const string STORY_DISABLED_LEVEL = "Disabled";

	public const string STORY_GUARANTEED_LEVEL = "Guaranteed";

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameSettings.CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	private Dictionary<string, string> currentStoryLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	public Dictionary<string, SettingConfig> StorySettings = new Dictionary<string, SettingConfig>();

	private const string storyCoordinatePattern = "(.*)-(\\d*)-(.*)-(.*)";

	private const string noStoryCoordinatePattern = "(.*)-(\\d*)-(.*)";

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
