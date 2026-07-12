using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KScreen
{
	public static MainMenu Instance
	{
		get
		{
			return MainMenu._instance;
		}
	}

	private KButton MakeButton(MainMenu.ButtonInfo info)
	{
		KButton kbutton = global::Util.KInstantiateUI<KButton>(this.buttonPrefab.gameObject, this.buttonParent, true);
		kbutton.onClick += info.action;
		KImage component = kbutton.GetComponent<KImage>();
		component.colorStyleSetting = info.style;
		component.ApplyColorStyleSetting();
		LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
		componentInChildren.text = info.text;
		componentInChildren.fontSize = (float)info.fontSize;
		return kbutton;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MainMenu._instance = this;
		this.Button_NewGame = this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, new global::System.Action(this.NewGame), 22, this.topButtonStyle));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, new global::System.Action(this.LoadGame), 22, this.normalButtonStyle));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.RETIREDCOLONIES, delegate
		{
			MainMenu.ActivateRetiredColoniesScreen(this.transform.gameObject, "");
		}, 14, this.normalButtonStyle));
		this.lockerButton = this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.LOCKERMENU, delegate
		{
			MainMenu.ActivateLockerMenu();
		}, 14, this.normalButtonStyle));
		if (DistributionPlatform.Initialized)
		{
			this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, new global::System.Action(this.Translations), 14, this.normalButtonStyle));
			this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MODS.TITLE, new global::System.Action(this.Mods), 14, this.normalButtonStyle));
		}
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, new global::System.Action(this.Options), 14, this.normalButtonStyle));
		this.MakeButton(new MainMenu.ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, new global::System.Action(this.QuitGame), 14, this.normalButtonStyle));
		this.RefreshResumeButton(false);
		this.Button_ResumeGame.onClick += this.ResumeGame;
		this.SpawnVideoScreen();
		this.StartFEAudio();
		this.CheckPlayerPrefsCorruption();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			global::Util.KInstantiateUI(this.patchNotesScreenPrefab.gameObject, FrontEndManager.Instance.gameObject, true);
		}
		this.CheckDoubleBoundKeys();
		this.topLeftAlphaMessage.gameObject.SetActive(false);
		this.MOTDContainer.SetActive(false);
		this.buttonContainer.SetActive(false);
		this.nextUpdateTimer.gameObject.SetActive(true);
		bool flag = DistributionPlatform.Inst.IsDLCPurchased("EXPANSION1_ID");
		this.expansion1Toggle.gameObject.SetActive(flag);
		if (this.expansion1Ad != null)
		{
			this.expansion1Ad.gameObject.SetActive(!flag);
		}
		this.m_motdServerClient = new MotdServerClient();
		this.m_motdServerClient.GetMotd(delegate(MotdServerClient.MotdResponse response, string error)
		{
			if (error == null)
			{
				if (DlcManager.IsExpansion1Active())
				{
					this.nextUpdateTimer.UpdateReleaseTimes(response.expansion1_update_data.last_update_time, response.expansion1_update_data.next_update_time, response.expansion1_update_data.update_text_override);
				}
				else
				{
					this.nextUpdateTimer.UpdateReleaseTimes(response.vanilla_update_data.last_update_time, response.vanilla_update_data.next_update_time, response.vanilla_update_data.update_text_override);
				}
				this.topLeftAlphaMessage.gameObject.SetActive(true);
				this.MOTDContainer.SetActive(true);
				this.buttonContainer.SetActive(true);
				this.motdImageHeader.text = response.image_header_text;
				this.motdNewsHeader.text = response.news_header_text;
				this.motdNewsBody.text = response.news_body_text;
				PatchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
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
				this.motdImageButton.ClearOnClick();
				this.motdImageButton.onClick += delegate
				{
					App.OpenWebURL(response.image_link_url);
				};
				return;
			}
			global::Debug.LogWarning("Motd Request error: " + error);
		});
		if (DistributionPlatform.Initialized && DistributionPlatform.Inst.IsPreviousVersionBranch)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(ScreenPrefabs.Instance.OldVersionWarningScreen, this.uiCanvas.transform);
		}
		string targetExpansion1AdURL = "";
		Sprite sprite = Assets.GetSprite("expansionPromo_en");
		if (DistributionPlatform.Initialized && this.expansion1Ad != null)
		{
			string name = DistributionPlatform.Inst.Name;
			if (name != null)
			{
				if (!(name == "Steam"))
				{
					if (!(name == "Epic"))
					{
						if (name == "Rail")
						{
							targetExpansion1AdURL = "https://www.wegame.com.cn/store/2001539/";
							sprite = Assets.GetSprite("expansionPromo_cn");
						}
					}
					else
					{
						targetExpansion1AdURL = "https://store.epicgames.com/en-US/p/oxygen-not-included--spaced-out";
					}
				}
				else
				{
					targetExpansion1AdURL = "https://store.steampowered.com/app/1452490/Oxygen_Not_Included__Spaced_Out/";
				}
			}
			this.expansion1Ad.GetComponentInChildren<KButton>().onClick += delegate
			{
				App.OpenWebURL(targetExpansion1AdURL);
			};
			this.expansion1Ad.GetComponent<HierarchyReferences>().GetReference<Image>("Image").sprite = sprite;
		}
		this.activateOnSpawn = true;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			this.RefreshResumeButton(false);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		if (e.Consumed)
		{
			return;
		}
		if (e.TryConsume(global::Action.DebugToggleUI))
		{
			this.m_screenshotMode = !this.m_screenshotMode;
			this.uiCanvas.alpha = (this.m_screenshotMode ? 0f : 1f);
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
		this.InitLoadScreen();
		LoadScreen.Instance.ShowMigrationIfNecessary(true);
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
			global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(text3, null, null, null, null, null, null, null, null);
		}
		Global.Instance.modManager.Report(base.gameObject);
		if ((GenericGameSettings.instance.autoResumeGame && !MainMenu.HasAutoresumedOnce && !KCrashReporter.hasCrash) || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) || KPlayerPrefs.HasKey("AutoResumeSaveFile"))
		{
			MainMenu.HasAutoresumedOnce = true;
			this.ResumeGame();
		}
		if (GenericGameSettings.instance.devAutoWorldGen && !KCrashReporter.hasCrash)
		{
			GenericGameSettings.instance.devAutoWorldGen = false;
			GenericGameSettings.instance.devAutoWorldGenActive = true;
			GenericGameSettings.instance.SaveSettings();
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.WorldGenScreen.gameObject, base.gameObject, true);
		}
		this.RefreshInventoryNotification();
	}

	protected override void OnForcedCleanUp()
	{
		base.OnForcedCleanUp();
	}

	private void RefreshInventoryNotification()
	{
		bool flag = PermitItems.HasUnopenedItem();
		this.lockerButton.GetComponent<HierarchyReferences>().GetReference<RectTransform>("AttentionIcon").gameObject.SetActive(flag);
	}

	private void UnregisterMotdRequest()
	{
		if (this.m_motdServerClient != null)
		{
			this.m_motdServerClient.UnregisterCallback();
			this.m_motdServerClient = null;
		}
	}

	protected override void OnActivate()
	{
		if (!this.ambientLoopEventName.IsNullOrWhiteSpace())
		{
			this.ambientLoop = KFMOD.CreateInstance(GlobalAssets.GetSound(this.ambientLoopEventName, false));
			if (this.ambientLoop.isValid())
			{
				this.ambientLoop.start();
			}
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
		if (KleiItemDropScreen.Instance != null && KleiItemDropScreen.Instance.gameObject.activeInHierarchy != this.itemDropOpenFlag)
		{
			this.RefreshInventoryNotification();
			this.itemDropOpenFlag = KleiItemDropScreen.Instance.gameObject.activeInHierarchy;
		}
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		this.StopAmbience();
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
		string text;
		if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
		{
			text = KPlayerPrefs.GetString("AutoResumeSaveFile");
			KPlayerPrefs.DeleteKey("AutoResumeSaveFile");
		}
		else if (!string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame))
		{
			text = GenericGameSettings.instance.performanceCapture.saveGame;
		}
		else
		{
			text = SaveLoader.GetLatestSaveForCurrentDLC();
		}
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

	private void InitLoadScreen()
	{
		if (LoadScreen.Instance == null)
		{
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, true).GetComponent<LoadScreen>();
		}
	}

	private void LoadGame()
	{
		this.InitLoadScreen();
		LoadScreen.Instance.Activate();
	}

	public static void ActivateRetiredColoniesScreen(GameObject parent, string colonyID = "")
	{
		if (RetiredColonyInfoScreen.Instance == null)
		{
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
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
			global::Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
		}
		RetiredColonyInfoScreen.Instance.Show(true);
		RetiredColonyInfoScreen.Instance.LoadColony(data);
	}

	public static void ActivateInventoyScreen()
	{
		LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen, null);
	}

	public static void ActivateLockerMenu()
	{
		LockerMenuScreen.Instance.Show(true);
	}

	private void SpawnVideoScreen()
	{
		VideoScreen.Instance = global::Util.KInstantiateUI(ScreenPrefabs.Instance.VideoScreen.gameObject, base.gameObject, false).GetComponent<VideoScreen>();
	}

	private void Update()
	{
	}

	public void RefreshResumeButton(bool simpleCheck = false)
	{
		string latestSaveForCurrentDLC = SaveLoader.GetLatestSaveForCurrentDLC();
		bool flag = !string.IsNullOrEmpty(latestSaveForCurrentDLC) && File.Exists(latestSaveForCurrentDLC);
		if (flag)
		{
			try
			{
				if (GenericGameSettings.instance.demoMode)
				{
					flag = false;
				}
				global::System.DateTime lastWriteTime = File.GetLastWriteTime(latestSaveForCurrentDLC);
				MainMenu.SaveFileEntry saveFileEntry = default(MainMenu.SaveFileEntry);
				SaveGame.Header header = default(SaveGame.Header);
				SaveGame.GameInfo gameInfo = default(SaveGame.GameInfo);
				if (!this.saveFileEntries.TryGetValue(latestSaveForCurrentDLC, out saveFileEntry) || saveFileEntry.timeStamp != lastWriteTime)
				{
					gameInfo = SaveLoader.LoadHeader(latestSaveForCurrentDLC, out header);
					saveFileEntry = new MainMenu.SaveFileEntry
					{
						timeStamp = lastWriteTime,
						header = header,
						headerData = gameInfo
					};
					this.saveFileEntries[latestSaveForCurrentDLC] = saveFileEntry;
				}
				else
				{
					header = saveFileEntry.header;
					gameInfo = saveFileEntry.headerData;
				}
				if (header.buildVersion > 562984U || gameInfo.saveMajorVersion != 7 || gameInfo.saveMinorVersion > 32)
				{
					flag = false;
				}
				if (!DlcManager.IsContentActive(gameInfo.dlcId))
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveForCurrentDLC);
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
			KImage component = this.Button_NewGame.GetComponent<KImage>();
			component.colorStyleSetting = (flag ? this.normalButtonStyle : this.topButtonStyle);
			component.ApplyColorStyleSetting();
			return;
		}
		global::Debug.LogWarning("Why is the resume game button null?");
	}

	private void Translations()
	{
		global::Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject, false);
	}

	private void Mods()
	{
		global::Util.KInstantiateUI<ModsScreen>(ScreenPrefabs.Instance.modsMenu.gameObject, base.transform.parent.gameObject, false);
	}

	private void Options()
	{
		global::Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
	}

	private void QuitGame()
	{
		App.Quit();
	}

	public void StartFEAudio()
	{
		AudioMixer.instance.Reset();
		MusicManager.instance.KillAllSongs(STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
		{
			AudioMixer.instance.StartUserVolumesSnapshot();
		}
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying(this.menuMusicEventName))
		{
			MusicManager.instance.PlaySong(this.menuMusicEventName, false);
		}
		this.CheckForAudioDriverIssue();
	}

	public void StopAmbience()
	{
		if (this.ambientLoop.isValid())
		{
			this.ambientLoop.stop(STOP_MODE.ALLOWFADEOUT);
			this.ambientLoop.release();
			this.ambientLoop.clearHandle();
		}
	}

	public void StopMainMenuMusic()
	{
		if (MusicManager.instance.SongIsPlaying(this.menuMusicEventName))
		{
			MusicManager.instance.StopSong(this.menuMusicEventName, true, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
	}

	private void CheckForAudioDriverIssue()
	{
		if (!KFMOD.didFmodInitializeSuccessfully)
		{
			global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO, delegate
			{
				App.OpenWebURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
			}, null, null, null, GlobalResources.Instance().sadDupeAudio);
		}
	}

	private void CheckPlayerPrefsCorruption()
	{
		if (KPlayerPrefs.HasCorruptedFlag())
		{
			KPlayerPrefs.ResetCorruptedFlag();
			global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.PLAYER_PREFS_CORRUPTED, null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe);
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
									text = string.Concat(new string[]
									{
										text,
										"\n\n",
										bindingEntry2.mAction.ToString(),
										": <b>",
										bindingEntry2.mKeyCode.ToString(),
										"</b>\n",
										bindingEntry.mAction.ToString(),
										": <b>",
										bindingEntry.mKeyCode.ToString(),
										"</b>"
									});
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
			global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true).PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text), null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}

	private static MainMenu _instance;

	public KButton Button_ResumeGame;

	private KButton Button_NewGame;

	public GameObject topLeftAlphaMessage;

	private MotdServerClient m_motdServerClient;

	private GameObject GameSettingsScreen;

	private bool m_screenshotMode;

	[SerializeField]
	private CanvasGroup uiCanvas;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private ColorStyleSetting topButtonStyle;

	[SerializeField]
	private ColorStyleSetting normalButtonStyle;

	[SerializeField]
	private string menuMusicEventName;

	[SerializeField]
	private string ambientLoopEventName;

	private EventInstance ambientLoop;

	[SerializeField]
	private GameObject MOTDContainer;

	[SerializeField]
	private GameObject buttonContainer;

	[SerializeField]
	private LocText motdImageHeader;

	[SerializeField]
	private KButton motdImageButton;

	[SerializeField]
	private Image motdImage;

	[SerializeField]
	private LocText motdNewsHeader;

	[SerializeField]
	private LocText motdNewsBody;

	[SerializeField]
	private PatchNotesScreen patchNotesScreenPrefab;

	[SerializeField]
	private NextUpdateTimer nextUpdateTimer;

	[SerializeField]
	private DLCToggle expansion1Toggle;

	[SerializeField]
	private GameObject expansion1Ad;

	[SerializeField]
	private BuildWatermark buildWatermark;

	[SerializeField]
	public string IntroShortName;

	private KButton lockerButton;

	private bool itemDropOpenFlag;

	private static bool HasAutoresumedOnce = false;

	private bool refreshResumeButton = true;

	private int m_cheatInputCounter;

	public const string AutoResumeSaveFileKey = "AutoResumeSaveFile";

	public const string PLAY_SHORT_ON_LAUNCH = "PlayShortOnLaunch";

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;

	private Dictionary<string, MainMenu.SaveFileEntry> saveFileEntries = new Dictionary<string, MainMenu.SaveFileEntry>();

	private struct ButtonInfo
	{
		public ButtonInfo(LocString text, global::System.Action action, int font_size, ColorStyleSetting style)
		{
			this.text = text;
			this.action = action;
			this.fontSize = font_size;
			this.style = style;
		}

		public LocString text;

		public global::System.Action action;

		public int fontSize;

		public ColorStyleSetting style;
	}

	private struct SaveFileEntry
	{
		public global::System.DateTime timeStamp;

		public SaveGame.Header header;

		public SaveGame.GameInfo headerData;
	}
}
