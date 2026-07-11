using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KScreen
{
	private KButton MakeButton(MainMenu.ButtonInfo info)
	{
		KButton kbutton = Util.KInstantiateUI<KButton>(this.buttonPrefab.gameObject, this.buttonParent, true);
		kbutton.onClick += info.action;
		LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
		componentInChildren.text = info.text;
		componentInChildren.fontSize = (float)info.fontSize;
		return kbutton;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, new global::System.Action(this.NewGame), 22));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, new global::System.Action(this.LoadGame), 14));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.RETIREDCOLONIES, delegate
		{
			MainMenu.ActivateRetiredColoniesScreen(base.transform.gameObject, "");
		}, 14));
		if (DistributionPlatform.Initialized)
		{
			this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, new global::System.Action(this.Translations), 14));
			this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MODS.TITLE, new global::System.Action(this.Mods), 14));
		}
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, new global::System.Action(this.Options), 14));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, new global::System.Action(this.QuitGame), 14));
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		this.RefreshResumeButton();
		this.Button_ResumeGame.onClick += this.ResumeGame;
		this.StartFEAudio();
		this.SpawnVideoScreen();
		this.CheckPlayerPrefsCorruption();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			this.patchNotesScreen.gameObject.SetActive(true);
		}
		this.CheckDoubleBoundKeys();
		this.topLeftAlphaMessage.gameObject.SetActive(false);
		this.nextUpdateTimer.gameObject.SetActive(false);
		this.m_motdServerClient = new MotdServerClient();
		this.m_motdServerClient.GetMotd(delegate(MotdServerClient.MotdResponse response, string error)
		{
			if (error == null)
			{
				this.topLeftAlphaMessage.gameObject.SetActive(true);
				this.nextUpdateTimer.gameObject.SetActive(true);
				this.motdImageHeader.text = response.image_header_text;
				this.motdNewsHeader.text = response.news_header_text;
				this.motdNewsBody.text = response.news_body_text;
				this.patchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
				this.nextUpdateTimer.UpdateReleaseTimes(response.last_update_time, response.next_update_time, response.update_text_override);
				if (response.image_texture != null)
				{
					this.motdImage.sprite = Sprite.Create(response.image_texture, new Rect(0f, 0f, (float)response.image_texture.width, (float)response.image_texture.height), Vector2.zero);
				}
				else
				{
					global::Debug.LogWarning("GetMotd failed to return an image texture");
				}
				if (this.motdImage.sprite != null && this.motdImage.sprite.rect.height != 0f)
				{
					AspectRatioFitter component = this.motdImage.gameObject.GetComponent<AspectRatioFitter>();
					if (component != null)
					{
						float num = this.motdImage.sprite.rect.width / this.motdImage.sprite.rect.height;
						component.aspectRatio = num;
					}
					else
					{
						global::Debug.LogWarning("Missing AspectRatioFitter on MainMenu motd image.");
					}
				}
				else
				{
					global::Debug.LogWarning("Cannot resize motd image, missing sprite");
				}
				this.motdImageButton.onClick.AddListener(delegate
				{
					Application.OpenURL(response.image_link_url);
				});
				return;
			}
			global::Debug.LogWarning("Motd Request error: " + error);
		});
		this.lastUpdateTime = Time.unscaledTime;
		this.activateOnSpawn = true;
	}

	public void RefreshMainMenu()
	{
		if (this.refreshResumeButton)
		{
			this.RefreshResumeButton();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		if (e.Consumed)
		{
			return;
		}
		KKeyCode kkeyCode;
		switch (this.m_cheatInputCounter)
		{
		case 0:
			kkeyCode = KKeyCode.K;
			break;
		case 1:
			kkeyCode = KKeyCode.L;
			break;
		case 2:
			kkeyCode = KKeyCode.E;
			break;
		case 3:
			kkeyCode = KKeyCode.I;
			break;
		case 4:
			kkeyCode = KKeyCode.P;
			break;
		case 5:
			kkeyCode = KKeyCode.L;
			break;
		case 6:
			kkeyCode = KKeyCode.A;
			break;
		default:
			kkeyCode = KKeyCode.Y;
			break;
		}
		if (e.Controller.GetKeyDown(kkeyCode))
		{
			e.Consumed = true;
			this.m_cheatInputCounter++;
			if (this.m_cheatInputCounter >= 8)
			{
				global::Debug.Log("Cheat Detected - enabling Debug Mode");
				DebugHandler.SetDebugEnabled(true);
				this.buildWatermark.RefreshText();
				this.m_cheatInputCounter = 0;
				return;
			}
		}
		else
		{
			this.m_cheatInputCounter = 0;
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
		global::Debug.Log("-- MAIN MENU -- ");
		base.OnSpawn();
		this.m_cheatInputCounter = 0;
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
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(text3, null, null, null, null, null, null, null, null, true);
		}
		Global.Instance.modManager.Report(base.gameObject);
		if ((GenericGameSettings.instance.autoResumeGame && !MainMenu.HasAutoresumedOnce) || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame))
		{
			MainMenu.HasAutoresumedOnce = true;
			this.ResumeGame();
		}
	}

	private void UnregisterMotdRequest()
	{
		if (this.m_motdServerClient != null)
		{
			this.m_motdServerClient.UnregisterCallback();
			this.m_motdServerClient = null;
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		this.UnregisterMotdRequest();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		this.refreshResumeButton = topLevel;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		this.UnregisterMotdRequest();
	}

	private void ShowLanguageConfirmation()
	{
		if (SteamManager.Initialized)
		{
			if (SteamUtils.GetSteamUILanguage() != "schinese")
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
		string text = (string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveFile() : GenericGameSettings.instance.performanceCapture.saveGame);
		if (!string.IsNullOrEmpty(text))
		{
			KCrashReporter.MOST_RECENT_SAVEFILE = text;
			SaveLoader.SetActiveSaveFilePath(text);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		base.GetComponent<NewGameFlow>().BeginFlow();
	}

	private void LoadGame()
	{
		if (LoadScreen.Instance == null)
		{
			LoadScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, true).GetComponent<LoadScreen>();
			component.requireConfirmation = false;
			component.SetBackgroundActive(true);
		}
		LoadScreen.Instance.gameObject.SetActive(true);
	}

	public static void ActivateRetiredColoniesScreen(GameObject parent, string colonyID = "")
	{
		if (RetiredColonyInfoScreen.Instance == null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
		}
		RetiredColonyInfoScreen.Instance.Show(true);
		if (!string.IsNullOrEmpty(colonyID))
		{
			if (SaveGame.Instance != null)
			{
				RetireColonyUtility.SaveColonySummaryData();
			}
			RetiredColonyInfoScreen.Instance.LoadColony(RetiredColonyInfoScreen.Instance.GetColonyDataByBaseName(colonyID));
		}
	}

	public static void ActivateRetiredColoniesScreenFromData(GameObject parent, RetiredColonyData data)
	{
		if (RetiredColonyInfoScreen.Instance == null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
		}
		RetiredColonyInfoScreen.Instance.Show(true);
		RetiredColonyInfoScreen.Instance.LoadColony(data);
	}

	private void SpawnVideoScreen()
	{
		VideoScreen.Instance = Util.KInstantiateUI(ScreenPrefabs.Instance.VideoScreen.gameObject, base.gameObject, false).GetComponent<VideoScreen>();
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
				if (header.buildVersion > 399090U || gameInfo.saveMajorVersion < 7)
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
				global::Debug.LogWarning(ex);
				flag = false;
			}
		}
		if (this.Button_ResumeGame != null && this.Button_ResumeGame.gameObject != null)
		{
			this.Button_ResumeGame.gameObject.SetActive(flag);
			return;
		}
		global::Debug.LogWarning("Why is the resume game button null?");
	}

	private void Translations()
	{
		Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject, false).SetBackgroundActive(true);
	}

	private void Mods()
	{
		Util.KInstantiateUI<ModsScreen>(ScreenPrefabs.Instance.modsMenu.gameObject, base.transform.parent.gameObject, false).SetBackgroundActive(true);
	}

	private void Options()
	{
		Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
	}

	private void QuitGame()
	{
		App.Quit();
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
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO, delegate
			{
				Application.OpenURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
			}, null, null, null, GlobalResources.Instance().sadDupeAudio, true);
		}
	}

	private void CheckPlayerPrefsCorruption()
	{
		if (KPlayerPrefs.HasCorruptedFlag())
		{
			KPlayerPrefs.ResetCorruptedFlag();
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.PLAYER_PREFS_CORRUPTED, null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe, true);
		}
	}

	private void CheckDoubleBoundKeys()
	{
		string text = "";
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
								if ((mGroup == "Root" || mGroup2 == "Root" || mGroup == mGroup2) && (!(mGroup == "Root") || !bindingEntry.mIgnoreRootConflics) && (!(mGroup2 == "Root") || !bindingEntry2.mIgnoreRootConflics))
								{
									text = string.Concat(new object[] { text, "\n\n", bindingEntry2.mAction, ": <b>", bindingEntry2.mKeyCode, "</b>\n", bindingEntry.mAction, ": <b>", bindingEntry.mKeyCode, "</b>" });
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
				hashSet.Add(GameInputMapping.KeyBindings[i]);
			}
		}
		if (text != "")
		{
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text), null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe, true);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}

	public RectTransform LogoAndMenu;

	public KButton Button_ResumeGame;

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private MotdServerClient m_motdServerClient;

	private GameObject GameSettingsScreen;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private LocText motdImageHeader;

	[SerializeField]
	private Button motdImageButton;

	[SerializeField]
	private Image motdImage;

	[SerializeField]
	private LocText motdNewsHeader;

	[SerializeField]
	private LocText motdNewsBody;

	[SerializeField]
	private PatchNotesScreen patchNotesScreen;

	[SerializeField]
	private NextUpdateTimer nextUpdateTimer;

	[SerializeField]
	private BuildWatermark buildWatermark;

	private static bool HasAutoresumedOnce = false;

	private bool refreshResumeButton = true;

	private int m_cheatInputCounter;

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
