using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Klei;
using Steamworks;
using TMPro;
using UnityEngine;

public class SteamUGCService : MonoBehaviour
{
	public uint numSubscriptions { get; private set; }

	public PublishedFileId_t currentLanguage { get; private set; }

	public static SteamUGCService Instance
	{
		get
		{
			SteamUGCService.Init();
			return SteamUGCService.instance;
		}
	}

	public static void Init()
	{
		if (SteamUGCService.instance == null)
		{
			global::Debug.Log("Initialising UGC Service", null);
			GameObject gameObject = GameObject.Find("/SteamManager");
			SteamUGCService.instance = gameObject.GetComponent<SteamUGCService>();
			if (SteamUGCService.instance == null)
			{
				SteamUGCService.instance = gameObject.AddComponent<SteamUGCService>();
			}
		}
	}

	public void DoDownload()
	{
	}

	public List<SteamUGCService.Subscibed> GetSubscribed()
	{
		List<SteamUGCService.Subscibed> list = new List<SteamUGCService.Subscibed>();
		if (this.details != null && this.subscribed != null)
		{
			for (int i = 0; i < this.details.Length; i++)
			{
				PublishedFileId_t nPublishedFileId = this.details[i].m_nPublishedFileId;
				if (this.subscribed.Contains(nPublishedFileId))
				{
					list.Add(new SteamUGCService.Subscibed(this.details[i], nPublishedFileId == this.currentLanguage));
				}
			}
		}
		return list;
	}

	public Texture2D GetPreviewImage(PublishedFileId_t item)
	{
		if (this.previewImages.ContainsKey(item))
		{
			return this.previewImages[item];
		}
		return null;
	}

	public void SetCurrentLanguage(PublishedFileId_t item)
	{
		this.InstallLanguageFile(item, false);
	}

	public static bool HasInstalledLanguage()
	{
		global::System.DateTime dateTime;
		global::System.DateTime dateTime2;
		return SteamUGCService.GetInstalledFileID(out dateTime, out dateTime2) != PublishedFileId_t.Invalid;
	}

	public static global::System.DateTime FromUnixTime(long unixTime)
	{
		global::System.DateTime dateTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return dateTime.AddSeconds((double)unixTime);
	}

	public string GetInstalledLanguageCode(out PublishedFileId_t installed)
	{
		string text = string.Empty;
		global::System.DateTime dateTime;
		global::System.DateTime dateTime2;
		string languageFile = SteamUGCService.GetLanguageFile(out installed, out dateTime, out dateTime2);
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

	public string GetInstalledLanguageData()
	{
		PublishedFileId_t publishedFileId_t;
		global::System.DateTime dateTime;
		global::System.DateTime dateTime2;
		string languageFile = SteamUGCService.GetLanguageFile(out publishedFileId_t, out dateTime, out dateTime2);
		if (languageFile == null || !File.Exists(languageFile))
		{
			return "None";
		}
		string[] array = File.ReadAllLines(languageFile, Encoding.UTF8);
		Localization.Locale locale = Localization.GetLocale(array);
		string text = "Error";
		if (locale != null && locale.Code != null && locale.Code != string.Empty)
		{
			text = locale.Code;
		}
		string text2 = string.Concat(new object[] { "Code: [", text, "] Id: [", publishedFileId_t.m_PublishedFileId, "]" });
		if (this.details != null)
		{
			for (int i = 0; i < this.details.Length; i++)
			{
				PublishedFileId_t nPublishedFileId = this.details[i].m_nPublishedFileId;
				if (publishedFileId_t == nPublishedFileId)
				{
					if (this.details[i].m_rgchTitle != null && this.details[i].m_rgchTitle.Length > 0)
					{
						text2 = text2 + " Title: [" + this.details[i].m_rgchTitle + "]";
					}
					if (this.details[i].m_rgchURL != null && this.details[i].m_rgchURL.Length > 0)
					{
						text2 = text2 + " URL: [" + this.details[i].m_rgchURL + "]";
					}
					text2 = text2 + " Last Modified (UTC): [" + SteamUGCService.FromUnixTime((long)((ulong)this.details[i].m_rtimeUpdated)).ToString() + "]";
					break;
				}
			}
		}
		if (dateTime != global::System.DateTime.MinValue)
		{
			text2 = text2 + " Last Modified On Disk (UTC): [" + dateTime.ToString() + "]";
		}
		if (dateTime2 != global::System.DateTime.MinValue)
		{
			text2 = text2 + " Last Modified PO (UTC): [" + dateTime2.ToString() + "]";
		}
		return text2;
	}

	public static string GetInstalledLanguageFile()
	{
		PublishedFileId_t publishedFileId_t;
		global::System.DateTime dateTime;
		global::System.DateTime dateTime2;
		return SteamUGCService.GetLanguageFile(out publishedFileId_t, out dateTime, out dateTime2);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		global::System.DateTime dateTime;
		global::System.DateTime dateTime2;
		string languageFileFromSteam = SteamUGCService.GetLanguageFileFromSteam(item, out dateTime, out dateTime2);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			string[] array = languageFileFromSteam.Split(new char[] { '\n' });
			string fontForLocalisation = SteamUGCService.GetFontForLocalisation(array);
			return Resources.Load<TMP_FontAsset>(fontForLocalisation);
		}
		return null;
	}

