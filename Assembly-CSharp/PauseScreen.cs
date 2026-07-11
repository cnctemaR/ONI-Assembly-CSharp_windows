using System;
using System.IO;
using FMOD.Studio;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

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
		if (!GenericGameSettings.instance.demoMode)
		{
			this.buttons = new KButtonMenu.ButtonInfo[]
			{
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, global::Action.NumActions, new UnityAction(this.OnResume), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVE, global::Action.NumActions, new UnityAction(this.OnSave), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVEAS, global::Action.NumActions, new UnityAction(this.OnSaveAs), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOAD, global::Action.NumActions, new UnityAction(this.OnLoad), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, global::Action.NumActions, new UnityAction(this.OnOptions), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.COLONY_SUMMARY, global::Action.NumActions, new UnityAction(this.OnColonySummary), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, global::Action.NumActions, new UnityAction(this.OnQuit), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, global::Action.NumActions, new UnityAction(this.OnDesktopQuit), null, null)
			};
		}
		else
		{
			this.buttons = new KButtonMenu.ButtonInfo[]
			{
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, global::Action.NumActions, new UnityAction(this.OnResume), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, global::Action.NumActions, new UnityAction(this.OnOptions), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, global::Action.NumActions, new UnityAction(this.OnQuit), null, null),
				new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, global::Action.NumActions, new UnityAction(this.OnDesktopQuit), null, null)
			};
		}
		this.closeButton.onClick += this.OnResume;
		PauseScreen.instance = this;
		base.Show(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.clipboard.GetText = new Func<string>(this.GetClipboardText);
		this.title.SetText(UI.FRONTEND.PAUSE_SCREEN.TITLE);
		string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
		string[] array = CustomGameSettings.Instance.ParseSettingCoordinate(settingsCoordinate);
		this.worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, settingsCoordinate));
		this.worldSeed.GetComponent<ToolTip>().toolTip = string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED_TOOLTIP, array[1], array[2], array[3]);
	}

	private string GetClipboardText()
	{
		return CustomGameSettings.Instance.GetSettingsCoordinate();
	}

	private void OnResume()
	{
		base.Show(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
			MusicManager.instance.OnEscapeMenu(true);
			MusicManager.instance.PlaySong("Music_ESC_Menu", false);
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
			((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, Path.GetFileNameWithoutExtension(filename)), delegate
			{
				this.DoSave(filename);
				this.gameObject.SetActive(true);
			}, new global::System.Action(this.OnCancelPopup), null, null, null, null, null, null, true);
			return;
		}
		this.OnSaveAs();
	}

	private void DoSave(string filename)
	{
		try
		{
			SaveLoader.Instance.Save(filename, false, true);
			ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
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
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, null, "");
			}, null, null, null, null, true);
		}
	}

	private void ConfirmDecision(string text, global::System.Action onConfirm)
	{
		base.gameObject.SetActive(false);
		((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)).PopupConfirmDialog(text, onConfirm, new global::System.Action(this.OnCancelPopup), null, null, null, null, null, null, true);
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

	private void OnQuit()
	{
		this.ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, new global::System.Action(this.OnQuitConfirm));
	}

	private void OnDesktopQuit()
	{
		this.ConfirmDecision(UI.FRONTEND.MAINMENU.DESKTOPQUITCONFIRM, new global::System.Action(this.OnDesktopQuitConfirm));
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

	private void OnQuitConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			this.Deactivate();
			PauseScreen.TriggerQuitGame();
		});
	}

	private void OnDesktopQuitConfirm()
	{
		App.Quit();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			base.Show(false);
			return;
		}
		base.OnKeyDown(e);
	}

	public static void TriggerQuitGame()
	{
		SaveGame.Instance.worldGen.Reset();
		ThreadedHttps<KleiMetrics>.Instance.EndGame();
		LoadScreen.ForceStopGame();
		App.LoadScene("frontend");
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

	private float originalTimeScale;

	private static PauseScreen instance;
}
