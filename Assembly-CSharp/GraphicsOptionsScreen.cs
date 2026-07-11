using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class GraphicsOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.TITLE);
		this.originalSettings = this.CaptureSettings();
		this.applyButton.isInteractable = false;
		this.applyButton.onClick += this.OnApply;
		this.applyButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.APPLYBUTTON);
		this.doneButton.onClick += this.OnDone;
		this.closeButton.onClick += this.OnDone;
		this.doneButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.DONE_BUTTON);
		bool flag = QualitySettings.GetQualityLevel() == 1;
		this.lowResToggle.ChangeState((!flag) ? 0 : 1);
		MultiToggle multiToggle = this.lowResToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnLowResToggle));
		this.lowResToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.LOWRES);
		this.resolutionDropdown.ClearOptions();
		this.BuildOptions();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(delegate
		{
			this.BuildOptions();
			this.resolutionDropdown.options = this.options;
		}));
		this.resolutionDropdown.options = this.options;
		this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionChanged));
		this.fullscreenToggle.ChangeState((!Screen.fullScreen) ? 0 : 1);
		MultiToggle multiToggle2 = this.fullscreenToggle;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(this.OnFullscreenToggle));
		this.fullscreenToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.FULLSCREEN);
		this.resolutionDropdown.transform.parent.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.RESOLUTION);
		if (this.fullscreenToggle.CurrentState == 1)
		{
			int resolutionIndex = this.GetResolutionIndex(this.originalSettings.resolution);
			if (resolutionIndex != -1)
			{
				this.resolutionDropdown.value = resolutionIndex;
			}
		}
		this.CanvasScalers = global::UnityEngine.Object.FindObjectsOfType<KCanvasScaler>();
		this.UpdateSliderLabel();
		this.uiScaleSlider.onValueChanged.AddListener(delegate(float data)
		{
			this.sliderLabel.text = this.uiScaleSlider.value + "%";
		});
		this.uiScaleSlider.onReleaseHandle += delegate
		{
			this.UpdateUIScale(this.uiScaleSlider.value);
		};
	}

	public static void SetSettingsFromPrefs()
	{
		GraphicsOptionsScreen.SetResolutionFromPrefs();
		GraphicsOptionsScreen.SetLowResFromPrefs();
	}

	public static void SetLowResFromPrefs()
	{
		int num = 0;
		if (KPlayerPrefs.HasKey(GraphicsOptionsScreen.LowResKey))
		{
			if (QualitySettings.GetQualityLevel() != num)
			{
				num = KPlayerPrefs.GetInt(GraphicsOptionsScreen.LowResKey);
				QualitySettings.SetQualityLevel(num, true);
			}
		}
		else
		{
			QualitySettings.SetQualityLevel(num, true);
		}
		DebugUtil.LogArgs(new object[] { string.Format("Low Res Textures? {0}", (num != 1) ? "No" : "Yes") });
	}

	public static void SetResolutionFromPrefs()
	{
		int num = Screen.currentResolution.width;
		int num2 = Screen.currentResolution.height;
		int num3 = Screen.currentResolution.refreshRate;
		bool flag = Screen.fullScreen;
		if (KPlayerPrefs.HasKey(GraphicsOptionsScreen.ResolutionWidthKey) && KPlayerPrefs.HasKey(GraphicsOptionsScreen.ResolutionHeightKey))
		{
			int @int = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ResolutionWidthKey);
			int int2 = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ResolutionHeightKey);
			int int3 = KPlayerPrefs.GetInt(GraphicsOptionsScreen.RefreshRateKey, Screen.currentResolution.refreshRate);
			bool flag2 = KPlayerPrefs.GetInt(GraphicsOptionsScreen.FullScreenKey, (!Screen.fullScreen) ? 0 : 1) == 1;
			if (int2 <= 1 || @int <= 1)
			{
				DebugUtil.LogArgs(new object[] { "Saved resolution was invalid, ignoring..." });
			}
			else
			{
				num = @int;
				num2 = int2;
				num3 = int3;
				flag = flag2;
			}
		}
		if (num <= 1 || num2 <= 1)
		{
			DebugUtil.LogWarningArgs(new object[] { "Detected a degenerate resolution, attempting to fix..." });
			foreach (Resolution resolution in Screen.resolutions)
			{
				if (resolution.width == 1920)
				{
					num = resolution.width;
					num2 = resolution.height;
					num3 = 0;
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				foreach (Resolution resolution2 in Screen.resolutions)
				{
					if (resolution2.width == 1280)
					{
						num = resolution2.width;
						num2 = resolution2.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				foreach (Resolution resolution3 in Screen.resolutions)
				{
					if (resolution3.width > 1 && resolution3.height > 1 && resolution3.refreshRate > 0)
					{
						num = resolution3.width;
						num2 = resolution3.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				string text = "Could not find a suitable resolution for this screen! Reported available resolutions are:";
				foreach (Resolution resolution4 in Screen.resolutions)
				{
					text += string.Format("\n{0}x{1} @ {2}hz", resolution4.width, resolution4.height, resolution4.refreshRate);
				}
				global::Debug.LogError(text);
				num = 1280;
				num2 = 720;
				flag = false;
				num3 = 0;
			}
		}
		DebugUtil.LogArgs(new object[] { string.Format("Applying resolution {0}x{1} @{2}hz (fullscreen: {3})", new object[] { num, num2, num3, flag }) });
		Screen.SetResolution(num, num2, flag, num3);
	}

	public static void OnResize()
	{
		GraphicsOptionsScreen.Settings settings = default(GraphicsOptionsScreen.Settings);
		settings.resolution = Screen.currentResolution;
		settings.resolution.width = Screen.width;
		settings.resolution.height = Screen.height;
		settings.fullscreen = Screen.fullScreen;
		settings.lowRes = QualitySettings.GetQualityLevel();
		GraphicsOptionsScreen.SaveSettingsToPrefs(settings);
	}

	private static void SaveSettingsToPrefs(GraphicsOptionsScreen.Settings settings)
	{
		KPlayerPrefs.SetInt(GraphicsOptionsScreen.LowResKey, settings.lowRes);
		global::Debug.LogFormat("Screen resolution updated, saving values to prefs: {0}x{1} @ {2}, fullscreen: {3}", new object[]
		{
			settings.resolution.width,
			settings.resolution.height,
			settings.resolution.refreshRate,
			settings.fullscreen
		});
		KPlayerPrefs.SetInt(GraphicsOptionsScreen.ResolutionWidthKey, settings.resolution.width);
		KPlayerPrefs.SetInt(GraphicsOptionsScreen.ResolutionHeightKey, settings.resolution.height);
		KPlayerPrefs.SetInt(GraphicsOptionsScreen.RefreshRateKey, settings.resolution.refreshRate);
		KPlayerPrefs.SetInt(GraphicsOptionsScreen.FullScreenKey, (!settings.fullscreen) ? 0 : 1);
	}

	private void UpdateUIScale(float value)
	{
		foreach (KCanvasScaler kcanvasScaler in this.CanvasScalers)
		{
			float num = value / 100f;
			kcanvasScaler.SetUserScale(num);
			KPlayerPrefs.SetFloat(KCanvasScaler.UIScalePrefKey, value);
		}
		this.UpdateSliderLabel();
	}

	private void UpdateSliderLabel()
	{
		if (this.CanvasScalers != null && this.CanvasScalers.Length > 0 && this.CanvasScalers[0] != null)
		{
			this.uiScaleSlider.value = this.CanvasScalers[0].GetUserScale() * 100f;
			this.sliderLabel.text = this.uiScaleSlider.value + "%";
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.resolutionDropdown.Hide();
			this.Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void BuildOptions()
	{
		this.options.Clear();
		this.resolutions.Clear();
		Resolution resolution = default(Resolution);
		resolution.width = Screen.width;
		resolution.height = Screen.height;
		resolution.refreshRate = Screen.currentResolution.refreshRate;
		this.options.Add(new Dropdown.OptionData(resolution.ToString()));
		this.resolutions.Add(resolution);
		foreach (Resolution resolution2 in Screen.resolutions)
		{
			if (resolution2.height >= 720)
			{
				this.options.Add(new Dropdown.OptionData(resolution2.ToString()));
				this.resolutions.Add(resolution2);
			}
		}
	}

	private int GetResolutionIndex(Resolution resolution)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this.resolutions.Count; i++)
		{
			Resolution resolution2 = this.resolutions[i];
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && resolution2.refreshRate == 0)
			{
				num2 = i;
			}
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && Math.Abs(resolution2.refreshRate - resolution.refreshRate) <= 1)
			{
				num = i;
				break;
			}
		}
		return (num != -1) ? num : num2;
	}

	private GraphicsOptionsScreen.Settings CaptureSettings()
	{
		return new GraphicsOptionsScreen.Settings
		{
			fullscreen = Screen.fullScreen,
			resolution = new Resolution
			{
				width = Screen.width,
				height = Screen.height,
				refreshRate = Screen.currentResolution.refreshRate
			},
			lowRes = QualitySettings.GetQualityLevel()
		};
	}

	private void OnApply()
	{
		try
		{
			GraphicsOptionsScreen.Settings new_settings = default(GraphicsOptionsScreen.Settings);
			new_settings.resolution = this.resolutions[this.resolutionDropdown.value];
			new_settings.fullscreen = this.fullscreenToggle.CurrentState != 0;
			new_settings.lowRes = this.lowResToggle.CurrentState;
			this.ApplyConfirmSettings(new_settings, delegate
			{
				this.applyButton.isInteractable = false;
				GraphicsOptionsScreen.SaveSettingsToPrefs(new_settings);
			});
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Failed to apply graphics options!\nResolutions:");
			foreach (Resolution resolution in this.resolutions)
			{
				stringBuilder.Append("\t" + resolution.ToString() + "\n");
			}
			stringBuilder.Append("Selected Resolution Idx: " + this.resolutionDropdown.value.ToString());
			stringBuilder.Append("FullScreen: " + this.fullscreenToggle.CurrentState.ToString());
			global::Debug.LogError(stringBuilder.ToString());
			throw ex;
		}
	}

	public void OnDone()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshApplyButton()
	{
		GraphicsOptionsScreen.Settings settings = this.CaptureSettings();
		if (settings.fullscreen && this.fullscreenToggle.CurrentState == 0)
		{
			this.applyButton.isInteractable = true;
		}
		else if (!settings.fullscreen && this.fullscreenToggle.CurrentState == 1)
		{
			this.applyButton.isInteractable = true;
		}
		else if (settings.lowRes != this.lowResToggle.CurrentState)
		{
			this.applyButton.isInteractable = true;
		}
		else
		{
			int resolutionIndex = this.GetResolutionIndex(settings.resolution);
			this.applyButton.isInteractable = this.resolutionDropdown.value != resolutionIndex;
		}
	}

	private void OnFullscreenToggle()
	{
		this.fullscreenToggle.ChangeState((this.fullscreenToggle.CurrentState != 0) ? 0 : 1);
		this.RefreshApplyButton();
	}

	private void OnResolutionChanged(int idx)
	{
		this.RefreshApplyButton();
	}

	private void OnLowResToggle()
	{
		this.lowResToggle.ChangeState((this.lowResToggle.CurrentState != 0) ? 0 : 1);
		this.RefreshApplyButton();
	}

	private void ApplyConfirmSettings(GraphicsOptionsScreen.Settings new_settings, global::System.Action on_confirm)
	{
		GraphicsOptionsScreen.Settings current_settings = this.CaptureSettings();
		this.ApplySettings(new_settings);
		this.confirmDialog = Util.KInstantiateUI(this.confirmPrefab.gameObject, base.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
		global::System.Action action = delegate
		{
			this.ApplySettings(current_settings);
		};
		Coroutine timer = base.StartCoroutine(this.Timer(15f, action));
		this.confirmDialog.onDeactivateCB = delegate
		{
			this.StopCoroutine(timer);
		};
		this.confirmDialog.PopupConfirmDialog(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES.text, on_confirm, action, null, null, null, null, null, null, true);
		this.confirmDialog.gameObject.SetActive(true);
	}

	private void ApplySettings(GraphicsOptionsScreen.Settings new_settings)
	{
		Resolution resolution = new_settings.resolution;
		Screen.SetResolution(resolution.width, resolution.height, new_settings.fullscreen, resolution.refreshRate);
		Screen.fullScreen = new_settings.fullscreen;
		int resolutionIndex = this.GetResolutionIndex(new_settings.resolution);
		if (resolutionIndex != -1)
		{
			this.resolutionDropdown.value = resolutionIndex;
		}
		global::Debug.Log(string.Concat(new object[]
		{
			"Applying low res settings ",
			new_settings.lowRes,
			" / existing is ",
			QualitySettings.GetQualityLevel()
		}));
		if (QualitySettings.GetQualityLevel() != new_settings.lowRes)
		{
			QualitySettings.SetQualityLevel(new_settings.lowRes, true);
		}
	}

	private IEnumerator Timer(float time, global::System.Action revert)
	{
		yield return new WaitForSeconds(time);
		if (this.confirmDialog != null)
		{
			this.confirmDialog.Deactivate();
			revert();
		}
		yield break;
	}

	private void Update()
	{
		global::Debug.developerConsoleVisible = false;
	}

	[SerializeField]
	private Dropdown resolutionDropdown;

	[SerializeField]
	private MultiToggle lowResToggle;

	[SerializeField]
	private MultiToggle fullscreenToggle;

	[SerializeField]
	private KButton applyButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	[SerializeField]
	private KSlider uiScaleSlider;

	[SerializeField]
	private LocText sliderLabel;

	[SerializeField]
	private LocText title;

	public static readonly string ResolutionWidthKey = "ResolutionWidth";

	public static readonly string ResolutionHeightKey = "ResolutionHeight";

	public static readonly string RefreshRateKey = "RefreshRate";

	public static readonly string FullScreenKey = "FullScreen";

	public static readonly string LowResKey = "LowResTextures";

	private KCanvasScaler[] CanvasScalers;

	private ConfirmDialogScreen confirmDialog;

	private List<Resolution> resolutions = new List<Resolution>();

	private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

	private GraphicsOptionsScreen.Settings originalSettings;

	private struct Settings
	{
		public bool fullscreen;

		public Resolution resolution;

		public int lowRes;
	}
}
