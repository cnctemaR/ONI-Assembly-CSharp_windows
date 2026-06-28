using System;
using System.IO;
using FMODUnity;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		this.StartFEAudio();
		this.RefreshResumeButton();
		this.Button_ResumeGame.onClick += this.ResumeGame;
		this.Button_NewGame.onClick += this.NewGame;
		this.Button_LoadGame.onClick += this.LoadGame;
		this.Button_Options.onClick += this.Options;
		this.Button_QuitGame.onClick += this.QuitGame;
		if (GenericGameSettings.instance != null && GenericGameSettings.instance.demoMode)
		{
			this.Button_ResumeGame.gameObject.SetActive(false);
			this.Button_LoadGame.gameObject.SetActive(false);
		}
		this.Button_Translations.onClick += this.Translations;
		if (SaveLoader.GetSaveFileCount() == 0)
		{
			this.Button_LoadGame.isInteractable = false;
		}
		this.CheckForCommonIssues();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			this.patchNotesScreen.SetActive(true);
		}
		this.lastUpdateTime = Time.unscaledTime;
		KPlayerPrefs.DeleteKey(Game.BaseAlreadyCreatedKey);
	}

	public void RefreshMainMenu()
	{
		this.RefreshResumeButton();
		if (SaveLoader.GetSaveFileCount() == 0)
		{
			this.Button_LoadGame.isInteractable = false;
		}
		else
		{
			this.Button_LoadGame.isInteractable = true;
		}
	}

	private void PlayMouseOverSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	private void PlayMouseClickSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Canvas.ForceUpdateCanvases();
	}

	private void ResumeGame()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		if (!string.IsNullOrEmpty(latestSaveFile))
		{
			KCrashReporter.MOST_RECENT_SAVEFILE = latestSaveFile;
			SaveLoader.SetActiveSaveFilePath(latestSaveFile);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		if (this.GameSettingsScreen == null)
		{
			this.GameSettingsScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.NewGameSettingsScreen.gameObject, base.gameObject, true);
		}
		else
		{
			this.GameSettingsScreen.GetComponent<KScreen>().Show(true);
		}
	}

	private void LoadGame()
	{
		if (LoadScreen.Instance == null)
		{
			GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, true);
			LoadScreen component = gameObject.GetComponent<LoadScreen>();
			component.requireConfirmation = false;
			component.SetBackgroundActive(true);
		}
		LoadScreen.Instance.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (Time.unscaledTime - this.lastUpdateTime > 1f)
		{
			this.RefreshMainMenu();
			this.lastUpdateTime = Time.unscaledTime;
		}
	}

	private void RefreshResumeButton()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		bool flag = !string.IsNullOrEmpty(latestSaveFile) && File.Exists(latestSaveFile);
		if (flag)
		{
			try
			{
				if (GenericGameSettings.instance.demoMode)
				{
					flag = false;
				}
				SaveGame.Header header;
				SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(latestSaveFile, out header);
				if (header.buildVersion > 229531U || gameInfo.saveMajorVersion < 7)
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveFile);
				if (!string.IsNullOrEmpty(gameInfo.baseName))
				{
					this.Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = string.Format(UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, gameInfo.baseName, gameInfo.numberOfCycles);
				}
				else
				{
					this.Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = fileNameWithoutExtension;
				}
			}
			catch (Exception ex)
			{
				global::Debug.LogWarning(ex, null);
				flag = false;
			}
		}
		if (this.Button_ResumeGame != null && this.Button_ResumeGame.gameObject != null)
		{
			this.Button_ResumeGame.gameObject.SetActive(flag);
		}
		else
		{
			global::Debug.LogWarning("Why is the resume game button null?", null);
		}
	}

	private void Translations()
	{
		if (SteamManager.Initialized)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, this.transform.parent.gameObject, false);
		}
		else
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.NO_STEAM, null, null, null, null);
		}
	}

	private void Options()
	{
		OptionsMenuScreen optionsMenuScreen = Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
		optionsMenuScreen.SetBackgroundActive(true);
	}

	private void QuitGame()
	{
		if (!Application.isEditor)
		{
			Application.Quit();
		}
	}

	public void StartFEAudio()
	{
		AudioMixer.instance.Reset();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
		{
			AudioMixer.instance.StartUserVolumesSnapshot();
		}
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_TitleTheme"))
		{
			MusicManager.instance.PlaySong("Music_TitleTheme", false);
		}
	}

	private void CheckForCommonIssues()
	{
		this.CheckForAudioDriverIssue();
		this.CheckForSavePathIssue();
	}

	private void CheckForAudioDriverIssue()
	{
		if (RuntimeManager.Instance != null && !RuntimeManager.Instance.initializedSuccessfully)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupeAudio;
			confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, null, null);
		}
	}

	private void CheckForSavePathIssue()
	{
		string savePrefix = SaveLoader.GetSavePrefix();
		string text = "testfile";
		string text2 = "testsavefile";
		bool flag;
		try
		{
			FileStream fileStream = File.Open(savePrefix + text, FileMode.Create, FileAccess.Write);
			new BinaryWriter(fileStream);
			fileStream.Close();
			flag = false;
		}
		catch
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupe;
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, savePrefix), null, null, null, null);
			flag = true;
		}
		if (!flag)
		{
			FileStream fileStream2 = File.Open(savePrefix + text2, FileMode.Create, FileAccess.Write);
			try
			{
				fileStream2.SetLength(15000000L);
				new BinaryWriter(fileStream2);
				fileStream2.Close();
			}
			catch
			{
				fileStream2.Close();
				ConfirmDialogScreen confirmDialogScreen2 = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
				confirmDialogScreen2.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupe;
				confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, savePrefix), null, null, null, null);
			}
		}
		if (File.Exists(savePrefix + text))
		{
			File.Delete(savePrefix + text);
		}
		if (File.Exists(savePrefix + text2))
		{
			File.Delete(savePrefix + text2);
		}
	}

	public RectTransform LogoAndMenu;

	public KButton Button_ResumeGame;

	public KButton Button_NewGame;

	public KButton Button_LoadGame;

	public KButton Button_Translations;

	public KButton Button_Options;

	public KButton Button_QuitGame;

	public GameObject patchNotesScreen;

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private GameObject GameSettingsScreen;
}
