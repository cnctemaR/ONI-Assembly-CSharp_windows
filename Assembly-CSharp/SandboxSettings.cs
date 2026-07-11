using System;
using System.Collections.Generic;

public class SandboxSettings
{
	public void AddIntSetting(string prefsKey, Action<int> setAction, int defaultValue)
	{
		this.intSettings.Add(new SandboxSettings.Setting<int>(prefsKey, setAction, defaultValue));
	}

	public int GetIntSetting(string prefsKey)
	{
		return KPlayerPrefs.GetInt(prefsKey);
	}

	public void SetIntSetting(string prefsKey, int value)
	{
		SandboxSettings.Setting<int> setting = this.intSettings.Find((SandboxSettings.Setting<int> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"No intSetting named: ",
				prefsKey,
				" could be found amongst ",
				this.intSettings.Count,
				" int settings."
			}));
		}
		setting.Value = value;
	}

	public void RestoreIntSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			this.SetIntSetting(prefsKey, this.GetIntSetting(prefsKey));
			return;
		}
		this.SetIntSetting(prefsKey, this.intSettings.Find((SandboxSettings.Setting<int> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public void AddFloatSetting(string prefsKey, Action<float> setAction, float defaultValue)
	{
		this.floatSettings.Add(new SandboxSettings.Setting<float>(prefsKey, setAction, defaultValue));
	}

	public float GetFloatSetting(string prefsKey)
	{
		return KPlayerPrefs.GetFloat(prefsKey);
	}

	public void SetFloatSetting(string prefsKey, float value)
	{
		SandboxSettings.Setting<float> setting = this.floatSettings.Find((SandboxSettings.Setting<float> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"No KPlayerPrefs float setting named: ",
				prefsKey,
				" could be found amongst ",
				this.floatSettings.Count,
				" float settings."
			}));
		}
		setting.Value = value;
	}

	public void RestoreFloatSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			this.SetFloatSetting(prefsKey, this.GetFloatSetting(prefsKey));
			return;
		}
		this.SetFloatSetting(prefsKey, this.floatSettings.Find((SandboxSettings.Setting<float> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public void AddStringSetting(string prefsKey, Action<string> setAction, string defaultValue)
	{
		this.stringSettings.Add(new SandboxSettings.Setting<string>(prefsKey, setAction, defaultValue));
	}

	public string GetStringSetting(string prefsKey)
	{
		return KPlayerPrefs.GetString(prefsKey);
	}

	public void SetStringSetting(string prefsKey, string value)
	{
		SandboxSettings.Setting<string> setting = this.stringSettings.Find((SandboxSettings.Setting<string> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"No KPlayerPrefs string setting named: ",
				prefsKey,
				" could be found amongst ",
				this.stringSettings.Count,
				" settings."
			}));
		}
		setting.Value = value;
	}

	public void RestoreStringSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			this.SetStringSetting(prefsKey, this.GetStringSetting(prefsKey));
			return;
		}
		this.SetStringSetting(prefsKey, this.stringSettings.Find((SandboxSettings.Setting<string> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public SandboxSettings()
	{
		this.AddStringSetting("SandboxTools.SelectedEntity", delegate(string data)
		{
			KPlayerPrefs.SetString("SandboxTools.SelectedEntity", data);
			this.OnChangeEntity();
		}, "MushBar");
		this.AddIntSetting("SandboxTools.SelectedElement", delegate(int data)
		{
			KPlayerPrefs.SetInt("SandboxTools.SelectedElement", data);
			this.OnChangeElement(this.hasRestoredElement);
			this.hasRestoredElement = true;
		}, ElementLoader.GetElementIndex(SimHashes.Oxygen));
		this.AddStringSetting("SandboxTools.SelectedDisease", delegate(string data)
		{
			KPlayerPrefs.SetString("SandboxTools.SelectedDisease", data);
			this.OnChangeDisease();
		}, Db.Get().Diseases.FoodGerms.Id);
		this.AddIntSetting("SandboxTools.DiseaseCount", delegate(int val)
		{
			KPlayerPrefs.SetInt("SandboxTools.DiseaseCount", val);
			this.OnChangeDiseaseCount();
		}, 0);
		this.AddIntSetting("SandboxTools.BrushSize", delegate(int val)
		{
			KPlayerPrefs.SetInt("SandboxTools.BrushSize", val);
			this.OnChangeBrushSize();
		}, 1);
		this.AddFloatSetting("SandboxTools.NoiseScale", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.NoiseScale", val);
			this.OnChangeNoiseScale();
		}, 1f);
		this.AddFloatSetting("SandboxTools.NoiseDensity", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.NoiseDensity", val);
			this.OnChangeNoiseDensity();
		}, 1f);
		this.AddFloatSetting("SandboxTools.Mass", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.Mass", val);
			this.OnChangeMass();
		}, 1f);
		this.AddFloatSetting("SandbosTools.Temperature", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandbosTools.Temperature", val);
			this.OnChangeTemperature();
		}, 300f);
		this.AddFloatSetting("SandbosTools.TemperatureAdditive", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandbosTools.TemperatureAdditive", val);
			this.OnChangeAdditiveTemperature();
		}, 5f);
	}

	public void RestorePrefs()
	{
		foreach (SandboxSettings.Setting<int> setting in this.intSettings)
		{
			this.RestoreIntSetting(setting.PrefsKey);
		}
		foreach (SandboxSettings.Setting<float> setting2 in this.floatSettings)
		{
			this.RestoreFloatSetting(setting2.PrefsKey);
		}
		foreach (SandboxSettings.Setting<string> setting3 in this.stringSettings)
		{
			this.RestoreStringSetting(setting3.PrefsKey);
		}
	}

	private List<SandboxSettings.Setting<int>> intSettings = new List<SandboxSettings.Setting<int>>();

	private List<SandboxSettings.Setting<float>> floatSettings = new List<SandboxSettings.Setting<float>>();

	private List<SandboxSettings.Setting<string>> stringSettings = new List<SandboxSettings.Setting<string>>();

	public bool InstantBuild = true;

	private bool hasRestoredElement;

	public Action<bool> OnChangeElement;

	public global::System.Action OnChangeMass;

	public global::System.Action OnChangeDisease;

	public global::System.Action OnChangeDiseaseCount;

	public global::System.Action OnChangeEntity;

	public global::System.Action OnChangeBrushSize;

	public global::System.Action OnChangeNoiseScale;

	public global::System.Action OnChangeNoiseDensity;

	public global::System.Action OnChangeTemperature;

	public global::System.Action OnChangeAdditiveTemperature;

	public const string KEY_SELECTED_ENTITY = "SandboxTools.SelectedEntity";

	public const string KEY_SELECTED_ELEMENT = "SandboxTools.SelectedElement";

	public const string KEY_SELECTED_DISEASE = "SandboxTools.SelectedDisease";

	public const string KEY_DISEASE_COUNT = "SandboxTools.DiseaseCount";

	public const string KEY_BRUSH_SIZE = "SandboxTools.BrushSize";

	public const string KEY_NOISE_SCALE = "SandboxTools.NoiseScale";

	public const string KEY_NOISE_DENSITY = "SandboxTools.NoiseDensity";

	public const string KEY_MASS = "SandboxTools.Mass";

	public const string KEY_TEMPERATURE = "SandbosTools.Temperature";

	public const string KEY_TEMPERATURE_ADDITIVE = "SandbosTools.TemperatureAdditive";

	public class Setting<T>
	{
		public Setting(string prefsKey, Action<T> setAction, T defaultValue)
		{
			this.prefsKey = prefsKey;
			this.SetAction = setAction;
			this.defaultValue = defaultValue;
		}

		public string PrefsKey
		{
			get
			{
				return this.prefsKey;
			}
		}

		public T Value
		{
			set
			{
				this.SetAction(value);
			}
		}

		private string prefsKey;

		private Action<T> SetAction;

		public T defaultValue;
	}
}
