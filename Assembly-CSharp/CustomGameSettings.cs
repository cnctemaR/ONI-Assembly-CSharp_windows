using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CustomGameSettings : MonoBehaviour
{
	public CustomGameSettings.SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		string empty = string.Empty;
		this.CurrentQualityLevelsBySetting.TryGetValue(setting_id, out empty);
		return this.QualitySettings[setting_id].GetLevel(empty);
	}

	public string GetSettingLevelLabel(string setting_id, string level_id)
	{
		foreach (CustomGameSettings.SettingLevel settingLevel in this.QualitySettings[setting_id].levels)
		{
			if (settingLevel.id == level_id)
			{
				return settingLevel.label;
			}
		}
		global::Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	public string GetSettingLevelTooltip(string setting_id, string level_id)
	{
		foreach (CustomGameSettings.SettingLevel settingLevel in this.QualitySettings[setting_id].levels)
		{
			if (settingLevel.id == level_id)
			{
				return settingLevel.tooltip;
			}
		}
		global::Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	private void Awake()
	{
		if (CustomGameSettings.instance != null)
		{
			global::UnityEngine.Object.DestroyImmediate(CustomGameSettings.instance.gameObject);
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		CustomGameSettings.instance = this;
		this.QualitySettings.Add(this.ImmuneSystem.id, this.ImmuneSystem);
		this.QualitySettings.Add(this.Stress.id, this.Stress);
		this.Reset();
	}

	public void Reset()
	{
		this.is_custom_game = false;
		this.CurrentQualityLevelsBySetting.Clear();
		foreach (KeyValuePair<string, CustomGameSettings.SettingConfig> keyValuePair in this.QualitySettings)
		{
			this.CurrentQualityLevelsBySetting.Add(keyValuePair.Value.id, keyValuePair.Value.default_level_id);
		}
	}

	public void CycleSettingLevel(string setting_id, int direction)
	{
		string text = string.Empty;
		for (int i = 0; i < this.QualitySettings[setting_id].levels.Length; i++)
		{
			if (this.QualitySettings[setting_id].levels[i].id == this.CurrentQualityLevelsBySetting[setting_id])
			{
				int num = i + direction;
				if (num < 0)
				{
					num += this.QualitySettings[setting_id].levels.Length;
				}
				if (num >= this.QualitySettings[setting_id].levels.Length)
				{
					num -= this.QualitySettings[setting_id].levels.Length;
				}
				text = this.QualitySettings[setting_id].levels[num].id;
				break;
			}
		}
		this.CurrentQualityLevelsBySetting[setting_id] = text;
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

	public Dictionary<string, CustomGameSettings.SettingConfig> QualitySettings = new Dictionary<string, CustomGameSettings.SettingConfig>();

	private CustomGameSettings.SettingConfig ImmuneSystem = new CustomGameSettings.SettingConfig("ImmuneSystem", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.TOOLTIP, new CustomGameSettings.SettingLevel[]
	{
		new CustomGameSettings.SettingLevel("Weak", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.TOOLTIP, 0f),
		new CustomGameSettings.SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.TOOLTIP, 1f),
		new CustomGameSettings.SettingLevel("Strong", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.TOOLTIP, 2f)
	}, "Default");

	private CustomGameSettings.SettingConfig Stress = new CustomGameSettings.SettingConfig("Stress", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.TOOLTIP, new CustomGameSettings.SettingLevel[]
	{
		new CustomGameSettings.SettingLevel("Pessimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.TOOLTIP, 0f),
		new CustomGameSettings.SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.TOOLTIP, 1f),
		new CustomGameSettings.SettingLevel("Optimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.TOOLTIP, 2f)
	}, "Default");

	public class SettingLevel
	{
		public SettingLevel(string id, string label, string tooltip, float value)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.value = value;
		}

		public string id { get; private set; }

		public string tooltip { get; private set; }

		public string label { get; private set; }

		public float value { get; private set; }
	}

	public class SettingConfig
	{
		public SettingConfig(string id, string label, string tooltip, CustomGameSettings.SettingLevel[] levels, string default_level_id)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.levels = levels;
			this.default_level_id = default_level_id;
		}

		public string id { get; private set; }

		public string label { get; private set; }

		public string tooltip { get; private set; }

		public CustomGameSettings.SettingLevel[] levels { get; private set; }

		public string default_level_id { get; private set; }

		public CustomGameSettings.SettingLevel GetLevel(string level_id)
		{
			for (int i = 0; i < this.levels.Length; i++)
			{
				if (this.levels[i].id == level_id)
				{
					return this.levels[i];
				}
			}
			for (int j = 0; j < this.levels.Length; j++)
			{
				if (this.levels[j].id == this.default_level_id)
				{
					global::Debug.LogWarning(string.Concat(new string[] { "Unable to find level for setting:", this.id, "(", level_id, ") Using default level." }), null);
					return this.levels[j];
				}
			}
			global::Debug.LogError("Unable to find setting level for setting:" + this.id + " level: " + level_id, null);
			return null;
		}
	}
}
