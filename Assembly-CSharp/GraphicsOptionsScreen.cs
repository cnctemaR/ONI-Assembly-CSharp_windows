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
		if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
		{
			this.resDropdownAlwaysActive = true;
		}
		this.title.SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.TITLE);
		this.originalSettings = this.CaptureSettings();
		this.applyButton.isInteractable = false;
		this.applyButton.onClick += this.OnApply;
		this.applyButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.APPLYBUTTON);
		this.revertButton.isInteractable = false;
		this.revertButton.onClick += this.OnRevert;
		this.revertButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.REVERTBUTTON);
		this.doneButton.onClick += this.OnDone;
		this.closeButton.onClick += this.OnDone;
		this.doneButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.DONE_BUTTON);
		this.resolutionDropdown.ClearOptions();
		this.BuildOptions();
		this.resolutionDropdown.options = this.options;
		this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionChanged));
		this.fullscreenToggle.isOn = Screen.fullScreen;
		this.fullscreenToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnFullscreenToggle));
		this.fullscreenToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.FULLSCREEN);
		this.resolutionDropdown.interactable = this.resDropdownAlwaysActive || this.fullscreenToggle.isOn;
		this.resolutionDropdown.transform.parent.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.RESOLUTION);
		if (this.fullscreenToggle.isOn)
		{
			int resolutionIndex = this.GetResolutionIndex(this.originalSettings.resolution);
			if (resolutionIndex != -1)
			{
				this.resolutionDropdown.value = resolutionIndex;
			}
		}
		this.CanvasScalers = global::UnityEngine.Object.FindObjectsOfType<KCanvasScaler>();
		this.UpdateSliderLabel();
		this.uiScaleSlider.onValueChanged.AddListener(new UnityAction<float>(this.UpdateUIScale));
	}

	public static void SetResolutionFromPrefs()
	{
		int num = Screen.currentResolution.width;
		int num2 = Screen.currentResolution.height;
		int num3 = Screen.currentResolution.refreshRate;
		bool flag = Screen.fullScreen;
		Output.Log(new object[] { string.Format("Starting up with a resolution of {0}x{1} @{2}hz (fullscreen: {3})", new object[] { num, num2, num3, flag }) });
		if ((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) && KPlayerPrefs.HasKey(GraphicsOptionsScreen.ResolutionWidthKey) && KPlayerPrefs.HasKey(GraphicsOptionsScreen.ResolutionHeightKey))
		{
			Output.Log(new object[] { "Found OSX player prefs resolution, overriding with that" });
			num = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ResolutionWidthKey);
			num2 = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ResolutionHeightKey);
			num3 = KPlayerPrefs.GetInt(GraphicsOptionsScreen.RefreshRateKey, Screen.currentResolution.refreshRate);
			flag = KPlayerPrefs.GetInt(GraphicsOptionsScreen.FullScreenKey, (!Screen.fullScreen) ? 0 : 1) == 1;
		}
		else if (num <= 1 || num2 <= 1)
		{
			Output.LogWarning(new object[] { "Detected a degenerate resolution, attempting to fix..." });
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
					text += string.Format("\n{0}x{1} @ {2}", resolution4.width, resolution4.height, resolution4.refreshRate);
				}
				Output.LogError(text);
			}
		}
		Output.Log(new object[] { string.Format("Reapplying a resolution of {0}x{1} @{2}hz (fullscreen: {3})", new object[] { num, num2, num3, flag }) });
		Screen.SetResolution(num, num2, flag, num3);
	}

	private void SaveResolutionToPrefs(GraphicsOptionsScreen.Settings settings)
	{
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			KPlayerPrefs.SetInt(GraphicsOptionsScreen.ResolutionWidthKey, settings.resolution.width);
			KPlayerPrefs.SetInt(GraphicsOptionsScreen.ResolutionHeightKey, settings.resolution.height);
			KPlayerPrefs.SetInt(GraphicsOptionsScreen.RefreshRateKey, settings.resolution.refreshRate);
			KPlayerPrefs.SetInt(GraphicsOptionsScreen.FullScreenKey, (!settings.fullscreen) ? 0 : 1);
		}
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
		foreach (Resolution resolution in Screen.resolutions)
		{
			if (resolution.height >= 720)
			{
				this.options.Add(new Dropdown.OptionData(resolution.ToString()));
				this.resolutions.Add(resolution);
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
			}
		};
	}

	private void OnApply()
	{
		try
		{
			GraphicsOptionsScreen.Settings new_settings = default(GraphicsOptionsScreen.Settings);
			new_settings.resolution = this.resolutions[this.resolutionDropdown.value];
			new_settings.fullscreen = this.fullscreenToggle.isOn;
			this.ApplyConfirmSettings(new_settings, delegate
			{
				this.applyButton.isInteractable = false;
				this.revertButton.isInteractable = true;
				this.SaveResolutionToPrefs(new_settings);
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
			stringBuilder.Append("FullScreen: " + this.fullscreenToggle.isOn.ToString());
			Output.LogError(stringBuilder.ToString());
			throw ex;
		}
	}

	private void OnRevert()
	{
		this.ApplyConfirmSettings(this.originalSettings, delegate
		{
			this.applyButton.isInteractable = false;
			this.revertButton.isInteractable = false;
			this.SaveResolutionToPrefs(this.originalSettings);
		});
	}

	public void OnDone()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshApplyButton()
	{
		GraphicsOptionsScreen.Settings settings = this.CaptureSettings();
		if (this.fullscreenToggle.isOn != settings.fullscreen)
		{
			this.applyButton.isInteractable = true;
		}
		else if (this.resDropdownAlwaysActive || this.fullscreenToggle.isOn)
		{
			int resolutionIndex = this.GetResolutionIndex(settings.resolution);
			this.applyButton.isInteractable = this.resolutionDropdown.value != resolutionIndex;
		}
		else
		{
			this.applyButton.isInteractable = false;
		}
	}

	private void OnFullscreenToggle(bool enabled)
	{
		this.resolutionDropdown.interactable = this.resDropdownAlwaysActive || this.fullscreenToggle.isOn;
		this.RefreshApplyButton();
	}

	private void OnResolutionChanged(int idx)
	{
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
		this.confirmDialog.PopupConfirmDialog(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES.text, on_confirm, action, null, null, null, null, null, null);
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
	private Toggle fullscreenToggle;

	[SerializeField]
	private KButton applyButton;

	[SerializeField]
	private KButton revertButton;

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

	private KCanvasScaler[] CanvasScalers;

	private ConfirmDialogScreen confirmDialog;

	private List<Resolution> resolutions = new List<Resolution>();

	private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

	private GraphicsOptionsScreen.Settings originalSettings;

	private bool resDropdownAlwaysActive;

	private struct Settings
	{
		public bool fullscreen;

		public Resolution resolution;
	}
}
