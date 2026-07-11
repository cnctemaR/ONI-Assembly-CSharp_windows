using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using Steamworks;
using STRINGS;
using UnityEngine;

public class MainMenu : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Global.Instance.modManager.DeactivateWorldGenMod();
		List<MainMenu.ButtonInfo> list = new List<MainMenu.ButtonInfo>
		{
			new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, new global::System.Action(this.NewGame), 22),
			new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, new global::System.Action(this.LoadGame), 14),
			new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, new global::System.Action(this.Translations), 14),
			new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, new global::System.Action(this.Options), 14),
			new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, new global::System.Action(this.QuitGame), 14)
		};
		if (!DistributionPlatform.Initialized)
		{
			int num = list.FindIndex((MainMenu.ButtonInfo x) => x.text == UI.FRONTEND.MAINMENU.TRANSLATIONS);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
		}
		foreach (MainMenu.ButtonInfo buttonInfo in list)
		{
			KButton kbutton = Util.KInstantiateUI<KButton>(this.buttonPrefab.gameObject, this.buttonParent, true);
			kbutton.onClick += buttonInfo.action;
			LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
			componentInChildren.text = buttonInfo.text;
			componentInChildren.fontSize = (float)buttonInfo.fontSize;
		}
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		this.RefreshResumeButton();
		this.Button_ResumeGame.onClick += this.ResumeGame;
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
		global::Debug.Log("-- MAIN MENU -- ", null);
		base.OnSpawn();
		Canvas.ForceUpdateCanvases();
		this.ShowLanguageConfirmation();
		string savePrefix = SaveLoader.GetSavePrefix();
		try
		{
			string text = Path.Combine(savePrefix, "__SPCCHK");
			using (FileStream fileStream = File.OpenWrite(text))
			{
				byte[] array = new byte[1024];
				for (int i = 0; i < 15360; i++)
				{
					fileStream.Write(array, 0, array.Length);
				}
			}
			File.Delete(text);
		}
		catch (Exception ex)
		{
			string text2;
			if (ex is IOException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, savePrefix);
			}
			else
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, savePrefix);
			}
			string text3 = string.Format(text2, savePrefix);
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.PopupConfirmDialog(text3, null, null, null, null, null, null, null, null);
		}
		if (GenericGameSettings.instance.autoResumeGame)
		{
			this.ResumeGame();
		}
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
		this.GameSettingsScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.ModeSelectScreen.gameObject, base.gameObject, true);
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
				global::System.DateTime lastWriteTime = File.GetLastWriteTime(latestSaveFile);
				MainMenu.SaveFileEntry saveFileEntry = default(MainMenu.SaveFileEntry);
				SaveGame.Header header = default(SaveGame.Header);
				SaveGame.GameInfo gameInfo = default(SaveGame.GameInfo);
				if (!this.saveFileEntries.TryGetValue(latestSaveFile, out saveFileEntry) || saveFileEntry.timeStamp != lastWriteTime)
				{
					gameInfo = SaveLoader.LoadHeader(latestSaveFile, out header);
					saveFileEntry = new MainMenu.SaveFileEntry
					{
						timeStamp = lastWriteTime,
						header = header,
						headerData = gameInfo
					};
					this.saveFileEntries[latestSaveFile] = saveFileEntry;
				}
				else
				{
					header = saveFileEntry.header;
					gameInfo = saveFileEntry.headerData;
				}
				if (header.buildVersion > 291640U || gameInfo.saveMajorVersion < 7)
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveFile);
				if (!string.IsNullOrEmpty(gameInfo.baseName))
				{
					this.Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = string.Format(UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, gameInfo.baseName, gameInfo.numberOfCycles + 1);
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
		LanguageOptionsScreen languageOptionsScreen = Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject, false);
		languageOptionsScreen.SetBackgroundActive(true);
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
		if (!KFMOD.didFmodInitializeSuccessfully)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			ConfirmDialogScreen confirmDialogScreen2 = confirmDialogScreen;
			string text = UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS;
			global::System.Action action = null;
			global::System.Action action2 = null;
			string text2 = UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO;
			global::System.Action action3 = delegate
			{
				Application.OpenURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
			};
			Sprite sadDupeAudio = GlobalResources.Instance().sadDupeAudio;
			confirmDialogScreen2.PopupConfirmDialog(text, action, action2, text2, action3, null, null, null, sadDupeAudio);
		}
	}

	private void CheckDoubleBoundKeys()
	{
		string text = string.Empty;
		HashSet<BindingEntry> hashSet = new HashSet<BindingEntry>();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			if (GameInputMapping.KeyBindings[i].mKeyCode != KKeyCode.Mouse1)
			{
				for (int j = 0; j < GameInputMapping.KeyBindings.Length; j++)
				{
					if (i != j)
					{
						BindingEntry bindingEntry = GameInputMapping.KeyBindings[j];
						if (!hashSet.Contains(bindingEntry))
						{
							BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
							if (bindingEntry2.mKeyCode != KKeyCode.None && bindingEntry2.mKeyCode == bindingEntry.mKeyCode && bindingEntry2.mModifier == bindingEntry.mModifier && bindingEntry2.mRebindable && bindingEntry.mRebindable)
							{
								string mGroup = GameInputMapping.KeyBindings[i].mGroup;
								string mGroup2 = GameInputMapping.KeyBindings[j].mGroup;
								if (mGroup == "Root" || mGroup2 == "Root" || mGroup == mGroup2)
								{
									if (!(mGroup == "Root") || !bindingEntry.mIgnoreRootConflics)
									{
										if (!(mGroup2 == "Root") || !bindingEntry2.mIgnoreRootConflics)
										{
											string text2 = text;
											text = string.Concat(new object[] { text2, "\n\n", bindingEntry2.mAction, ": <b>", bindingEntry2.mKeyCode, "</b>\n", bindingEntry.mAction, ": <b>", bindingEntry.mKeyCode, "</b>" });
											BindingEntry bindingEntry3 = bindingEntry2;
											bindingEntry3.mKeyCode = KKeyCode.None;
											bindingEntry3.mModifier = Modifier.None;
											GameInputMapping.KeyBindings[i] = bindingEntry3;
											bindingEntry3 = bindingEntry;
											bindingEntry3.mKeyCode = KKeyCode.None;
											bindingEntry3.mModifier = Modifier.None;
											GameInputMapping.KeyBindings[j] = bindingEntry3;
										}
									}
								}
							}
						}
					}
				}
				hashSet.Add(GameInputMapping.KeyBindings[i]);
			}
		}
		if (text != string.Empty)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			ConfirmDialogScreen confirmDialogScreen2 = confirmDialogScreen;
			string text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text);
			global::System.Action action = null;
			global::System.Action action2 = null;
			Sprite sadDupe = GlobalResources.Instance().sadDupe;
			confirmDialogScreen2.PopupConfirmDialog(text2, action, action2, null, null, null, null, null, sadDupe);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}

	public RectTransform LogoAndMenu;

	public KButton Button_ResumeGame;

	public GameObject patchNotesScreen;

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private GameObject GameSettingsScreen;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;

	private Dictionary<string, MainMenu.SaveFileEntry> saveFileEntries = new Dictionary<string, MainMenu.SaveFileEntry>();

	private struct ButtonInfo
	{
		public ButtonInfo(LocString text, global::System.Action action, int font_size)
		{
			this.text = text;
			this.action = action;
			this.fontSize = font_size;
		}

		public LocString text;

		public global::System.Action action;

		public int fontSize;
	}

	private struct SaveFileEntry
	{
		public global::System.DateTime timeStamp;

		public SaveGame.Header header;

		public SaveGame.GameInfo headerData;
	}
}
