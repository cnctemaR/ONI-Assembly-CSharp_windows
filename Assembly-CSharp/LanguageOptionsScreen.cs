using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Klei;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IUGCEventHandler
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText reference = this.dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.workshopButton.onClick += delegate
		{
			this.OnClickOpenWorkshop();
		};
		this.uninstallButton.onClick += delegate
		{
			this.OnClickUninstall();
		};
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
		foreach (string text in Localization.PreinstalledLanguages)
		{
			if (!(text != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(text)))
			{
				GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.preinstalledLanguagesContainer, false);
				gameObject.name = text + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				LocText reference = component.GetReference<LocText>("Title");
				reference.text = Localization.GetPreinstalledLocalizationTitle(text);
				reference.enabled = false;
				reference.enabled = true;
				Texture2D preinstalledLocalizationImage = Localization.GetPreinstalledLocalizationImage(text);
				if (preinstalledLocalizationImage != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(preinstalledLocalizationImage, new Rect(Vector2.zero, new Vector2((float)preinstalledLocalizationImage.width, (float)preinstalledLocalizationImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				string _code = text;
				component2.onClick += delegate
				{
					this.ActivatePreinstalledLanguage(_code);
				};
				this.buttons.Add(gameObject);
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.currentLanguage = LanguageOptionsScreen.GetInstalledFileID(out this.currentLastModified);
		if (SteamUGCService.Instance != null)
		{
			if (!SteamUGCService.Instance.IsSubscribedTo(this.currentLanguage))
			{
				this.currentLanguage = PublishedFileId_t.Invalid;
				this.InstallLanguageFile(this.currentLanguage, false);
			}
			SteamUGCService.Instance.ugcEventHandlers.Add(this);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Remove(this);
		}
	}

	private void ActivatePreinstalledLanguage(string code)
	{
		Localization.LoadPreinstalledTranslation(code);
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null, null);
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
		List<SteamUGCService.Subscribed> subs = SteamUGCService.Instance.GetSubscribed("language");
		if (subs.Count != 0)
		{
			for (int i = 0; i < subs.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.ugcLanguagesContainer, false);
				gameObject.name = subs[i].title + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLangage = LanguageOptionsScreen.GetFontForLangage(subs[i].fileId);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, subs[i].title));
				reference.font = fontForLangage;
				Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subs[i].fileId);
				if (previewImage != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int index = i;
				component2.onClick += delegate
				{
					PublishedFileId_t fileId = subs[index].fileId;
					this.SetCurrentLanguage(fileId);
				};
				this.buttons.Add(gameObject);
			}
		}
	}

	private void InstallLanguage(PublishedFileId_t item)
	{
		this.SetCurrentLanguage(item);
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null, null);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialog2 = this.GetConfirmDialog();
			confirmDialog2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
			{
				Application.Quit();
			}, delegate
			{
				App.LoadScene("frontend");
			}, null, null, null, null, null, null);
		}, delegate
		{
		}, null, null, null, null, null, null);
	}

	private void OnClickUninstall()
	{
		this.Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	private ulong GetCurrentLanguage()
	{
		return (ulong)((long)KPlayerPrefs.GetInt("InstalledLanguage"));
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		ulong currentLanguage = this.GetCurrentLanguage();
		if (currentLanguage == pCallback.m_nPublishedFileId.m_PublishedFileId)
		{
			global::Debug.Log("Update detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			SteamUGCService.DoDownloadItem(pCallback.m_nPublishedFileId);
		}
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		ulong currentLanguage = this.GetCurrentLanguage();
		if (pCallback.m_nPublishedFileId.m_PublishedFileId == currentLanguage)
		{
			global::Debug.Log("Unsubscribe detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			LanguageOptionsScreen.CleanUpCurrentModLanguage();
		}
	}

	public void OnUGCRefresh()
	{
		this.RebuildScreen();
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

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
		ulong currentLanguage = this.GetCurrentLanguage();
		if (currentLanguage == pCallback.m_nPublishedFileId.m_PublishedFileId)
		{
			global::Debug.Log("Download complete for currently installed font [" + pCallback.m_nPublishedFileId + "] updating in background. Changes will happen next restart.", null);
			this.UpdateInstalledLanguage(pCallback.m_nPublishedFileId);
		}
	}

	public PublishedFileId_t currentLanguage { get; private set; }

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
		string languageFile = LanguageOptionsScreen.GetLanguageFile(out installed, out dateTime);
		if (languageFile != null && File.Exists(languageFile))
		{
			string[] array = File.ReadAllLines(languageFile, Encoding.UTF8);
			Localization.Locale locale = Localization.GetLocale(array);
			if (locale != null)
			{
				text = locale.Code;
			}
		}
		return text;
	}

	public static string GetInstalledLanguageFile(ref PublishedFileId_t item)
	{
		global::System.DateTime dateTime;
		return LanguageOptionsScreen.GetLanguageFile(out item, out dateTime);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		global::System.DateTime dateTime;
		string languageFileFromSteam = LanguageOptionsScreen.GetLanguageFileFromSteam(item, out dateTime);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			string[] array = languageFileFromSteam.Split(new char[] { '\n' });
			string fontForLocalisation = LanguageOptionsScreen.GetFontForLocalisation(array);
			return Localization.GetFont(fontForLocalisation);
		}
		return null;
	}

	public static void LoadTranslation(ref PublishedFileId_t item)
	{
		string installedLanguageFile = LanguageOptionsScreen.GetInstalledLanguageFile(ref item);
		Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, installedLanguageFile);
	}

	private void UpdateInstalledLanguage(PublishedFileId_t item)
	{
		string languageFileFromSteam = LanguageOptionsScreen.GetLanguageFileFromSteam(item, out this.currentLastModified);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			LanguageOptionsScreen.InstalledLanguageData.Set(item, this.currentLastModified);
			File.WriteAllText(Localization.GetModLocalizationFilePath(), languageFileFromSteam);
			return;
		}
		global::Debug.LogWarning(string.Concat(new object[] { "Loc file was empty.. [", item, "]  [", this.currentLastModified, "]" }), null);
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

	private static string GetLanguageFile(out PublishedFileId_t item, out global::System.DateTime lastModified)
	{
		LanguageOptionsScreen.InstalledLanguageData.Get(out item, out lastModified);
		if (item != PublishedFileId_t.Invalid)
		{
			string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
			if (File.Exists(modLocalizationFilePath))
			{
				return modLocalizationFilePath;
			}
			global::Debug.LogWarning(string.Concat(new object[] { "GetLanguagFile [", modLocalizationFilePath, "] missing for [", item, "]" }), null);
		}
		return null;
	}

	private static string GetLanguageFileFromSteam(PublishedFileId_t item, out global::System.DateTime lastModified)
	{
		lastModified = global::System.DateTime.MinValue;
		if (item == PublishedFileId_t.Invalid)
		{
			global::Debug.LogWarning("Cant get INVALID file id from Steam", null);
			return null;
		}
		EItemState itemState = (EItemState)SteamUGC.GetItemState(item);
		if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled)
		{
			byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, LanguageOptionsScreen.poFile, out lastModified, false);
			if (bytesFromZip != null && bytesFromZip.Length > 0)
			{
				return Encoding.UTF8.GetString(bytesFromZip);
			}
			global::Debug.LogWarning("Empty bytes from Zip file, trying redownload", null);
			SteamUGCService.DoDownloadItem(item);
		}
		else
		{
			global::Debug.LogWarning("Steam says item not installed [" + itemState + "]", null);
		}
		return null;
	}

	private static PublishedFileId_t GetInstalledFileID(out global::System.DateTime lastModified)
	{
		PublishedFileId_t publishedFileId_t;
		LanguageOptionsScreen.InstalledLanguageData.Get(out publishedFileId_t, out lastModified);
		if (publishedFileId_t == PublishedFileId_t.Invalid)
		{
			publishedFileId_t = new PublishedFileId_t((ulong)KPlayerPrefs.GetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId));
			if (publishedFileId_t != PublishedFileId_t.Invalid)
			{
				if (SteamUGCService.Instance != null)
				{
					if (!SteamUGCService.Instance.IsSubscribedTo(publishedFileId_t))
					{
						global::Debug.LogWarning("It doesn't look like we are subscribed..." + publishedFileId_t, null);
					}
				}
				else
				{
					global::Debug.LogWarning("Cant check yet..." + publishedFileId_t, null);
				}
			}
		}
		return publishedFileId_t;
	}

	private static readonly string[] poFile = new string[] { "strings.po" };

	private const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";

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

	private global::System.DateTime currentLastModified;

	private class InstalledLanguageData : YamlIO<LanguageOptionsScreen.InstalledLanguageData>
	{
		private static string FilePath()
		{
			return Path.Combine(Application.streamingAssetsPath, LanguageOptionsScreen.InstalledLanguageData.FILE_NAME);
		}

		public static void Set(PublishedFileId_t item, global::System.DateTime lastModified)
		{
			new LanguageOptionsScreen.InstalledLanguageData
			{
				PublishedFileId = item.m_PublishedFileId,
				LastModified = lastModified.ToFileTimeUtc()
			}.Save(LanguageOptionsScreen.InstalledLanguageData.FilePath(), null);
		}

		public static void Get(out PublishedFileId_t item, out global::System.DateTime lastModified)
		{
			if (LanguageOptionsScreen.InstalledLanguageData.Exists())
			{
				LanguageOptionsScreen.InstalledLanguageData installedLanguageData = YamlIO<LanguageOptionsScreen.InstalledLanguageData>.LoadFile(LanguageOptionsScreen.InstalledLanguageData.FilePath(), null);
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

		private static readonly string FILE_NAME = "Mods/mod_installed.dat";
	}
}
