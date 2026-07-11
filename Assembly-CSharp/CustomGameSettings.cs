using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;

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

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SettingConfig, SettingLevel> OnSettingChanged;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			this.customGameMode = ((!this.is_custom_game) ? CustomGameSettings.CustomGameMode.Survival : CustomGameSettings.CustomGameMode.Custom);
		}
		if (this.CurrentQualityLevelsBySetting.ContainsKey("CarePackages "))
		{
			if (!this.CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
			{
				this.CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.CarePackages.id, this.CurrentQualityLevelsBySetting["CarePackages "]);
			}
			this.CurrentQualityLevelsBySetting.Remove("CarePackages ");
		}
	}

	protected override void OnPrefabInit()
	{
		CustomGameSettings.instance = this;
		this.AddSettingConfig(CustomGameSettingConfigs.World);
		this.AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		this.AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		this.AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		this.AddSettingConfig(CustomGameSettingConfigs.Morale);
		this.AddSettingConfig(CustomGameSettingConfigs.Stress);
		this.AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		this.AddSettingConfig(CustomGameSettingConfigs.CarePackages);
		this.AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		this.VerifySettingCoordinates();
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
		bool flag = true;
		bool flag2 = true;
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			if (this.QualitySettings[keyValuePair.Key].triggers_custom_game)
			{
				if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].default_level_id)
				{
					flag = false;
				}
				if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].nosweat_default_level_id)
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
		if (this.OnSettingChanged != null)
		{
			this.OnSettingChanged(config, this.GetCurrentQualitySetting(config));
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
			return settingConfig.GetLevel(settingConfig.default_level_id);
		}
		if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.nosweat_default_level_id);
		}
		if (!this.CurrentQualityLevelsBySetting.ContainsKey(setting_id))
		{
			this.CurrentQualityLevelsBySetting[setting_id] = this.QualitySettings[setting_id].default_level_id;
		}
		string text = this.CurrentQualityLevelsBySetting[setting_id];
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
		global::Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id);
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

	private static void AddWorldMods(object user_data, List<SettingLevel> levels)
	{
		string text = FileSystem.Normalize(global::System.IO.Path.Combine(SettingsCache.GetPath(), "worlds"));
		ListPool<string, CustomGameSettings>.PooledList pooledList = ListPool<string, CustomGameSettings>.Allocate();
		FileSystem.GetFiles(text, "*.yaml", pooledList);
		foreach (string text2 in pooledList)
		{
			global::ProcGen.World world = YamlIO.LoadFile<global::ProcGen.World>(text2, null, null);
			string worldName = Worlds.GetWorldName(text2);
			string text3 = worldName;
			string name = world.name;
			string description = world.description;
			levels.Add(new SettingLevel(text3, name, description, 0, user_data));
		}
		pooledList.Recycle();
	}

	public void LoadWorlds()
	{
		Dictionary<string, global::ProcGen.World> worldCache = SettingsCache.worlds.worldCache;
		List<SettingLevel> list = new List<SettingLevel>(worldCache.Count);
		foreach (KeyValuePair<string, global::ProcGen.World> keyValuePair in worldCache)
		{
			StringEntry stringEntry;
			string text = ((!Strings.TryGet(new StringKey(keyValuePair.Value.name), out stringEntry)) ? keyValuePair.Value.name : stringEntry.ToString());
			string text2 = ((!Strings.TryGet(new StringKey(keyValuePair.Value.description), out stringEntry)) ? keyValuePair.Value.description : stringEntry.ToString());
			list.Add(new SettingLevel(keyValuePair.Key, text, text2, 0, null));
		}
		CustomGameSettingConfigs.World.StompLevels(list, "worlds/SandstoneDefault", "worlds/SandstoneDefault");
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
		{
			string text2 = text;
			text = string.Concat(new string[] { text2, keyValuePair.Key, "=", keyValuePair.Value, "," });
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
					global::Debug.Assert(false, string.Concat(new object[]
					{
						keyValuePair.Value.id,
						": Range between coordinate min and max insufficient for all levels (",
						keyValuePair.Value.coordinate_dimension_width,
						"<",
						levels.Count,
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
						if (settingLevel.id != keyValuePair.Value.default_level_id)
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
						if (settingLevel.id == keyValuePair.Value.default_level_id)
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

	public string[] ParseSettingCoordinate(string coord)
	{
		Regex regex = new Regex("(.*)-(.*)-(.*)");
		Match match = regex.Match(coord);
		string[] array = new string[match.Groups.Count];
		for (int i = 0; i < match.Groups.Count; i++)
		{
			array[i] = match.Groups[i].Value;
		}
		return array;
	}

	public string GetSettingsCoordinate()
	{
		global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id);
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = this.GetOtherSettingsCode();
		return string.Format("{0}-{1}-{2}", worldData.GetCoordinatePrefix(), currentQualitySetting.id, otherSettingsCode);
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
		string text = string.Empty;
		while (i > 0)
		{
			text += this.hexChars[i % 36];
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
