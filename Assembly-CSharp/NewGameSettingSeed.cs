using System;
using Klei.CustomSettings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewGameSettingSeed : NewGameSettingWidget
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Input.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
		this.Input.onValueChanged.AddListener(new UnityAction<string>(this.OnValueChanged));
		this.RandomizeButton.onClick += this.GetNewRandomSeed;
	}

	public void Initialize(SeedSettingConfig config)
	{
		this.config = config;
		this.Label.text = config.label;
		this.ToolTip.toolTip = config.tooltip;
		this.GetNewRandomSeed();
	}

	public override void Refresh()
	{
		string currentQualitySettingLevelId = CustomGameSettings.Instance.GetCurrentQualitySettingLevelId(this.config);
		this.Input.text = currentQualitySettingLevelId;
		DebugUtil.LogArgs(new object[]
		{
			"Set worldgen seed to",
			int.Parse(currentQualitySettingLevelId)
		});
	}

	private char ValidateInput(string text, int charIndex, char addedChar)
	{
		return ('0' > addedChar || addedChar > '9') ? '\0' : addedChar;
	}

	private void OnEndEdit(string text)
	{
		int num;
		try
		{
			num = Convert.ToInt32(text);
		}
		catch
		{
			num = 0;
		}
		this.SetSeed(num);
	}

	public void SetSeed(int seed)
	{
		seed = Mathf.Min(seed, int.MaxValue);
		CustomGameSettings.Instance.SetQualitySetting(this.config, seed.ToString());
		this.Refresh();
	}

	private void OnValueChanged(string text)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(text);
		}
		catch
		{
			if (text.Length > 0)
			{
				this.Input.text = text.Substring(0, text.Length - 1);
			}
			else
			{
				this.Input.text = string.Empty;
			}
		}
		if (num > 2147483647)
		{
			this.Input.text = text.Substring(0, text.Length - 1);
		}
	}

	private void GetNewRandomSeed()
	{
		int num = global::UnityEngine.Random.Range(0, int.MaxValue);
		this.SetSeed(num);
	}

	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private TMP_InputField Input;

	[SerializeField]
	private KButton RandomizeButton;

	[SerializeField]
	private Image BG;

	private const int MAX_VALID_SEED = 2147483647;

	private SeedSettingConfig config;
}
