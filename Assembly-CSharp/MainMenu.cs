using System;
using System.Collections.Generic;
using System.IO;
using FMODUnity;
using Klei;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		this.RefreshResumeButton();
		this.Button_ResumeGame.onClick += this.ResumeGame;
		this.Button_NewGame.onClick += this.NewGame;
		this.Button_LoadGame.onClick += this.LoadGame;
		this.Button_Options.onClick += this.Options;
		this.Button_QuitGame.onClick += this.QuitGame;
		this.Button_Translations.onClick += this.Translations;
		if (GenericGameSettings.instance != null && GenericGameSettings.instance.demoMode)
		{
			this.Button_ResumeGame.gameObject.SetActive(false);
			this.Button_LoadGame.gameObject.SetActive(false);
			this.Button_Options.gameObject.SetActive(false);
			this.Button_Translations.gameObject.SetActive(false);
			this.topLeftAlphaMessage.gameObject.SetActive(false);
		}
		if (SaveLoader.GetSaveFileCount() == 0)
		{
			this.Button_LoadGame.isInteractable = false;
		}
		this.StartFEAudio();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			this.patchNotesScreen.SetActive(true);
		}
		this.CheckDoubleBoundKeys();
		this.lastUpdateTime = Time.unscaledTime;
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
		this.ShowLanguageConfirmation();
	}

	private void ShowLanguageConfirmation()
	{
		if (SteamManager.Initialized)
		{
			string steamUILanguage = SteamUtils.GetSteamUILanguage();
			if (steamUILanguage != "schinese")
			{
				return;
			}
			if (KPlayerPrefs.GetInt("LanguageConfirmationVersion") >= MainMenu.LANGUAGE_CONFIRMATION_VERSION)
			{
				return;
			}
			KPlayerPrefs.SetInt("LanguageConfirmationVersion", MainMenu.LANGUAGE_CONFIRMATION_VERSION);
			this.Translations();
		}
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
		this.GameSettingsScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.NewGameSettingsScreen.gameObject, base.gameObject, true);
		this.GameSettingsScreen.GetComponent<KScreen>().Activate();
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
				if (header.buildVersion > 236191U || gameInfo.saveMajorVersion < 7)
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
		Util.KInstantiateUI(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, this.transform.parent.gameObject, false);
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
		this.CheckForAudioDriverIssue();
	}

	private void CheckForAudioDriverIssue()
	{
		if (RuntimeManager.Instance != null && !RuntimeManager.Instance.initializedSuccessfully)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupeAudio;
			confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, null, null, null, null, null);
		}
	}

	private void CheckDoubleBoundKeys()
	{
		string text = string.Empty;
		List<BindingEntry> list = new List<BindingEntry>();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			if (GameInputMapping.KeyBindings[i].mKeyCode != KKeyCode.Mouse1)
			{
				for (int j = 0; j < GameInputMapping.KeyBindings.Length; j++)
				{
					if (i != j)
					{
						if (!list.Contains(GameInputMapping.KeyBindings[j]))
						{
							BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
							BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[j];
							if (bindingEntry.mKeyCode != KKeyCode.None && bindingEntry.mKeyCode == bindingEntry2.mKeyCode && bindingEntry.mModifier == bindingEntry2.mModifier && bindingEntry.mRebindable && bindingEntry2.mRebindable)
							{
								if (!(GameInputMapping.KeyBindings[i].mGroup != GameInputMapping.KeyBindings[j].mGroup) || GameInputMapping.KeyBindings[i].mGroup == "Root" || GameInputMapping.KeyBindings[j].mGroup == "Root")
								{
									string text2 = text;
									text = string.Concat(new object[]
									{
										text2,
										"\n\n",
										GameInputMapping.KeyBindings[i].mAction,
										": <b>",
										GameInputMapping.KeyBindings[i].mKeyCode,
										"</b>\n",
										GameInputMapping.KeyBindings[j].mAction,
										": <b>",
										GameInputMapping.KeyBindings[j].mKeyCode,
										"</b>"
									});
									BindingEntry bindingEntry3 = GameInputMapping.KeyBindings[i];
									bindingEntry3.mKeyCode = KKeyCode.None;
									bindingEntry3.mModifier = Modifier.None;
									GameInputMapping.KeyBindings[i] = bindingEntry3;
									bindingEntry3 = GameInputMapping.KeyBindings[j];
									bindingEntry3.mKeyCode = KKeyCode.None;
									bindingEntry3.mModifier = Modifier.None;
									GameInputMapping.KeyBindings[j] = bindingEntry3;
								}
							}
						}
					}
				}
				list.Add(GameInputMapping.KeyBindings[i]);
			}
		}
		if (text != string.Empty)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = GlobalResources.Instance().sadDupe;
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text), null, null, null, null, null, null, null);
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

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;
}
