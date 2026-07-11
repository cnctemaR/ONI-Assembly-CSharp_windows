using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using Steamworks;

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

	private static void AddModLevels(IFileSystem fs, object user_data, List<SettingLevel> levels)
	{
		string text = FSUtil.Normalize(global::System.IO.Path.Combine(SettingsCache.GetPath(), "worlds"));
		ListPool<string, CustomGameSettings>.PooledList pooledList = ListPool<string, CustomGameSettings>.Allocate();
		FSUtil.GetFiles(fs, text, "*.yaml", pooledList);
		foreach (string text2 in pooledList)
		{
			global::ProcGen.World world = YamlIO<global::ProcGen.World>.LoadFile(text2, null);
			string worldName = Worlds.GetWorldName(text2);
			levels.Add(new SettingLevel(worldName, world.name, world.description, user_data));
		}
		pooledList.Recycle();
	}

	public void LoadWorlds()
	{
		Dictionary<string, Worlds.Data> worldCache = SettingsCache.worlds.worldCache;
		List<SettingLevel> list = new List<SettingLevel>(worldCache.Count);
		foreach (KeyValuePair<string, Worlds.Data> keyValuePair in worldCache)
		{
			list.Add(new SettingLevel(keyValuePair.Key, keyValuePair.Value.world.name, keyValuePair.Value.world.description, null));
		}
		if (DistributionPlatform.Initialized)
		{
			List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("worldgen");
			foreach (SteamUGCService.Subscribed subscribed2 in subscribed)
			{
				ulong num;
				string text;
				uint num2;
				SteamUGC.GetItemInstallInfo(subscribed2.fileId, out num, out text, 1024U, out num2);
				string path = SettingsCache.GetPath();
				string text2 = subscribed2.fileId.m_PublishedFileId.ToString();
				ModInfo modInfo = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.WorldGen, text2, subscribed2.description, path, 0UL);
				FileStream fileStream = File.OpenRead(text);
				ZipFileSystem zipFileSystem = new ZipFileSystem(text2, fileStream, path);
				Global.Instance.layeredFileSystem.AddFileSystem(zipFileSystem);
				CustomGameSettings.AddModLevels(zipFileSystem, modInfo, list);
				Global.Instance.layeredFileSystem.RemoveFileSystem(zipFileSystem);
			}
		}
		CustomGameSettingConfigs.World.StompLevels(list, "worlds/Default", "worlds/Default");
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

	private bool AllValuesMatch(Dictionary<string, string> data, CustomGameSettings.CustomGameMode mode)
	{
		bool flag = true;
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.QualitySettings)
		{
			if (!(keyValuePair.Key == CustomGameSettingConfigs.WorldgenSeed.id))
			{
				string text = null;
				if (mode != CustomGameSettings.CustomGameMode.Nosweat)
				{
					if (mode == CustomGameSettings.CustomGameMode.Survival)
					{
						text = keyValuePair.Value.default_level_id;
					}
				}
				else
				{
					text = keyValuePair.Value.nosweat_default_level_id;
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
		IEnumerator enumerator2 = Enum.GetValues(typeof(CustomGameSettings.CustomGameMode)).GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object obj = enumerator2.Current;
				CustomGameSettings.CustomGameMode customGameMode = (CustomGameSettings.CustomGameMode)obj;
				if (customGameMode != CustomGameSettings.CustomGameMode.Custom)
				{
					if (this.AllValuesMatch(this.CurrentQualityLevelsBySetting, customGameMode))
					{
						metricSettingsData.Value = customGameMode.ToString();
						break;
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator2 as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		list.Add(metricSettingsData);
		return list;
	}

	public const string TAG_WORLDGEN = "worldgen";

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
