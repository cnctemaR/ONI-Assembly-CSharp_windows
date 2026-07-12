using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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

	public event Action<SettingConfig, SettingLevel> OnMixingSettingChanged;

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
		string clusterDefaultName;
		this.CurrentQualityLevelsBySetting.TryGetValue(CustomGameSettingConfigs.ClusterLayout.id, out clusterDefaultName);
		if (clusterDefaultName.IsNullOrWhiteSpace())
		{
			if (!DlcManager.IsExpansion1Active())
			{
				DebugUtil.LogWarningArgs(new object[] { "Deserializing CustomGameSettings.ClusterLayout: ClusterLayout is blank, using default cluster instead" });
			}
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

	private void AddMissingQualitySettings()
	{
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			SettingConfig value = keyValuePair.Value;
			if (SaveLoader.Instance.IsDlcListActiveForCurrentSave(value.required_content) && !this.CurrentQualityLevelsBySetting.ContainsKey(value.id))
			{
				if (value.missing_content_default != "")
				{
					DebugUtil.LogArgs(new object[] { string.Concat(new string[] { "QualitySetting '", value.id, "' is missing, setting it to missing_content_default '", value.missing_content_default, "'." }) });
					this.SetQualitySetting(value, value.missing_content_default);
				}
				else
				{
					DebugUtil.DevLogError("QualitySetting '" + value.id + "' is missing in this save. Either provide a missing_content_default or handle it in OnDeserialized.");
				}
			}
		}
	}

	protected override void OnPrefabInit()
	{
		DlcManager.IsExpansion1Active();
		Action<SettingConfig> action = delegate(SettingConfig setting)
		{
			this.AddQualitySettingConfig(setting);
			if (setting.coordinate_range >= 0L)
			{
				this.CoordinatedQualitySettings.Add(setting.id);
			}
		};
		Action<SettingConfig> action2 = delegate(SettingConfig setting)
		{
			this.AddStorySettingConfig(setting);
			if (setting.coordinate_range >= 0L)
			{
				this.CoordinatedStorySettings.Add(setting.id);
			}
		};
		Action<SettingConfig> action3 = delegate(SettingConfig setting)
		{
			this.AddMixingSettingsConfig(setting);
			if (setting.coordinate_range >= 0L)
			{
				this.CoordinatedMixingSettings.Add(setting.id);
			}
		};
		CustomGameSettings.instance = this;
		action(CustomGameSettingConfigs.ClusterLayout);
		action(CustomGameSettingConfigs.WorldgenSeed);
		action(CustomGameSettingConfigs.ImmuneSystem);
		action(CustomGameSettingConfigs.CalorieBurn);
		action(CustomGameSettingConfigs.Morale);
		action(CustomGameSettingConfigs.Durability);
		action(CustomGameSettingConfigs.MeteorShowers);
		action(CustomGameSettingConfigs.Radiation);
		action(CustomGameSettingConfigs.Stress);
		action(CustomGameSettingConfigs.StressBreaks);
		action(CustomGameSettingConfigs.CarePackages);
		action(CustomGameSettingConfigs.SandboxMode);
		action(CustomGameSettingConfigs.FastWorkersMode);
		action(CustomGameSettingConfigs.SaveToCloud);
		action(CustomGameSettingConfigs.Teleporters);
		action3(CustomMixingSettingsConfigs.DLC2Mixing);
		action3(CustomMixingSettingsConfigs.IceCavesMixing);
		action3(CustomMixingSettingsConfigs.CarrotQuarryMixing);
		action3(CustomMixingSettingsConfigs.SugarWoodsMixing);
		action3(CustomMixingSettingsConfigs.CeresAsteroidMixing);
		foreach (Story story in Db.Get().Stories.GetStoriesSortedByCoordinateOrder())
		{
			int num = ((story.kleiUseOnlyCoordinateOrder == -1) ? (-1) : 3);
			SettingConfig settingConfig = new ListSettingConfig(story.Id, "", "", new List<SettingLevel>
			{
				new SettingLevel("Disabled", "", "", 0L, null),
				new SettingLevel("Guaranteed", "", "", 1L, null)
			}, "Disabled", "Disabled", (long)num, false, false, null, "", false);
			action2(settingConfig);
		}
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.MixingSettings)
		{
			DlcMixingSettingConfig dlcMixingSettingConfig = keyValuePair.Value as DlcMixingSettingConfig;
			if (dlcMixingSettingConfig != null && DlcManager.IsContentSubscribed(dlcMixingSettingConfig.id))
			{
				this.SetMixingSetting(dlcMixingSettingConfig, "Enabled");
			}
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

	public SettingLevel CycleQualitySettingLevel(ListSettingConfig config, int direction)
	{
		this.SetQualitySetting(config, config.CycleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id], direction));
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleQualitySettingLevel(ToggleSettingConfig config)
	{
		this.SetQualitySetting(config, config.ToggleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id]));
		return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
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

	public void SetQualitySetting(SettingConfig config, string value)
	{
		this.SetQualitySetting(config, value, true);
	}

	public void SetQualitySetting(SettingConfig config, string value, bool notify)
	{
		this.CurrentQualityLevelsBySetting[config.id] = value;
		this.CheckCustomGameMode();
		if (notify && this.OnQualitySettingChanged != null)
		{
			this.OnQualitySettingChanged(config, this.GetCurrentQualitySetting(config));
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
		string text = (DlcManager.HasAllContentSubscribed(settingConfig.required_content) ? this.CurrentQualityLevelsBySetting[setting_id] : settingConfig.GetDefaultLevelId());
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
		BigInteger bigInteger = this.Base36toBinary(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (object obj in global::Util.Reverse(this.CoordinatedStorySettings))
		{
			string text = (string)obj;
			SettingConfig settingConfig = this.StorySettings[text];
			if (settingConfig.coordinate_range != -1L)
			{
				long num = (long)(bigInteger % settingConfig.coordinate_range);
				bigInteger /= settingConfig.coordinate_range;
				foreach (SettingLevel settingLevel in settingConfig.GetLevels())
				{
					if (settingLevel.coordinate_value == num)
					{
						dictionary[settingConfig] = settingLevel.id;
						break;
					}
				}
			}
		}
		foreach (KeyValuePair<SettingConfig, string> keyValuePair in dictionary)
		{
			this.SetStorySetting(keyValuePair.Key, keyValuePair.Value);
		}
	}

	private string GetStoryTraitSettingsCode()
	{
		BigInteger bigInteger = 0;
		foreach (string text in this.CoordinatedStorySettings)
		{
			SettingConfig settingConfig = this.StorySettings[text];
			bigInteger *= settingConfig.coordinate_range;
			bigInteger += settingConfig.GetLevel(this.currentStoryLevelsBySetting[text]).coordinate_value;
		}
		return this.BinarytoBase36(bigInteger);
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
		string text = (DlcManager.HasAllContentSubscribed(settingConfig.required_content) ? this.currentStoryLevelsBySetting[settingId] : settingConfig.GetDefaultLevelId());
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

	public void SetMixingSetting(SettingConfig config, string value)
	{
		this.SetMixingSetting(config, value, true);
	}

	public void SetMixingSetting(SettingConfig config, string value, bool notify)
	{
		this.CurrentMixingLevelsBySetting[config.id] = value;
		if (notify && this.OnMixingSettingChanged != null)
		{
			this.OnMixingSettingChanged(config, this.GetCurrentMixingSettingLevel(config));
		}
	}

	public void AddMixingSettingsConfig(SettingConfig config)
	{
		this.MixingSettings.Add(config.id, config);
		if (!this.CurrentMixingLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(this.CurrentMixingLevelsBySetting[config.id]))
		{
			this.CurrentMixingLevelsBySetting[config.id] = config.GetDefaultLevelId();
		}
	}

	public SettingLevel GetCurrentMixingSettingLevel(SettingConfig setting)
	{
		return this.GetCurrentMixingSettingLevel(setting.id);
	}

	public SettingConfig GetWorldMixingSettingForWorldgenFile(string file)
	{
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.MixingSettings)
		{
			WorldMixingSettingConfig worldMixingSettingConfig = keyValuePair.Value as WorldMixingSettingConfig;
			if (worldMixingSettingConfig != null && worldMixingSettingConfig.worldgenPath == file)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public SettingConfig GetSubworldMixingSettingForWorldgenFile(string file)
	{
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.MixingSettings)
		{
			SubworldMixingSettingConfig subworldMixingSettingConfig = keyValuePair.Value as SubworldMixingSettingConfig;
			if (subworldMixingSettingConfig != null && subworldMixingSettingConfig.worldgenPath == file)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public void DisableAllMixing()
	{
		foreach (SettingConfig settingConfig in this.MixingSettings.Values)
		{
			this.SetMixingSetting(settingConfig, settingConfig.GetDefaultLevelId());
		}
	}

	public List<SubworldMixingSettingConfig> GetActiveSubworldMixingSettings()
	{
		List<SubworldMixingSettingConfig> list = new List<SubworldMixingSettingConfig>();
		foreach (SettingConfig settingConfig in this.MixingSettings.Values)
		{
			SubworldMixingSettingConfig subworldMixingSettingConfig = settingConfig as SubworldMixingSettingConfig;
			if (subworldMixingSettingConfig != null && this.GetCurrentMixingSettingLevel(settingConfig).id != "Disabled")
			{
				list.Add(subworldMixingSettingConfig);
			}
		}
		return list;
	}

	public List<WorldMixingSettingConfig> GetActiveWorldMixingSettings()
	{
		List<WorldMixingSettingConfig> list = new List<WorldMixingSettingConfig>();
		foreach (SettingConfig settingConfig in this.MixingSettings.Values)
		{
			WorldMixingSettingConfig worldMixingSettingConfig = settingConfig as WorldMixingSettingConfig;
			if (worldMixingSettingConfig != null && this.GetCurrentMixingSettingLevel(settingConfig).id != "Disabled")
			{
				list.Add(worldMixingSettingConfig);
			}
		}
		return list;
	}

	public SettingLevel CycleMixingSettingLevel(ListSettingConfig config, int direction)
	{
		this.SetMixingSetting(config, config.CycleSettingLevelID(this.CurrentMixingLevelsBySetting[config.id], direction));
		return config.GetLevel(this.CurrentMixingLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleMixingSettingLevel(ToggleSettingConfig config)
	{
		this.SetMixingSetting(config, config.ToggleSettingLevelID(this.CurrentMixingLevelsBySetting[config.id]));
		return config.GetLevel(this.CurrentMixingLevelsBySetting[config.id]);
	}

	public SettingLevel GetCurrentMixingSettingLevel(string settingId)
	{
		SettingConfig settingConfig = this.MixingSettings[settingId];
		if (!this.CurrentMixingLevelsBySetting.ContainsKey(settingId))
		{
			this.CurrentMixingLevelsBySetting[settingId] = this.MixingSettings[settingId].GetDefaultLevelId();
		}
		string text = (DlcManager.HasAllContentSubscribed(settingConfig.required_content) ? this.CurrentMixingLevelsBySetting[settingId] : settingConfig.GetDefaultLevelId());
		return this.MixingSettings[settingId].GetLevel(text);
	}

	public List<string> GetCurrentDlcMixingIds()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.MixingSettings)
		{
			DlcMixingSettingConfig dlcMixingSettingConfig = keyValuePair.Value as DlcMixingSettingConfig;
			if (dlcMixingSettingConfig != null && dlcMixingSettingConfig.IsOnLevel(this.GetCurrentMixingSettingLevel(dlcMixingSettingConfig.id).id))
			{
				list.Add(dlcMixingSettingConfig.id);
			}
		}
		return list;
	}

	public void ParseAndApplyMixingSettingsCode(string code)
	{
		BigInteger bigInteger = this.Base36toBinary(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (object obj in global::Util.Reverse(this.CoordinatedMixingSettings))
		{
			string text = (string)obj;
			SettingConfig settingConfig = this.MixingSettings[text];
			if (settingConfig.coordinate_range != -1L)
			{
				long num = (long)(bigInteger % settingConfig.coordinate_range);
				bigInteger /= settingConfig.coordinate_range;
				foreach (SettingLevel settingLevel in settingConfig.GetLevels())
				{
					if (settingLevel.coordinate_value == num)
					{
						dictionary[settingConfig] = settingLevel.id;
						break;
					}
				}
			}
		}
		foreach (KeyValuePair<SettingConfig, string> keyValuePair in dictionary)
		{
			this.SetMixingSetting(keyValuePair.Key, keyValuePair.Value);
		}
	}

	private string GetMixingSettingsCode()
	{
		BigInteger bigInteger = 0;
		foreach (string text in this.CoordinatedMixingSettings)
		{
			SettingConfig settingConfig = this.MixingSettings[text];
			bigInteger *= settingConfig.coordinate_range;
			bigInteger += settingConfig.GetLevel(this.GetCurrentMixingSettingLevel(settingConfig).id).coordinate_value;
		}
		return this.BinarytoBase36(bigInteger);
	}

	public void RemoveInvalidMixingSettings()
	{
		ClusterLayout currentClusterLayout = this.GetCurrentClusterLayout();
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.MixingSettings)
		{
			DlcMixingSettingConfig dlcMixingSettingConfig = keyValuePair.Value as DlcMixingSettingConfig;
			if (dlcMixingSettingConfig != null && currentClusterLayout.requiredDlcIds.Contains(dlcMixingSettingConfig.id))
			{
				this.SetMixingSetting(keyValuePair.Value, "Disabled");
			}
		}
		CustomGameSettings.<>c__DisplayClass71_0 CS$<>8__locals1;
		CS$<>8__locals1.availableDlcs = this.GetCurrentDlcMixingIds();
		CS$<>8__locals1.availableDlcs.AddRange(currentClusterLayout.requiredDlcIds);
		foreach (KeyValuePair<string, SettingConfig> keyValuePair2 in this.MixingSettings)
		{
			SettingConfig value = keyValuePair2.Value;
			WorldMixingSettingConfig worldMixingSettingConfig = value as WorldMixingSettingConfig;
			if (worldMixingSettingConfig == null)
			{
				SubworldMixingSettingConfig subworldMixingSettingConfig = value as SubworldMixingSettingConfig;
				if (subworldMixingSettingConfig != null)
				{
					if (!CustomGameSettings.<RemoveInvalidMixingSettings>g__HasRequiredContent|71_0(subworldMixingSettingConfig.required_content, ref CS$<>8__locals1) || currentClusterLayout.HasAnyTags(subworldMixingSettingConfig.forbiddenClusterTags))
					{
						this.SetMixingSetting(keyValuePair2.Value, "Disabled");
					}
				}
			}
			else if (!CustomGameSettings.<RemoveInvalidMixingSettings>g__HasRequiredContent|71_0(worldMixingSettingConfig.required_content, ref CS$<>8__locals1) || currentClusterLayout.HasAnyTags(worldMixingSettingConfig.forbiddenClusterTags))
			{
				this.SetMixingSetting(keyValuePair2.Value, "Disabled");
			}
		}
	}

	public ClusterLayout GetCurrentClusterLayout()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout);
		if (currentQualitySetting == null)
		{
			return null;
		}
		return SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
	}

	public int GetCurrentWorldgenSeed()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		if (currentQualitySetting == null)
		{
			return 0;
		}
		return int.Parse(currentQualitySetting.id);
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
		text = "Mixing Settings: ";
		foreach (KeyValuePair<string, string> keyValuePair3 in this.CurrentMixingLevelsBySetting)
		{
			text = string.Concat(new string[] { text, keyValuePair3.Key, "=", keyValuePair3.Value, "," });
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

	public List<CustomGameSettings.MetricSettingsData> GetSettingsForMixingMetrics()
	{
		List<CustomGameSettings.MetricSettingsData> list = new List<CustomGameSettings.MetricSettingsData>();
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentMixingLevelsBySetting)
		{
			if (DlcManager.HasAllContentSubscribed(this.MixingSettings[keyValuePair.Key].required_content))
			{
				list.Add(new CustomGameSettings.MetricSettingsData
				{
					Name = keyValuePair.Key,
					Value = keyValuePair.Value
				});
			}
		}
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
		bool flag = false;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in configs)
		{
			if (keyValuePair.Value.coordinate_range >= 0L)
			{
				List<SettingLevel> levels = keyValuePair.Value.GetLevels();
				if (keyValuePair.Value.coordinate_range < (long)levels.Count)
				{
					flag = true;
					global::Debug.Assert(false, string.Concat(new string[]
					{
						keyValuePair.Value.id,
						": Range between coordinate min and max insufficient for all levels (",
						keyValuePair.Value.coordinate_range.ToString(),
						"<",
						levels.Count.ToString(),
						")"
					}));
				}
				foreach (SettingLevel settingLevel in levels)
				{
					Dictionary<long, string> dictionary = new Dictionary<long, string>();
					string text = keyValuePair.Value.id + " > " + settingLevel.id;
					if (keyValuePair.Value.coordinate_range <= settingLevel.coordinate_value)
					{
						flag = true;
						global::Debug.Assert(false, string.Format("%s: Level coordinate value (%u) exceedes range (%u)", text, settingLevel.coordinate_value, keyValuePair.Value.coordinate_range));
					}
					if (settingLevel.coordinate_value < 0L)
					{
						flag = true;
						global::Debug.Assert(false, text + ": Level coordinate value must be >= 0");
					}
					else if (settingLevel.coordinate_value == 0L)
					{
						if (settingLevel.id != keyValuePair.Value.GetDefaultLevelId())
						{
							flag = true;
							global::Debug.Assert(false, text + ": Only the default level should have a coordinate value of 0");
						}
					}
					else
					{
						string text2;
						bool flag2 = !dictionary.TryGetValue(settingLevel.coordinate_value, out text2);
						dictionary[settingLevel.coordinate_value] = text;
						if (settingLevel.id == keyValuePair.Value.GetDefaultLevelId())
						{
							flag = true;
							global::Debug.Assert(false, text + ": Default level must be a coordinate value of 0");
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
		Match match = new Regex("(.*)-(\\d*)-(.*)-(.*)-(.*)").Match(coord);
		for (int i = 1; i <= 2; i++)
		{
			if (match.Groups.Count == 1)
			{
				match = new Regex("(.*)-(\\d*)-(.*)-(.*)-(.*)".Remove("(.*)-(\\d*)-(.*)-(.*)-(.*)".Length - i * 5)).Match(coord);
			}
		}
		string[] array = new string[match.Groups.Count];
		for (int j = 0; j < match.Groups.Count; j++)
		{
			array[j] = match.Groups[j].Value;
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
			return "0-0-0-0-0";
		}
		ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = this.GetOtherSettingsCode();
		string storyTraitSettingsCode = this.GetStoryTraitSettingsCode();
		string mixingSettingsCode = this.GetMixingSettingsCode();
		return string.Format("{0}-{1}-{2}-{3}-{4}", new object[]
		{
			clusterData.GetCoordinatePrefix(),
			currentQualitySetting2.id,
			otherSettingsCode,
			storyTraitSettingsCode,
			mixingSettingsCode
		});
	}

	public void ParseAndApplySettingsCode(string code)
	{
		BigInteger bigInteger = this.Base36toBinary(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (object obj in global::Util.Reverse(this.CoordinatedQualitySettings))
		{
			string text = (string)obj;
			if (this.QualitySettings.ContainsKey(text))
			{
				SettingConfig settingConfig = this.QualitySettings[text];
				if (settingConfig.coordinate_range != -1L)
				{
					long num = (long)(bigInteger % settingConfig.coordinate_range);
					bigInteger /= settingConfig.coordinate_range;
					foreach (SettingLevel settingLevel in settingConfig.GetLevels())
					{
						if (settingLevel.coordinate_value == num)
						{
							dictionary[settingConfig] = settingLevel.id;
							break;
						}
					}
				}
			}
		}
		foreach (KeyValuePair<SettingConfig, string> keyValuePair in dictionary)
		{
			this.SetQualitySetting(keyValuePair.Key, keyValuePair.Value);
		}
	}

	private string GetOtherSettingsCode()
	{
		BigInteger bigInteger = 0;
		foreach (string text in this.CoordinatedQualitySettings)
		{
			SettingConfig settingConfig = this.QualitySettings[text];
			bigInteger *= settingConfig.coordinate_range;
			bigInteger += settingConfig.GetLevel(this.GetCurrentQualitySetting(text).id).coordinate_value;
		}
		return this.BinarytoBase36(bigInteger);
	}

	private BigInteger Base36toBinary(string input)
	{
		if (input == "0")
		{
			return 0;
		}
		BigInteger bigInteger = 0;
		for (int i = input.Length - 1; i >= 0; i--)
		{
			bigInteger *= 36;
			long num = (long)this.hexChars.IndexOf(input[i]);
			bigInteger += num;
		}
		DebugUtil.LogArgs(new object[]
		{
			"tried converting",
			input,
			", got",
			bigInteger,
			"and returns to",
			this.BinarytoBase36(bigInteger)
		});
		return bigInteger;
	}

	private string BinarytoBase36(BigInteger input)
	{
		if (input == 0L)
		{
			return "0";
		}
		BigInteger bigInteger = input;
		string text = "";
		while (bigInteger > 0L)
		{
			text += this.hexChars[(int)(bigInteger % 36)].ToString();
			bigInteger /= 36;
		}
		return text;
	}

	[CompilerGenerated]
	internal static bool <RemoveInvalidMixingSettings>g__HasRequiredContent|71_0(string[] requiredContent, ref CustomGameSettings.<>c__DisplayClass71_0 A_1)
	{
		foreach (string text in requiredContent)
		{
			if (!A_1.availableDlcs.Contains(text))
			{
				return false;
			}
		}
		return true;
	}

	private static CustomGameSettings instance;

	public const long NO_COORDINATE_RANGE = -1L;

	private const int NUM_STORY_LEVELS = 3;

	public const string STORY_DISABLED_LEVEL = "Disabled";

	public const string STORY_GUARANTEED_LEVEL = "Guaranteed";

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameSettings.CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	[Serialize]
	private Dictionary<string, string> CurrentMixingLevelsBySetting = new Dictionary<string, string>();

	private Dictionary<string, string> currentStoryLevelsBySetting = new Dictionary<string, string>();

	public List<string> CoordinatedQualitySettings = new List<string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	public List<string> CoordinatedStorySettings = new List<string>();

	public Dictionary<string, SettingConfig> StorySettings = new Dictionary<string, SettingConfig>();

	public List<string> CoordinatedMixingSettings = new List<string>();

	public Dictionary<string, SettingConfig> MixingSettings = new Dictionary<string, SettingConfig>();

	private const string coordinatePatern = "(.*)-(\\d*)-(.*)-(.*)-(.*)";

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
