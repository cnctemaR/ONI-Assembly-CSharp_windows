using System;
using System.IO;
using FMOD.Studio;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseScreen : KModalButtonMenu
{
	public override bool IsModal()
	{
		return true;
	}

	public static PauseScreen Instance
	{
		get
		{
			return PauseScreen.instance;
		}
	}

	protected override void OnPrefabInit()
	{
		this.keepMenuOpen = true;
		this.versionText.text = "Game Version: " + 208689U;
		if (Debug.isDebugBuild)
		{
			Text text = this.versionText;
			text.text += "(debug)";
		}
		base.OnPrefabInit();
		this.buttons = new KButtonMenu.ButtonInfo[]
		{
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, global::Action.NumActions, new UnityAction(this.OnResume), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, global::Action.NumActions, new UnityAction(this.OnOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVE, global::Action.NumActions, new UnityAction(this.OnSave), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVEAS, global::Action.NumActions, new UnityAction(this.OnSaveAs), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOAD, global::Action.NumActions, new UnityAction(this.OnLoad), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, global::Action.NumActions, new UnityAction(this.OnQuit), null, null)
		};
		this.closeButton.onClick += this.OnResume;
		PauseScreen.instance = this;
		base.Show(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.PAUSE_SCREEN.TITLE);
	}

	private void OnResume()
	{
		ToolTipScreen.Instance.ClearToolTip(this.closeButton.GetComponent<ToolTip>());
		base.Show(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.OnEscapeMenu(false);
		MusicManager.instance.StopSong("Music_ESC_Menu", true);
	}

	private void OnOptions()
	{
		global::Util.KInstantiateUI(this.optionsScreen.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnSaveAs()
	{
		global::Util.KInstantiateUI<SaveScreen>(this.saveScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnSave()
	{
		string filename = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
		{
			base.gameObject.SetActive(false);
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, this.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, Path.GetFileNameWithoutExtension(filename)), delegate
			{
				this.DoSave(filename);
				this.gameObject.SetActive(true);
			}, new global::System.Action(this.OnCancelPopup), null, null);
		}
		else
		{
			this.OnSaveAs();
		}
	}

	private void DoSave(string filename)
	{
		SaveLoader.Instance.Save(filename, false, true);
	}

	private void ConfirmDecision(string text, global::System.Action onConfirm)
	{
		base.gameObject.SetActive(false);
		ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, this.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		confirmDialogScreen.PopupConfirmDialog(text, onConfirm, new global::System.Action(this.OnCancelPopup), null, null);
	}

	private void OnLoad()
	{
		global::Util.KInstantiateUI<LoadScreen>(this.loadScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnQuit()
	{
		this.ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, new global::System.Action(this.OnQuitConfirm));
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(true);
	}

	private void OnLoadConfirm()
	{
		LoadScreen.ForceStopGame();
		this.Deactivate();
		App.LoadScene("frontend");
	}

	private void OnQuitConfirm()
	{
		this.Deactivate();
		MusicManager.instance.StopDynamicMusic();
		PauseScreen.TriggerQuitGame();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			base.Show(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.OnEscapeMenu(false);
			MusicManager.instance.StopSong("Music_ESC_Menu", true);
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public static void TriggerQuitGame()
	{
		WorldGen.Reset();
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
	private Text versionText;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private LocText title;

	private float originalTimeScale;

	private static PauseScreen instance;
}