	public static void LoadTranslation()
	{
		string installedLanguageFile = SteamUGCService.GetInstalledLanguageFile();
		Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, installedLanguageFile);
	}

	public void OnEnable()
	{
		this.setupComplete = false;
		SteamUGCService.instance = this;
	}

	private void OnDestroy()
	{
		SteamUGCService.instance = null;
	}

	private void Update()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (!this.setupComplete && Global.Instance != null && DistributionPlatform.Initialized)
		{
			this.Setup();
		}
		if (this.doClearList)
		{
			this.doClearList = false;
			this.ClearLists();
		}
		this.GetSubscribedDetails();
		this.numSubscriptions = SteamUGC.GetNumSubscribedItems();
		this.UpdateSubscription(this.numSubscriptions);
		if (this.details == null)
		{
			return;
		}
		for (int i = 0; i < this.details.Length; i++)
		{
			PublishedFileId_t nPublishedFileId = this.details[i].m_nPublishedFileId;
			if (this.subscribed != null && this.subscribed.Contains(nPublishedFileId))
			{
				EItemState itemState = (EItemState)SteamUGC.GetItemState(nPublishedFileId);
				bool flag = ((itemState & EItemState.k_EItemStateInstalled) != EItemState.k_EItemStateInstalled) | ((itemState & EItemState.k_EItemStateNeedsUpdate) == EItemState.k_EItemStateNeedsUpdate);
				bool flag2 = ((itemState & EItemState.k_EItemStateDownloading) == EItemState.k_EItemStateDownloading) | ((itemState & EItemState.k_EItemStateDownloadPending) == EItemState.k_EItemStateDownloadPending);
				bool flag3 = flag && !flag2;
				if (flag3)
				{
					SteamUGC.DownloadItem(nPublishedFileId, false);
				}
				else if (!flag3 && !this.previewImages.ContainsKey(nPublishedFileId) && (itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled)
				{
					global::System.DateTime dateTime;
					global::System.DateTime dateTime2;
					byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(nPublishedFileId, SteamUGCService.previewFileNames, out dateTime, out dateTime2, false);
					if (this.currentLanguage == nPublishedFileId && this.currentLastModified < dateTime)
					{
						this.UpdateInstalledLanguage(nPublishedFileId);
					}
					if (bytesFromZip != null)
					{
						Texture2D texture2D = new Texture2D(2, 2);
						texture2D.LoadImage(bytesFromZip);
						this.previewImages.Add(nPublishedFileId, texture2D);
						this.doClearList = true;
					}
					if (SteamUGCService.getBytesRetryCount.ContainsKey(nPublishedFileId) && SteamUGCService.getBytesRetryCount[nPublishedFileId] > 3)
					{
						this.previewImages.Add(nPublishedFileId, null);
					}
				}
			}
		}
	}

	private static PublishedFileId_t GetInstalledFileID(out global::System.DateTime lastModified, out global::System.DateTime lastModifiedPofile)
	{
		PublishedFileId_t invalid;
		SteamUGCService.InstalledModData.Get(out invalid, out lastModified, out lastModifiedPofile);
		if (invalid == PublishedFileId_t.Invalid)
		{
			invalid = new PublishedFileId_t((ulong)KPlayerPrefs.GetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId));
			if (invalid != PublishedFileId_t.Invalid)
			{
				if (SteamUGCService.instance != null && SteamUGCService.instance.subscribed != null)
				{
					if (!SteamUGCService.instance.subscribed.Contains(invalid))
					{
						global::Debug.LogWarning("It doesnt look like we are subscribed..." + invalid, null);
						invalid = PublishedFileId_t.Invalid;
						SteamUGCService.instance.InstallLanguageFile(invalid, false);
					}
				}
				else
				{
					global::Debug.LogWarning("Cant check yet..." + invalid, null);
				}
			}
		}
		return invalid;
	}

	private void Setup()
	{
		this.m_ItemInstalled = Callback<ItemInstalled_t>.Create(new Callback<ItemInstalled_t>.DispatchDelegate(this.OnItemInstalled));
		this.m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(new Callback<DownloadItemResult_t>.DispatchDelegate(this.OnDownloadItemResult));
		this.m_ItemUninstalled = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(new Callback<RemoteStoragePublishedFileUnsubscribed_t>.DispatchDelegate(this.OnItemUnSubscibed));
		this.m_ItemUpdated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(new Callback<RemoteStoragePublishedFileUpdated_t>.DispatchDelegate(this.OnItemUpdated));
		this.OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryCompleted));
		this.OnSteamUGCQueryDetailsCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryDetailsCompleted));
		this.doClearList = true;
		this.currentLanguage = SteamUGCService.GetInstalledFileID(out this.currentLastModified, out this.currentLastModifiedPofile);
		this.setupComplete = true;
		global::Debug.Log("UGC Service setup complete..", null);
	}

	private void UpdateInstalledLanguage(PublishedFileId_t item)
	{
		string languageFileFromSteam = SteamUGCService.GetLanguageFileFromSteam(item, out this.currentLastModified, out this.currentLastModifiedPofile);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			SteamUGCService.InstalledModData.Set(item, this.currentLastModified, this.currentLastModifiedPofile);
			File.WriteAllText(Localization.GetModLocalizationFilePath(), languageFileFromSteam);
			return;
		}
		global::Debug.LogWarning(string.Concat(new object[] { "Loc file was empty.. [", item, "]  [", this.currentLastModified, "]  [", this.currentLastModifiedPofile, "] " }), null);
	}

	private void InstallLanguageFile(PublishedFileId_t item, bool fromDownload = false)
	{
		this.CleanUpCurrentModLanguage();
		if (item != PublishedFileId_t.Invalid)
		{
			this.UpdateInstalledLanguage(item);
		}
		SteamUGCService.LoadTranslation();
		this.currentLanguage = item;
	}

	public void CleanUpCurrentModLanguage()
	{
		KPlayerPrefs.SetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId);
		SteamUGCService.InstalledModData.Delete();
		string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
		if (File.Exists(modLocalizationFilePath))
		{
			File.Delete(modLocalizationFilePath);
		}
	}

	private static string GetFontForLocalisation(string[] lines)
	{
		return Localization.GetLocale(lines).FontName;
	}

	private static string GetLanguageFile(out PublishedFileId_t item, out global::System.DateTime lastModified, out global::System.DateTime lastModifiedPofile)
	{
		SteamUGCService.InstalledModData.Get(out item, out lastModified, out lastModifiedPofile);
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

	private static bool DoDownloadItem(PublishedFileId_t item)
	{
		if (SteamUGCService.waitingForDownload == item)
		{
			global::Debug.Log("We are waiting for [" + item + "] to download", null);
			return false;
		}
		if (SteamUGCService.waitingForDownload != PublishedFileId_t.Invalid)
		{
			global::Debug.Log(string.Concat(new object[]
			{
				"We are waiting for [",
				SteamUGCService.waitingForDownload,
				"] to download, cant downloand [",
				item,
				"] now"
			}), null);
			return false;
		}
		if (!SteamUGCService.getBytesRetryCount.ContainsKey(item))
		{
			SteamUGCService.getBytesRetryCount.Add(item, 0);
		}
		if (SteamUGCService.getBytesRetryCount[item] > SteamUGCService.MAX_FILE_RETRY_COUNT)
		{
			global::Debug.Log("Max retry count reached for [" + item + "]", null);
			return false;
		}
		if (!SteamUGC.DownloadItem(item, true))
		{
			global::Debug.Log("SteamUGC.DownloadItem returned false for [" + item + "]", null);
			return false;
		}
		Dictionary<PublishedFileId_t, int> dictionary2;
		Dictionary<PublishedFileId_t, int> dictionary = (dictionary2 = SteamUGCService.getBytesRetryCount);
		int num = dictionary2[item];
		dictionary[item] = num + 1;
		SteamUGCService.waitingForDownload = item;
		return true;
	}

	private static string GetLanguageFileFromSteam(PublishedFileId_t item, out global::System.DateTime lastModified, out global::System.DateTime lastModifiedPofile)
	{
		lastModified = global::System.DateTime.MinValue;
		lastModifiedPofile = global::System.DateTime.MinValue;
		if (item == PublishedFileId_t.Invalid)
		{
			global::Debug.LogWarning("Cant get INVALID file id from Steam", null);
			return null;
		}
		EItemState itemState = (EItemState)SteamUGC.GetItemState(item);
		if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled)
		{
			byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, SteamUGCService.poFile, out lastModified, out lastModifiedPofile, false);
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

	private static byte[] GetBytesFromZip(PublishedFileId_t item, string[] filesToExtract, out global::System.DateTime lastModified, out global::System.DateTime lastModifiedPofile, bool getFirstMatch = false)
	{
		byte[] array = null;
		lastModified = global::System.DateTime.MinValue;
		lastModifiedPofile = global::System.DateTime.MinValue;
		ulong num;
		string text;
		uint num2;
		SteamUGC.GetItemInstallInfo(item, out num, out text, 1024U, out num2);
		try
		{
			lastModified = File.GetLastWriteTimeUtc(text);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipFile zipFile = ZipFile.Read(text))
				{
					ZipEntry zipEntry = null;
					foreach (string file in filesToExtract)
					{
						if (file.Length > 4)
						{
							if (zipFile.ContainsEntry(file))
							{
								zipEntry = zipFile[file];
							}
						}
						else
						{
							zipEntry = zipFile.Entries.First<ZipEntry>((ZipEntry x) => x.FileName.EndsWith(file));
						}
						if (zipEntry != null)
						{
							break;
						}
					}
					if (zipEntry != null)
					{
						lastModifiedPofile = zipEntry.LastModified;
						zipEntry.Extract(memoryStream);
					}
				}
				memoryStream.Flush();
				array = memoryStream.ToArray();
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	private void RequestAvailableIfNeeded()
	{
		if (this.details == null && !this.listPending)
		{
			this.GetAvailable();
		}
	}

	private void GetSubscribedDetails()
	{
		if (this.listPending)
		{
			return;
		}
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		if (numSubscribedItems != 0U && (this.subscribed == null || (ulong)numSubscribedItems != (ulong)((long)this.subscribed.Count)))
		{
			PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, (uint)array.Length);
			this.subscribed = new List<PublishedFileId_t>(array);
			this.listPending = true;
			this.m_UGCQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
			SteamUGC.AddRequiredTag(this.m_UGCQueryHandle, "language");
			SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(this.m_UGCQueryHandle);
			this.OnSteamUGCQueryDetailsCompletedCallResult.Set(steamAPICall_t, null);
		}
	}

	private void GetAvailable()
	{
		global::Debug.Log("UGC Service requesting language mods list..", null);
		this.listPending = true;
		this.m_UGCQueryHandle = SteamUGC.CreateQueryAllUGCRequest(EUGCQuery.k_EUGCQuery_RankedByPublicationDate, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, (AppId_t)SteamUGCService.kModUploaderAppID, SteamUtils.GetAppID(), 1U);
		SteamUGC.AddRequiredTag(this.m_UGCQueryHandle, "language");
		SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(this.m_UGCQueryHandle);
		this.OnSteamUGCQueryCompletedCallResult.Set(steamAPICall_t, null);
	}

	private void ClearLists()
	{
		this.listPending = false;
		this.subscribed = null;
		this.details = null;
	}

	private void UpdateSubscription(uint num)
	{
		if (num != 0U && (this.subscribed == null || (ulong)num != (ulong)((long)this.subscribed.Count)))
		{
			PublishedFileId_t[] array = new PublishedFileId_t[num];
			SteamUGC.GetSubscribedItems(array, (uint)array.Length);
			this.subscribed = new List<PublishedFileId_t>(array);
		}
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			this.details = new SteamUGCDetails_t[pCallback.m_unNumResultsReturned];
			for (uint num = 0U; num < pCallback.m_unNumResultsReturned; num += 1U)
			{
				SteamUGC.GetQueryUGCResult(this.m_UGCQueryHandle, num, out this.details[(int)((UIntPtr)num)]);
			}
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			this.UpdateSubscription(numSubscribedItems);
			if (this.OnRefreshLanguage != null)
			{
				this.OnRefreshLanguage();
			}
			this.listPending = false;
		}
		else
		{
			global::Debug.Log(string.Concat(new object[] { "[SteamUGCQueryCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " -- NUm results: ", pCallback.m_unNumResultsReturned, " --Total Matching: ", pCallback.m_unTotalMatchingResults, " -- cached: ", pCallback.m_bCachedData }), null);
		}
		SteamUGC.ReleaseQueryUGCRequest(this.m_UGCQueryHandle);
	}

	private void OnSteamUGCQueryDetailsCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			this.details = new SteamUGCDetails_t[pCallback.m_unNumResultsReturned];
			for (uint num = 0U; num < pCallback.m_unNumResultsReturned; num += 1U)
			{
				SteamUGC.GetQueryUGCResult(this.m_UGCQueryHandle, num, out this.details[(int)((UIntPtr)num)]);
				if (!this.previews.ContainsKey(this.details[(int)((UIntPtr)num)].m_hPreviewFile))
				{
					this.previews.Add(this.details[(int)((UIntPtr)num)].m_hPreviewFile, this.details[(int)((UIntPtr)num)].m_nPublishedFileId);
					this.AddPreviewToDownloadQueue(this.details[(int)((UIntPtr)num)].m_hPreviewFile);
				}
			}
			this.listPending = false;
		}
		else if (pCallback.m_eResult == EResult.k_EResultBusy)
		{
			global::Debug.Log(string.Concat(new object[] { "[OnSteamUGCQueryDetailsCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " Resending" }), null);
			this.listPending = false;
		}
		else
		{
			global::Debug.Log(string.Concat(new object[] { "[OnSteamUGCQueryDetailsCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " -- NUm results: ", pCallback.m_unNumResultsReturned, " --Total Matching: ", pCallback.m_unTotalMatchingResults, " -- cached: ", pCallback.m_bCachedData }), null);
		}
		SteamUGC.ReleaseQueryUGCRequest(this.m_UGCQueryHandle);
	}

	private void AddPreviewToDownloadQueue(UGCHandle_t next)
	{
		CallResult<RemoteStorageDownloadUGCResult_t> callResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(new CallResult<RemoteStorageDownloadUGCResult_t>.APIDispatchDelegate(this.OnDownloadPreviewResult));
		SteamAPICall_t steamAPICall_t = SteamRemoteStorage.UGCDownload(next, 0U);
		callResult.Set(steamAPICall_t, null);
		this.m_DownloadPreviewResult.Add(next, callResult);
	}

	private void OnDownloadPreviewResult(RemoteStorageDownloadUGCResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK && !this.previewImages.ContainsKey(this.previews[pCallback.m_hFile]))
		{
			byte[] array = new byte[pCallback.m_nSizeInBytes];
			SteamRemoteStorage.UGCRead(pCallback.m_hFile, array, array.Length, 0U, EUGCReadAction.k_EUGCRead_ContinueReadingUntilFinished);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(array);
			this.previewImages.Add(this.previews[pCallback.m_hFile], texture2D);
		}
		this.m_DownloadPreviewResult.Remove(pCallback.m_hFile);
		if (this.m_DownloadPreviewResult.Count == 0)
		{
			this.doClearList = true;
			if (this.OnRefreshLanguage != null)
			{
				this.OnRefreshLanguage();
			}
		}
	}

	private void OnItemInstalled(ItemInstalled_t pCallback)
	{
		if (this.currentLanguage == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Item Install detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
		}
		this.doClearList = true;
	}

	private void OnItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		if (this.currentLanguage == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Update detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			SteamUGCService.DoDownloadItem(pCallback.m_nPublishedFileId);
		}
		this.doClearList = true;
	}

	private void OnItemUnSubscibed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		if (this.currentLanguage == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Unsubscribe detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			this.CleanUpCurrentModLanguage();
		}
		this.doClearList = true;
	}

	private void OnDownloadItemResult(DownloadItemResult_t pCallback)
	{
		if (SteamUGCService.waitingForDownload == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Download complete for waitingForDownload [" + SteamUGCService.waitingForDownload + "]", null);
			SteamUGCService.waitingForDownload = PublishedFileId_t.Invalid;
		}
		if (this.currentLanguage == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Download complete for currently installed font [" + pCallback.m_nPublishedFileId + "] updating in background. Changes will happen next restart.", null);
			this.UpdateInstalledLanguage(pCallback.m_nPublishedFileId);
		}
		this.doClearList = true;
	}

	private const string INSTALLED_LANGUAGE = "InstalledLanguage";

	public static uint kModUploaderAppID = 636750U;

	private UGCQueryHandle_t m_UGCQueryHandle;

	protected Callback<RemoteStoragePublishedFileUpdated_t> m_ItemUpdated;

	protected Callback<RemoteStoragePublishedFileUnsubscribed_t> m_ItemUninstalled;

	protected Callback<ItemInstalled_t> m_ItemInstalled;

	protected Callback<DownloadItemResult_t> m_DownloadItemResult;

	protected Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>> m_DownloadPreviewResult = new Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>>();

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryDetailsCompletedCallResult;

	private bool listPending;

	private List<PublishedFileId_t> subscribed;

	private SteamUGCDetails_t[] details;

	private Dictionary<PublishedFileId_t, Texture2D> previewImages = new Dictionary<PublishedFileId_t, Texture2D>();

	private Dictionary<UGCHandle_t, PublishedFileId_t> previews = new Dictionary<UGCHandle_t, PublishedFileId_t>();

	private bool doClearList;

	private global::System.DateTime currentLastModified;

	private global::System.DateTime currentLastModifiedPofile;

	public global::System.Action OnRefreshLanguage;

	private static readonly string[] previewFileNames = new string[] { "preview.png", "preview.png", ".png", ".jpg" };

	private static readonly string[] poFile = new string[] { "strings.po" };

	private static string currentFont = string.Empty;

	private bool setupComplete;

	private static SteamUGCService instance;

	private static readonly int MAX_FILE_RETRY_COUNT = 3;

	private static PublishedFileId_t waitingForDownload = PublishedFileId_t.Invalid;

	private static Dictionary<PublishedFileId_t, int> getBytesRetryCount = new Dictionary<PublishedFileId_t, int>();

	private class InstalledModData : YamlIO<SteamUGCService.InstalledModData>
	{
		private static string FilePath()
		{
			return Path.Combine(Application.streamingAssetsPath, SteamUGCService.InstalledModData.FILE_NAME);
		}

		public static void Set(PublishedFileId_t item, global::System.DateTime lastModified, global::System.DateTime lastModifiedPofile)
		{
			new SteamUGCService.InstalledModData
			{
				PublishedFileId = item.m_PublishedFileId,
				LastModified = lastModified.ToFileTimeUtc(),
				LastModifiedPofile = lastModifiedPofile.ToFileTimeUtc()
			}.Save(SteamUGCService.InstalledModData.FilePath());
		}

		public static void Get(out PublishedFileId_t item, out global::System.DateTime lastModified, out global::System.DateTime lastModifiedPofile)
		{
			if (SteamUGCService.InstalledModData.Exists())
			{
				SteamUGCService.InstalledModData installedModData = YamlIO<SteamUGCService.InstalledModData>.LoadFile(SteamUGCService.InstalledModData.FilePath());
				if (installedModData != null)
				{
					lastModified = global::System.DateTime.FromFileTimeUtc(installedModData.LastModified);
					lastModifiedPofile = global::System.DateTime.FromFileTimeUtc(installedModData.LastModifiedPofile);
					item = new PublishedFileId_t(installedModData.PublishedFileId);
					return;
				}
			}
			lastModified = global::System.DateTime.MinValue;
			lastModifiedPofile = global::System.DateTime.MinValue;
			item = PublishedFileId_t.Invalid;
		}

		public static bool Exists()
		{
			return File.Exists(SteamUGCService.InstalledModData.FilePath());
		}

		public static void Delete()
		{
			if (SteamUGCService.InstalledModData.Exists())
			{
				File.Delete(SteamUGCService.InstalledModData.FilePath());
			}
		}

		public ulong PublishedFileId { get; set; }

		public long LastModified { get; set; }

		public long LastModifiedPofile { get; set; }

		private static readonly string FILE_NAME = "Mods/mod_installed.dat";
	}

	public class Subscibed
	{
		public Subscibed(SteamUGCDetails_t item, bool isCurrent)
		{
			this.title = item.m_rgchTitle;
			this.description = item.m_rgchDescription;
			this.fileId = item.m_nPublishedFileId;
			this.isActiveLanguage = isCurrent;
		}

		public string title { get; private set; }

		public string description { get; private set; }

		public PublishedFileId_t fileId { get; private set; }

		public bool isActiveLanguage { get; private set; }
	}
}
