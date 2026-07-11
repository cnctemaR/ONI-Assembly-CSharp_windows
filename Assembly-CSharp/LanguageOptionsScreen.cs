using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Klei;
using KMod;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IClient
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.dismissButton.onClick += this.Deactivate;
		LocText reference = this.dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		this.closeButton.onClick += this.Deactivate;
		this.workshopButton.onClick += delegate
		{
			this.OnClickOpenWorkshop();
		};
		this.uninstallButton.onClick += delegate
		{
			this.OnClickUninstall();
		};
		this.uninstallButton.gameObject.SetActive(false);
		this.RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject gameObject in this.buttons)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.buttons.Clear();
		this.uninstallButton.isInteractable = KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()) != Localization.SelectedLanguageType.None.ToString();
		this.RebuildPreinstalledButtons();
		this.RebuildUGCButtons();
	}

	private void RebuildPreinstalledButtons()
	{
		using (List<string>.Enumerator enumerator = Localization.PreinstalledLanguages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string code = enumerator.Current;
				LanguageOptionsScreen $this = this;
				if (!(code != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(code)))
				{
					GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.preinstalledLanguagesContainer, false);
					gameObject.name = code + "_button";
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					LocText reference = component.GetReference<LocText>("Title");
					reference.text = Localization.GetPreinstalledLocalizationTitle(code);
					reference.enabled = false;
					reference.enabled = true;
					Texture2D preinstalledLocalizationImage = Localization.GetPreinstalledLocalizationImage(code);
					if (preinstalledLocalizationImage != null)
					{
						Image reference2 = component.GetReference<Image>("Image");
						reference2.sprite = Sprite.Create(preinstalledLocalizationImage, new Rect(Vector2.zero, new Vector2((float)preinstalledLocalizationImage.width, (float)preinstalledLocalizationImage.height)), Vector2.one * 0.5f);
					}
					KButton component2 = gameObject.GetComponent<KButton>();
					component2.onClick += delegate
					{
						$this.ConfirmLanguageChoiceDialog((!(code != Localization.DEFAULT_LANGUAGE_CODE)) ? string.Empty : code, PublishedFileId_t.Invalid);
					};
					this.buttons.Add(gameObject);
				}
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Global.Instance.modManager.Sanitize(base.gameObject);
		this.currentLanguage = LanguageOptionsScreen.GetInstalledFileID(out this.currentLastModified);
		if (SteamUGCService.Instance != null)
		{
			if (SteamUGCService.Instance.FindMod(this.currentLanguage) == null)
			{
				this.currentLanguage = PublishedFileId_t.Invalid;
				this.InstallLanguageFile(this.currentLanguage, false);
			}
			SteamUGCService.Instance.AddClient(this);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.RemoveClient(this);
		}
	}

	private void ConfirmLanguageChoiceDialog(string[] lines, bool is_template, global::System.Action install_language)
	{
		Localization.Locale locale = Localization.GetLocale(lines);
		Dictionary<string, string> translated_strings = Localization.ExtractTranslatedStrings(lines, is_template);
		TMP_FontAsset font = Localization.GetFont(locale.FontName);
		ConfirmDialogScreen screen = this.GetConfirmDialog();
		HashSet<MemberInfo> excluded_members = new HashSet<MemberInfo>(typeof(ConfirmDialogScreen).GetMember("cancelButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
		Localization.SetFont<ConfirmDialogScreen>(screen, font, locale.IsRightToLeft, excluded_members);
		Func<LocString, string> func = delegate(LocString loc_string)
		{
			string text4;
			return (!translated_strings.TryGetValue(loc_string.key.String, out text4)) ? loc_string : text4;
		};
		ConfirmDialogScreen screen2 = screen;
		string text = func(UI.CONFIRMDIALOG.DIALOG_HEADER);
		string text2 = func(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT);
		string text3 = UI.FRONTEND.TRANSLATIONS_SCREEN.CANCEL;
		screen2.PopupConfirmDialog(text2, delegate
		{
			install_language();
			App.instance.Restart();
		}, delegate
		{
			Localization.SetFont<ConfirmDialogScreen>(screen, Localization.FontAsset, Localization.IsRightToLeft, excluded_members);
		}, null, null, text, func(UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART), text3, null, true);
	}

	private void ConfirmLanguageChoiceDialog(string selected_preinstalled_translation)
	{
		Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
		if (!string.IsNullOrEmpty(selected_preinstalled_translation))
		{
			string selectedPreinstalledLanguageCode = Localization.GetSelectedPreinstalledLanguageCode();
			if (selectedLanguageType == Localization.SelectedLanguageType.Preinstalled && selectedPreinstalledLanguageCode == selected_preinstalled_translation)
			{
				this.Deactivate();
				return;
			}
			string preinstalledLocalizationFilePath = Localization.GetPreinstalledLocalizationFilePath(selected_preinstalled_translation);
			string[] array = File.ReadAllLines(preinstalledLocalizationFilePath, Encoding.UTF8);
			this.ConfirmLanguageChoiceDialog(array, false, delegate
			{
				Localization.LoadPreinstalledTranslation(selected_preinstalled_translation);
			});
		}
		else
		{
			if (selectedLanguageType == Localization.SelectedLanguageType.None)
			{
				this.Deactivate();
				return;
			}
			string defaultLocalizationFilePath = Localization.GetDefaultLocalizationFilePath();
			string[] array2 = File.ReadAllLines(defaultLocalizationFilePath, Encoding.UTF8);
			this.ConfirmLanguageChoiceDialog(array2, true, delegate
			{
				Localization.ClearLanguage();
			});
		}
	}

	private void ConfirmLanguageChoiceDialog(string selected_preinstalled_translation, PublishedFileId_t selected_language_pack)
	{
		if (selected_language_pack != PublishedFileId_t.Invalid)
		{
			Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
			if (selectedLanguageType == Localization.SelectedLanguageType.UGC && selected_language_pack == this.currentLanguage)
			{
				this.Deactivate();
				return;
			}
			global::System.DateTime dateTime;
			string languageFile = LanguageOptionsScreen.GetLanguageFile(selected_language_pack, out dateTime);
			string[] array = languageFile.Split(new char[] { '\n' });
			this.ConfirmLanguageChoiceDialog(array, false, delegate
			{
				this.SetCurrentLanguage(selected_language_pack);
			});
		}
		else
		{
			this.ConfirmLanguageChoiceDialog(selected_preinstalled_translation);
		}
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		GameObject gameObject = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void RebuildUGCButtons()
	{
		if (SteamUGCService.Instance == null)
		{
			return;
		}
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if ((byte)(mod.available_content & Content.Translation) != 0 && mod.status == Mod.Status.Installed)
			{
				GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.ugcLanguagesContainer, false);
				gameObject.name = mod.title + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				PublishedFileId_t file_id = new PublishedFileId_t(ulong.Parse(mod.label.id));
				TMP_FontAsset fontForLangage = LanguageOptionsScreen.GetFontForLangage(file_id);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, mod.title));
				reference.font = fontForLangage;
				SteamUGCService.Mod mod2 = SteamUGCService.Instance.FindMod(file_id);
				Texture2D texture2D = ((mod2 == null) ? null : mod2.previewImage);
				if (texture2D != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				component2.onClick += delegate
				{
					this.ConfirmLanguageChoiceDialog(string.Empty, file_id);
				};
				this.buttons.Add(gameObject);
			}
		}
	}

	private void InstallLanguage(PublishedFileId_t item)
	{
		this.SetCurrentLanguage(item);
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, new global::System.Action(App.instance.Restart), new global::System.Action(this.Deactivate), null, null, null, null, null, null, true);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialog2 = this.GetConfirmDialog();
			confirmDialog2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, new global::System.Action(App.instance.Restart), new global::System.Action(this.Deactivate), null, null, null, null, null, null, true);
		}, delegate
		{
		}, null, null, null, null, null, null, true);
	}

	private void OnClickUninstall()
	{
		this.Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
	{
		PublishedFileId_t publishedFileId_t = (PublishedFileId_t)this.GetCurrentLanguage();
		if (removed.Contains(publishedFileId_t))
		{
			global::Debug.Log("Unsubscribe detected for currently installed font [" + publishedFileId_t + "]");
			ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
			string text = UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT;
			string text2 = UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART;
			confirmDialog.PopupConfirmDialog(text, delegate
			{
				Localization.ClearLanguage();
				this.currentLanguage = PublishedFileId_t.Invalid;
				App.instance.Restart();
			}, null, null, null, null, text2, null, null, true);
		}
		if (updated.Contains(publishedFileId_t))
		{
			global::Debug.Log("Download complete for currently installed font [" + publishedFileId_t + "] updating in background. Changes will happen next restart.");
			this.UpdateInstalledLanguage(publishedFileId_t);
		}
		this.RebuildScreen();
	}

	private ulong GetCurrentLanguage()
	{
		return (ulong)((long)KPlayerPrefs.GetInt("InstalledLanguage"));
	}

	public static void CleanUpCurrentModLanguage()
	{
		KPlayerPrefs.SetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId);
		LanguageOptionsScreen.InstalledLanguageData.Delete();
		string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
		if (File.Exists(modLocalizationFilePath))
		{
			File.Delete(modLocalizationFilePath);
		}
	}

	public PublishedFileId_t currentLanguage
	{
		get
		{
			return this._currentLanguage;
		}
		private set
		{
			this._currentLanguage = value;
			KPlayerPrefs.SetInt("InstalledLanguage", (int)this._currentLanguage.m_PublishedFileId);
		}
	}

	public void SetCurrentLanguage(PublishedFileId_t item)
	{
		this.InstallLanguageFile(item, false);
	}

	public static bool HasInstalledLanguage()
	{
		global::System.DateTime dateTime;
		return LanguageOptionsScreen.GetInstalledFileID(out dateTime) != PublishedFileId_t.Invalid;
	}

	public static string GetInstalledLanguageCode(out PublishedFileId_t installed)
	{
		string text = string.Empty;
		global::System.DateTime dateTime;
		string languageFilename = LanguageOptionsScreen.GetLanguageFilename(out installed, out dateTime);
		if (languageFilename != null && File.Exists(languageFilename))
		{
			string[] array = File.ReadAllLines(languageFilename, Encoding.UTF8);
			Localization.Locale locale = Localization.GetLocale(array);
			if (locale != null)
			{
				text = locale.Code;
			}
		}
		return text;
	}

	public static string GetInstalledLanguageFilename(ref PublishedFileId_t item)
	{
		global::System.DateTime dateTime;
		return LanguageOptionsScreen.GetLanguageFilename(out item, out dateTime);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		global::System.DateTime dateTime;
		string languageFile = LanguageOptionsScreen.GetLanguageFile(item, out dateTime);
		if (languageFile != null && languageFile.Length > 0)
		{
			string[] array = languageFile.Split(new char[] { '\n' });
			string fontForLocalisation = LanguageOptionsScreen.GetFontForLocalisation(array);
			return Localization.GetFont(fontForLocalisation);
		}
		return null;
	}

	public static void LoadTranslation(ref PublishedFileId_t item)
	{
		string installedLanguageFilename = LanguageOptionsScreen.GetInstalledLanguageFilename(ref item);
		Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, installedLanguageFilename);
	}

	private void UpdateInstalledLanguage(PublishedFileId_t item)
	{
		string languageFile = LanguageOptionsScreen.GetLanguageFile(item, out this.currentLastModified);
		if (languageFile != null && languageFile.Length > 0)
		{
			LanguageOptionsScreen.InstalledLanguageData.Set(item, this.currentLastModified);
			File.WriteAllText(Localization.GetModLocalizationFilePath(), languageFile);
			return;
		}
		global::Debug.LogWarning(string.Concat(new object[] { "Loc file was empty.. [", item, "]  [", this.currentLastModified, "]" }));
	}

	private void InstallLanguageFile(PublishedFileId_t item, bool fromDownload = false)
	{
		LanguageOptionsScreen.CleanUpCurrentModLanguage();
		if (item != PublishedFileId_t.Invalid)
		{
			this.UpdateInstalledLanguage(item);
		}
		PublishedFileId_t invalid = PublishedFileId_t.Invalid;
		LanguageOptionsScreen.LoadTranslation(ref invalid);
		this.currentLanguage = item;
	}

	private static string GetFontForLocalisation(string[] lines)
	{
		return Localization.GetLocale(lines).FontName;
	}

	private static string GetLanguageFilename(out PublishedFileId_t item, out global::System.DateTime lastModified)
	{
		LanguageOptionsScreen.InstalledLanguageData.Get(out item, out lastModified);
		if (item != PublishedFileId_t.Invalid)
		{
			string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
			if (File.Exists(modLocalizationFilePath))
			{
				return modLocalizationFilePath;
			}
			global::Debug.LogWarning(string.Concat(new object[] { "GetLanguagFile [", modLocalizationFilePath, "] missing for [", item, "]" }));
		}
		return null;
	}

	private static string GetLanguageFile(PublishedFileId_t item, out global::System.DateTime lastModified)
	{
		lastModified = global::System.DateTime.MinValue;
		if (Global.Instance == null || Global.Instance.modManager == null)
		{
			global::Debug.LogFormat("Failed to load language file from local mod installation...too early in initialization flow.", new object[0]);
			return LanguageOptionsScreen.GetLanguageFileFromSteam(item, out lastModified);
		}
		string language_id = item.ToString();
		Mod mod = Global.Instance.modManager.mods.Find((Mod candidate) => candidate.label.id == language_id);
		if (string.IsNullOrEmpty(mod.label.id))
		{
			global::Debug.LogFormat("Failed to load language file from local mod installation...mod not found.", new object[0]);
			return LanguageOptionsScreen.GetLanguageFileFromSteam(item, out lastModified);
		}
		lastModified = mod.label.time_stamp;
		string text = Path.Combine(Application.streamingAssetsPath, "strings.po");
		byte[] array = mod.file_source.GetFileSystem().ReadBytes(text);
		if (array == null)
		{
			global::Debug.LogFormat("Failed to load language file from local mod installation...couldn't find {0}", new object[] { text });
			return LanguageOptionsScreen.GetLanguageFileFromSteam(item, out lastModified);
		}
		return FileSystem.ConvertToText(array);
	}

	private static string GetLanguageFileFromSteam(PublishedFileId_t item, out global::System.DateTime lastModified)
	{
		lastModified = global::System.DateTime.MinValue;
		if (item == PublishedFileId_t.Invalid)
		{
			global::Debug.LogWarning("Cant get INVALID file id from Steam");
			return null;
		}
		if (SteamUGCService.Instance.FindMod(item) == null)
		{
			global::Debug.LogWarning("Mod is not in published list");
			return null;
		}
		byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, LanguageOptionsScreen.poFile, out lastModified, false);
		if (bytesFromZip == null || bytesFromZip.Length == 0)
		{
			global::Debug.LogWarning("Failed to read from Steam mod installation");
			return null;
		}
		return Encoding.UTF8.GetString(bytesFromZip);
	}

	private static PublishedFileId_t GetInstalledFileID(out global::System.DateTime lastModified)
	{
		PublishedFileId_t invalid;
		LanguageOptionsScreen.InstalledLanguageData.Get(out invalid, out lastModified);
		if (invalid == PublishedFileId_t.Invalid)
		{
			invalid = new PublishedFileId_t((ulong)KPlayerPrefs.GetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId));
		}
		if (invalid != PublishedFileId_t.Invalid && SteamUGCService.Instance != null && !SteamUGCService.Instance.IsSubscribed(invalid))
		{
			global::Debug.LogWarning("It doesn't look like we are subscribed..." + invalid);
			invalid = PublishedFileId_t.Invalid;
		}
		return invalid;
	}

	private static readonly string[] poFile = new string[] { "strings.po" };

	public const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";

	public const string TAG_LANGUAGE = "language";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton uninstallButton;

	[Space]
	public GameObject languageButtonPrefab;

	public GameObject preinstalledLanguagesTitle;

	public GameObject preinstalledLanguagesContainer;

	public GameObject ugcLanguagesTitle;

	public GameObject ugcLanguagesContainer;

	private List<GameObject> buttons = new List<GameObject>();

	private PublishedFileId_t _currentLanguage = PublishedFileId_t.Invalid;

	private global::System.DateTime currentLastModified;

	private class InstalledLanguageData
	{
		private static string FilePath()
		{
			return Path.Combine(Application.streamingAssetsPath, LanguageOptionsScreen.InstalledLanguageData.FILE_NAME);
		}

		public static void Set(PublishedFileId_t item, global::System.DateTime lastModified)
		{
			YamlIO.SaveOrWarnUser<LanguageOptionsScreen.InstalledLanguageData>(new LanguageOptionsScreen.InstalledLanguageData
			{
				PublishedFileId = item.m_PublishedFileId,
				LastModified = lastModified.ToFileTimeUtc()
			}, LanguageOptionsScreen.InstalledLanguageData.FilePath(), null);
		}

		public static void Get(out PublishedFileId_t item, out global::System.DateTime lastModified)
		{
			if (LanguageOptionsScreen.InstalledLanguageData.Exists())
			{
				LanguageOptionsScreen.InstalledLanguageData installedLanguageData = YamlIO.LoadFile<LanguageOptionsScreen.InstalledLanguageData>(LanguageOptionsScreen.InstalledLanguageData.FilePath(), null, null);
				if (installedLanguageData != null)
				{
					lastModified = global::System.DateTime.FromFileTimeUtc(installedLanguageData.LastModified);
					item = new PublishedFileId_t(installedLanguageData.PublishedFileId);
					return;
				}
			}
			lastModified = global::System.DateTime.MinValue;
			item = PublishedFileId_t.Invalid;
		}

		public static bool Exists()
		{
			return File.Exists(LanguageOptionsScreen.InstalledLanguageData.FilePath());
		}

		public static void Delete()
		{
			if (LanguageOptionsScreen.InstalledLanguageData.Exists())
			{
				File.Delete(LanguageOptionsScreen.InstalledLanguageData.FilePath());
			}
		}

		public ulong PublishedFileId { get; set; }

		public long LastModified { get; set; }

		private static readonly string FILE_NAME = "strings/mod_installed.dat";
	}
}
