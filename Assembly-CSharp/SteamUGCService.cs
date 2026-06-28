using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
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
			if (SteamUGCService.instance == null)
			{
				GameObject gameObject = GameObject.Find("/SteamManager");
				SteamUGCService.instance = gameObject.GetComponent<SteamUGCService>();
				if (SteamUGCService.instance == null)
				{
					SteamUGCService.instance = gameObject.AddComponent<SteamUGCService>();
				}
			}
			return SteamUGCService.instance;
		}
	}

	public void ClearInstall()
	{
		this.InstallLanguageFile(PublishedFileId_t.Invalid);
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
		this.InstallLanguageFile(item);
	}

	public void OnEnable()
	{
		this.setupComplete = false;
		Debug.Assert(SteamUGCService.instance == null);
		SteamUGCService.instance = this;
	}

	private void Setup()
	{
		this.m_ItemInstalled = Callback<ItemInstalled_t>.Create(new Callback<ItemInstalled_t>.DispatchDelegate(this.OnItemInstalled));
		this.m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(new Callback<DownloadItemResult_t>.DispatchDelegate(this.OnDownloadItemResult));
		this.OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryCompleted));
		this.doClearList = true;
		this.currentLanguage = SteamUGCService.GetInstalledLanguage();
		this.setupComplete = true;
	}

	private void OnDestroy()
	{
		Debug.Assert(SteamUGCService.instance == this);
		SteamUGCService.instance = null;
	}

	public bool RequiresDownload()
	{
		return this.showDownload;
	}

	private void Update()
	{
		if (!this.setupComplete && Global.Instance != null && DistributionPlatform.Initialized)
		{
			this.Setup();
		}
		if (this.doClearList)
		{
			this.doClearList = false;
			this.ClearLists();
		}
		this.RequestAvailableIfNeeded();
		this.numSubscriptions = SteamUGC.GetNumSubscribedItems();
		this.UpdateSubscription(this.numSubscriptions);
		if (this.details == null)
		{
			return;
		}
		this.showDownload = false;
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
					SteamUGC.DownloadItem(nPublishedFileId, true);
				}
				else if (!flag3 && !this.previewImages.ContainsKey(nPublishedFileId))
				{
					byte[] array = null;
					if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled)
					{
						array = SteamUGCService.GetBytesFromZip(nPublishedFileId, "preview.png");
					}
					if (array != null)
					{
						Texture2D texture2D = new Texture2D(128, 64);
						texture2D.LoadImage(array);
						this.previewImages.Add(nPublishedFileId, texture2D);
						this.doClearList = true;
					}
				}
			}
		}
	}

	private static PublishedFileId_t GetInstalledLanguage()
	{
		int @int = PlayerPrefs.GetInt("InstalledLanguage", (int)PublishedFileId_t.Invalid.m_PublishedFileId);
		return new PublishedFileId_t((ulong)@int);
	}

	public static bool HasInstalledLanguage()
	{
		return SteamUGCService.GetInstalledLanguage() != PublishedFileId_t.Invalid;
	}

	public static string GetLanguageFile(PublishedFileId_t item)
	{
		if (item != PublishedFileId_t.Invalid && (SteamUGC.GetItemState(item) & 4U) == 4U)
		{
			byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, "strings.po");
			if (bytesFromZip != null)
			{
				return Encoding.UTF8.GetString(bytesFromZip);
			}
		}
		return null;
	}

	public static string GetInstalledLanguageFile()
	{
		PublishedFileId_t installedLanguage = SteamUGCService.GetInstalledLanguage();
		return SteamUGCService.GetLanguageFile(installedLanguage);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		string languageFile = SteamUGCService.GetLanguageFile(item);
		if (languageFile != null && languageFile.Length > 0)
		{
			string[] array = languageFile.Split(new char[] { '\n' });
			string fontForLocalisation = SteamUGCService.GetFontForLocalisation(array);
			return Resources.Load<TMP_FontAsset>(fontForLocalisation);
		}
		return null;
	}

	private void InstallLanguageFile(PublishedFileId_t item)
	{
		PlayerPrefs.SetInt("InstalledLanguage", (int)item.m_PublishedFileId);
		SteamUGCService.LoadTranslation();
		this.currentLanguage = item;
		App.LoadScene("frontend");
	}

	public static void LoadTranslation()
	{
		string installedLanguageFile = SteamUGCService.GetInstalledLanguageFile();
		if (installedLanguageFile != null && installedLanguageFile.Length > 0)
		{
			string[] array = installedLanguageFile.Split(new char[] { '\n' });
			SteamUGCService.SetFontForLocalization(array);
			Localization.LoadTranslation(array);
		}
		else
		{
			SteamUGCService.SetFontForLocalization(null);
		}
	}

	private static string GetFontForLocalisation(string[] lines)
	{
		string text;
		if (lines != null)
		{
			text = Localization.GetFontForLocalization(lines);
		}
		else
		{
			text = Localization.GetFontNameForLocalization(string.Empty);
		}
		return text;
	}

	private static void SetFontForLocalization(string[] lines)
	{
		Localization.SwapToLocalizedFont(SteamUGCService.GetFontForLocalisation(lines));
	}

	public static void SetFontForLocalization()
	{
		string installedLanguageFile = SteamUGCService.GetInstalledLanguageFile();
		if (installedLanguageFile != null && installedLanguageFile.Length > 0)
		{
			string[] array = installedLanguageFile.Split(new char[] { '\n' });
			Localization.SwapToLocalizedFont(SteamUGCService.GetFontForLocalisation(array));
		}
	}

	private static byte[] GetBytesFromZip(PublishedFileId_t item, string fileToExtract)
	{
		byte[] array = null;
		ulong num;
		string text;
		uint num2;
		SteamUGC.GetItemInstallInfo(item, out num, out text, 1024U, out num2);
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipFile zipFile = ZipFile.Read(text))
				{
					ZipEntry zipEntry = zipFile[fileToExtract];
					zipEntry.Extract(memoryStream);
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

	private void GetAvailable()
	{
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
			Debug.Log("Refresh should be called next");
			if (this.OnRefreshLanguage != null)
			{
				Debug.Log("Refresh");
				this.OnRefreshLanguage();
			}
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
			this.listPending = false;
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			this.UpdateSubscription(numSubscribedItems);
		}
		else
		{
			Debug.Log(string.Concat(new object[] { "[SteamUGCQueryCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " -- NUm results: ", pCallback.m_unNumResultsReturned, " --Total Matching: ", pCallback.m_unTotalMatchingResults, " -- cached: ", pCallback.m_bCachedData }));
		}
		SteamUGC.ReleaseQueryUGCRequest(this.m_UGCQueryHandle);
	}

	private void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
	{
		this.doClearList = true;
	}

	private void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
	{
		this.doClearList = true;
	}

	private void OnItemInstalled(ItemInstalled_t pCallback)
	{
		this.doClearList = true;
	}

	private void OnDownloadItemResult(DownloadItemResult_t pCallback)
	{
		this.doClearList = true;
	}

	private const string INSTALLED_LANGUAGE = "InstalledLanguage";

	public static uint kModUploaderAppID = 636750U;

	private Vector2 m_ScrollPos;

	private UGCQueryHandle_t m_UGCQueryHandle;

	protected Callback<ItemInstalled_t> m_ItemInstalled;

	protected Callback<DownloadItemResult_t> m_DownloadItemResult;

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private bool listPending;

	private List<PublishedFileId_t> subscribed;

	private SteamUGCDetails_t[] details;

	private Dictionary<PublishedFileId_t, Texture2D> previewImages = new Dictionary<PublishedFileId_t, Texture2D>();

	private bool doClearList;

	private bool showDownload;

	public global::System.Action OnRefreshLanguage;

	private static SteamUGCService instance;

	private bool setupComplete;

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
