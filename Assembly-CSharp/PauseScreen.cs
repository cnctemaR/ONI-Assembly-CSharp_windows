using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseScreen : KModalButtonMenu
{
	public static PauseScreen Instance
	{
		get
		{
			return PauseScreen.instance;
		}
	}

	public static void DestroyInstance()
	{
		PauseScreen.instance = null;
	}

	protected override void OnPrefabInit()
	{
		this.keepMenuOpen = true;
		base.OnPrefabInit();
		this.ConfigureButtonInfos();
		this.closeButton.onClick += this.OnResume;
		PauseScreen.instance = this;
		this.Show(false);
	}

	private void ConfigureButtonInfos()
	{
		if (!GenericGameSettings.instance.demoMode)
		{
			this.buttons = new KButtonMenu.ButtonInfo[]
			{
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, global::Action.NumActions, new UnityAction(this.OnResume), null, null),
				new KButtonMenu.ButtonInfo(this.recentlySaved ? UI.FRONTEND.PAUSE_SCREEN.ALREADY_SAVED : UI.FRONTEND.PAUSE_SCREEN.SAVE, global::Action.NumActions, new UnityAction(this.OnSave), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVEAS, global::Action.NumActions, new UnityAction(this.OnSaveAs), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOAD, global::Action.NumActions, new UnityAction(this.OnLoad), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, global::Action.NumActions, new UnityAction(this.OnOptions), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.COLONY_SUMMARY, global::Action.NumActions, new UnityAction(this.OnColonySummary), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOCKERMENU, global::Action.NumActions, new UnityAction(this.OnLockerMenu), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, global::Action.NumActions, new UnityAction(this.OnQuit), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, global::Action.NumActions, new UnityAction(this.OnDesktopQuit), null, null)
			};
			return;
		}
		this.buttons = new KButtonMenu.ButtonInfo[]
		{
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, global::Action.NumActions, new UnityAction(this.OnResume), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, global::Action.NumActions, new UnityAction(this.OnOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, global::Action.NumActions, new UnityAction(this.OnQuit), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, global::Action.NumActions, new UnityAction(this.OnDesktopQuit), null, null)
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.clipboard.GetText = new Func<string>(this.GetClipboardText);
		this.title.SetText(UI.FRONTEND.PAUSE_SCREEN.TITLE);
		try
		{
			string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
			string[] array = CustomGameSettings.ParseSettingCoordinate(settingsCoordinate);
			this.worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, settingsCoordinate));
			this.worldSeed.GetComponent<ToolTip>().toolTip = string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED_TOOLTIP, new object[]
			{
				array[1],
				array[2],
				array[3],
				array[4],
				array[5]
			});
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(string.Format("Failed to load Coordinates on ClusterLayout {0}, please report this error on the forums", ex));
			CustomGameSettings.Instance.Print();
			global::Debug.Log("ClusterCache: " + string.Join(",", SettingsCache.clusterLayouts.clusterCache.Keys));
			this.worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, "0"));
		}
	}

	public override float GetSortKey()
	{
		return 30f;
	}

	private string GetClipboardText()
	{
		string text;
		try
		{
			text = CustomGameSettings.Instance.GetSettingsCoordinate();
		}
		catch
		{
			text = "";
		}
		return text;
	}

	private void OnResume()
	{
		this.Show(false);
	}

	protected override void OnShow(bool show)
	{
		this.recentlySaved = false;
		this.ConfigureButtonInfos();
		base.OnShow(show);
		if (show)
		{
			this.RefreshButtons();
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
			MusicManager.instance.OnEscapeMenu(true);
			MusicManager.instance.PlaySong("Music_ESC_Menu", false);
			this.RefreshDLCActivationButtons();
			return;
		}
		ToolTipScreen.Instance.ClearToolTip(this.closeButton.GetComponent<ToolTip>());
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.OnEscapeMenu(false);
		if (MusicManager.instance.SongIsPlaying("Music_ESC_Menu"))
		{
			MusicManager.instance.StopSong("Music_ESC_Menu", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	private void OnOptions()
	{
		base.ActivateChildScreen(this.optionsScreen.gameObject);
	}

	private void OnSaveAs()
	{
		base.ActivateChildScreen(this.saveScreenPrefab.gameObject);
	}

	private void OnSave()
	{
		string filename = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
		{
			base.gameObject.SetActive(false);
			((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, global::System.IO.Path.GetFileNameWithoutExtension(filename)), delegate
			{
				this.DoSave(filename);
				this.gameObject.SetActive(true);
			}, new global::System.Action(this.OnCancelPopup), null, null, null, null, null, null);
			return;
		}
		this.OnSaveAs();
	}

	public void OnSaveComplete()
	{
		this.recentlySaved = true;
		this.ConfigureButtonInfos();
		this.RefreshButtons();
	}

	private void DoSave(string filename)
	{
		try
		{
			SaveLoader.Instance.Save(filename, false, true);
			this.OnSaveComplete();
		}
		catch (IOException ex)
		{
			IOException ex2 = ex;
			IOException e = ex2;
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.IO_ERROR, e.ToString()), delegate
			{
				this.Deactivate();
			}, null, UI.FRONTEND.SAVESCREEN.REPORT_BUG, delegate
			{
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, null, true, new string[] { KCrashReporter.CRASH_CATEGORY.FILEIO }, null);
			}, null, null, null, null);
		}
	}

	private void ConfirmDecision(string questionText, string primaryButtonText, global::System.Action primaryButtonAction, string alternateButtonText = null, global::System.Action alternateButtonAction = null)
	{
		base.gameObject.SetActive(false);
		((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(questionText, primaryButtonAction, new global::System.Action(this.OnCancelPopup), alternateButtonText, alternateButtonAction, null, primaryButtonText, null, null);
	}

	private void OnLoad()
	{
		base.ActivateChildScreen(this.loadScreenPrefab.gameObject);
	}

	private void OnColonySummary()
	{
		RetiredColonyData currentColonyRetiredColonyData = RetireColonyUtility.GetCurrentColonyRetiredColonyData();
		MainMenu.ActivateRetiredColoniesScreenFromData(PauseScreen.Instance.transform.parent.gameObject, currentColonyRetiredColonyData);
	}

	private void OnLockerMenu()
	{
		LockerMenuScreen.Instance.Show(true);
	}

	private void OnQuit()
	{
		this.ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, UI.FRONTEND.MAINMENU.SAVEANDQUITTITLE, delegate
		{
			this.OnQuitConfirm(true);
		}, UI.FRONTEND.MAINMENU.QUIT, delegate
		{
			this.OnQuitConfirm(false);
		});
	}

	private void OnDesktopQuit()
	{
		this.ConfirmDecision(UI.FRONTEND.MAINMENU.DESKTOPQUITCONFIRM, UI.FRONTEND.MAINMENU.SAVEANDQUITDESKTOP, delegate
		{
			this.OnDesktopQuitConfirm(true);
		}, UI.FRONTEND.MAINMENU.QUIT, delegate
		{
			this.OnDesktopQuitConfirm(false);
		});
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(true);
	}

	private void OnLoadConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			LoadScreen.ForceStopGame();
			this.Deactivate();
			App.LoadScene("frontend");
		});
	}

	private void OnRetireConfirm()
	{
		RetireColonyUtility.SaveColonySummaryData();
	}

	private void OnQuitConfirm(bool saveFirst)
	{
		if (saveFirst)
		{
			string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
			if (!string.IsNullOrEmpty(activeSaveFilePath) && File.Exists(activeSaveFilePath))
			{
				this.DoSave(activeSaveFilePath);
			}
			else
			{
				this.OnSaveAs();
			}
		}
		LoadingOverlay.Load(delegate
		{
			this.Deactivate();
			PauseScreen.TriggerQuitGame();
		});
	}

	private void OnDesktopQuitConfirm(bool saveFirst)
	{
		if (saveFirst)
		{
			string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
			if (!string.IsNullOrEmpty(activeSaveFilePath) && File.Exists(activeSaveFilePath))
			{
				this.DoSave(activeSaveFilePath);
			}
			else
			{
				this.OnSaveAs();
			}
		}
		App.Quit();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Show(false);
			return;
		}
		base.OnKeyDown(e);
	}

	public static void TriggerQuitGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.EndGame();
		LoadScreen.ForceStopGame();
		App.LoadScene("frontend");
	}

	private void RefreshDLCActivationButtons()
	{
		foreach (KeyValuePair<string, DlcManager.DlcInfo> keyValuePair in DlcManager.DLC_PACKS)
		{
			if (!keyValuePair.Value.isCosmetic && !this.dlcActivationButtons.ContainsKey(keyValuePair.Key))
			{
				GameObject gameObject = global::Util.KInstantiateUI(this.dlcActivationButtonPrefab, this.dlcActivationButtonPrefab.transform.parent.gameObject, true);
				Sprite sprite = Assets.GetSprite(DlcManager.GetDlcSmallLogo(keyValuePair.Key));
				gameObject.GetComponent<Image>().sprite = sprite;
				gameObject.GetComponent<MultiToggle>().states[0].sprite = sprite;
				gameObject.GetComponent<MultiToggle>().states[1].sprite = sprite;
				this.dlcActivationButtons.Add(keyValuePair.Key, gameObject);
			}
		}
		this.RefreshDLCButton("EXPANSION1_ID", this.dlc1ActivationButton, false);
		foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.dlcActivationButtons)
		{
			this.RefreshDLCButton(keyValuePair2.Key, keyValuePair2.Value.GetComponent<MultiToggle>(), true);
		}
	}

	private void RefreshDLCButton(string DLCID, MultiToggle button, bool userEditable)
	{
		button.GetComponent<MultiToggle>().states[0].sprite = Assets.GetSprite(DlcManager.GetDlcSmallLogo(DLCID));
		button.GetComponent<MultiToggle>().states[1].sprite = Assets.GetSprite(DlcManager.GetDlcSmallLogo(DLCID));
		button.ChangeState(Game.IsDlcActiveForCurrentSave(DLCID) ? 1 : 0);
		button.GetComponent<Image>().material = (Game.IsDlcActiveForCurrentSave(DLCID) ? GlobalResources.Instance().AnimUIMaterial : GlobalResources.Instance().AnimMaterialUIDesaturated);
		ToolTip component = button.GetComponent<ToolTip>();
		string dlcTitle = DlcManager.GetDlcTitle(DLCID);
		if (!DlcManager.IsContentSubscribed(DLCID))
		{
			component.SetSimpleTooltip(string.Format(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.DLC_DISABLED_NOT_EDITABLE_TOOLTIP, dlcTitle));
			button.onClick = null;
			return;
		}
		if (userEditable)
		{
			component.SetSimpleTooltip(Game.IsDlcActiveForCurrentSave(DLCID) ? string.Format(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.DLC_ENABLED_TOOLTIP, dlcTitle) : string.Format(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.DLC_DISABLED_TOOLTIP, dlcTitle));
			button.onClick = delegate
			{
				this.OnClickAddDLCButton(DLCID);
			};
			return;
		}
		component.SetSimpleTooltip(Game.IsDlcActiveForCurrentSave(DLCID) ? string.Format(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.DLC_ENABLED_TOOLTIP, dlcTitle) : string.Format(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.DLC_DISABLED_NOT_EDITABLE_TOOLTIP, dlcTitle));
		button.onClick = null;
	}

	private void OnClickAddDLCButton(string dlcID)
	{
		if (!Game.IsDlcActiveForCurrentSave(dlcID))
		{
			this.ConfirmDecision(UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.ENABLE_QUESTION, UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.CONFIRM, delegate
			{
				this.OnConfirmAddDLC(dlcID);
			}, null, null);
		}
	}

	private void OnConfirmAddDLC(string dlcId)
	{
		SaveLoader.Instance.UpgradeActiveSaveDLCInfo(dlcId, true);
	}

	[SerializeField]
	private OptionsMenuScreen optionsScreen;

	[SerializeField]
	private SaveScreen saveScreenPrefab;

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private LocText worldSeed;

	[SerializeField]
	private CopyTextFieldToClipboard clipboard;

	[SerializeField]
	private MultiToggle dlc1ActivationButton;

	[SerializeField]
	private GameObject dlcActivationButtonPrefab;

	private Dictionary<string, GameObject> dlcActivationButtons = new Dictionary<string, GameObject>();

	private float originalTimeScale;

	private bool recentlySaved;

	private static PauseScreen instance;
}
