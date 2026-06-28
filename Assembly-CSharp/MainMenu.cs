using System;
using System.IO;
using FMOD.Studio;
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
		this.Button_Translations.onClick += this.Translations;
		this.Button_Translations.gameObject.SetActive(false);
		if (SaveLoader.GetSaveFileCount() == 0)
		{
			this.Button_LoadGame.isInteractable = false;
		}
		if (RuntimeManager.Instance != null && !RuntimeManager.Instance.initializedSuccessfully)
		{
			ConfirmDialogScreen confirmDialogScreen = global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupe;
			confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.AUDIODRIVERSCREEN.WARNING, null, null, null, null);
		}
		if (PatchNotesScreen.ShouldShowScreen())
		{
			this.patchNotesScreen.SetActive(true);
		}
		this.lastUpdateTime = Time.unscaledTime;
	}

	public void RefreshMainMenu()
	{
		this.RefreshResumeButton();
		if (SaveLoader.GetSaveFileCount() == 0)
		{
			this.Button_LoadGame.isInteractable = false;
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
		if (SteamManager.Initialized && SteamUGCService.HasInstalledLanguage())
		{
			Output.Log(new object[] { "Installing language pack " + SteamUGCService.Instance.GetInstalledLanguageData() });
			SteamUGCService.SetFontForLocalization();
		}
		base.OnSpawn();
		Canvas.ForceUpdateCanvases();
	}

	private void ResumeGame()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		if (!string.IsNullOrEmpty(latestSaveFile))
		{
			SaveLoader.SetActiveSaveFilePath(latestSaveFile);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		this.TriggerLoadingMusic();
		WorldGen.Reset();
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			File.Delete(WorldGen.SIM_SAVE_FILENAME);
		}
		catch (Exception ex)
		{
			Output.LogWarning(new object[] { ex.ToString() });
		}
		global::Util.KInstantiateUI(ScreenPrefabs.Instance.WorldGenScreen.gameObject, base.gameObject, true);
		global::UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(false);
	}

	private void LoadGame()
	{
		if (LoadScreen.Instance == null)
		{
			GameObject gameObject = global::Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, true);
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
			this.RefreshResumeButton();
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
				SaveGame.Header header;
				SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(latestSaveFile, out header);
				if (header.buildVersion > 219330U || gameInfo.saveMajorVersion < 7)
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
				this.Button_ResumeGame.GetComponent<ToolTip>().toolTip = fileNameWithoutExtension;
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
		Application.OpenURL("http://forums.kleientertainment.com/topic/74765-creatingusing-translation-files/");
	}

	private void Options()
	{
		OptionsMenuScreen optionsMenuScreen = global::Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
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

	private void TriggerLoadingMusic()
	{
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f, true);
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
}
