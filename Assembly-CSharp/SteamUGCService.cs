using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Steamworks;
using UnityEngine;

public class SteamUGCService : MonoBehaviour
{
	public uint numSubscriptions { get; private set; }

	public bool setupComplete { get; private set; }

	public static SteamUGCService Instance
	{
		get
		{
			return SteamUGCService.instance;
		}
	}

	public static void Initialize()
	{
		if (SteamUGCService.instance == null)
		{
			global::Debug.Log("Initialising UGC Service");
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

	public List<SteamUGCService.Subscribed> GetSubscribed()
	{
		List<SteamUGCService.Subscribed> list = new List<SteamUGCService.Subscribed>();
		if (this.details == null)
		{
			return list;
		}
		if (this.subscribed == null)
		{
			return list;
		}
		foreach (SteamUGCDetails_t steamUGCDetails_t in this.details)
		{
			if (this.subscribed.Contains(steamUGCDetails_t.m_nPublishedFileId))
			{
				list.Add(new SteamUGCService.Subscribed(steamUGCDetails_t));
			}
		}
		return list;
	}

	public List<SteamUGCService.Subscribed> GetSubscribed(string required_tag)
	{
		List<SteamUGCService.Subscribed> list = new List<SteamUGCService.Subscribed>();
		if (this.details == null)
		{
			return list;
		}
		if (this.subscribed == null)
		{
			return list;
		}
		for (int i = 0; i < this.details.Length; i++)
		{
			SteamUGCDetails_t steamUGCDetails_t = this.details[i];
			string[] array = steamUGCDetails_t.m_rgchTags.Split(new char[] { ',' });
			if (Array.IndexOf<string>(array, required_tag) >= 0)
			{
				PublishedFileId_t nPublishedFileId = steamUGCDetails_t.m_nPublishedFileId;
				if (this.subscribed.Contains(nPublishedFileId))
				{
					list.Add(new SteamUGCService.Subscribed(steamUGCDetails_t));
				}
			}
		}
		return list;
	}

	public SteamUGCService.Subscribed GetSubscribed(PublishedFileId_t id)
	{
		SteamUGCService.Subscribed subscribed = null;
		if (this.details == null)
		{
			return null;
		}
		if (this.subscribed == null)
		{
			return null;
		}
		for (int i = 0; i < this.details.Length; i++)
		{
			SteamUGCDetails_t steamUGCDetails_t = this.details[i];
			if (steamUGCDetails_t.m_nPublishedFileId == id)
			{
				subscribed = new SteamUGCService.Subscribed(steamUGCDetails_t);
				break;
			}
		}
		return subscribed;
	}

	public bool IsSubscribedTo(PublishedFileId_t id)
	{
		bool flag = false;
		if (this.subscribed != null)
		{
			foreach (PublishedFileId_t publishedFileId_t in this.subscribed)
			{
				if (publishedFileId_t.m_PublishedFileId == id.m_PublishedFileId)
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	public Texture2D GetPreviewImage(PublishedFileId_t item)
	{
		if (this.previewImages.ContainsKey(item))
		{
			return this.previewImages[item];
		}
		return null;
	}

	public void Awake()
	{
		this.setupComplete = false;
		global::Debug.Assert(SteamUGCService.instance == null);
		SteamUGCService.instance = this;
	}

	private void OnDestroy()
	{
		global::Debug.Assert(SteamUGCService.instance == this);
		SteamUGCService.instance = null;
	}

	private void Update()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (Game.Instance != null)
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
				else if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled && !this.previewImages.ContainsKey(nPublishedFileId))
				{
					global::System.DateTime dateTime;
					byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(nPublishedFileId, SteamUGCService.previewFileNames, out dateTime, false);
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

	private void Setup()
	{
		this.m_ItemSubscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(new Callback<RemoteStoragePublishedFileSubscribed_t>.DispatchDelegate(this.OnItemSubscribed));
		this.m_ItemInstalled = Callback<ItemInstalled_t>.Create(new Callback<ItemInstalled_t>.DispatchDelegate(this.OnItemInstalled));
		this.m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(new Callback<DownloadItemResult_t>.DispatchDelegate(this.OnDownloadItemResult));
		this.m_ItemUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(new Callback<RemoteStoragePublishedFileUnsubscribed_t>.DispatchDelegate(this.OnItemUnsubscribed));
		this.m_ItemUpdated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(new Callback<RemoteStoragePublishedFileUpdated_t>.DispatchDelegate(this.OnItemUpdated));
		this.OnSteamUGCQueryDetailsCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryDetailsCompleted));
		this.doClearList = true;
		this.setupComplete = true;
		global::Debug.Log("UGC Service setup complete..");
	}

	public static bool DoDownloadItem(PublishedFileId_t item)
	{
		if (SteamUGCService.waitingForDownload == item)
		{
			global::Debug.Log("We are waiting for [" + item + "] to download");
			return false;
		}
		if (SteamUGCService.waitingForDownload != PublishedFileId_t.Invalid)
		{
			global::Debug.Log(string.Concat(new object[]
			{
				"We are waiting for [",
				SteamUGCService.waitingForDownload,
				"] to download, cant download [",
				item,
				"] now"
			}));
			return false;
		}
		if (!SteamUGCService.getBytesRetryCount.ContainsKey(item))
		{
			SteamUGCService.getBytesRetryCount.Add(item, 0);
		}
		if (SteamUGCService.getBytesRetryCount[item] > SteamUGCService.MAX_FILE_RETRY_COUNT)
		{
			global::Debug.Log("Max retry count reached for [" + item + "]");
			return false;
		}
		if (!SteamUGC.DownloadItem(item, true))
		{
			global::Debug.Log("SteamUGC.DownloadItem returned false for [" + item + "]");
			return false;
		}
		Dictionary<PublishedFileId_t, int> dictionary;
		(dictionary = SteamUGCService.getBytesRetryCount)[item] = dictionary[item] + 1;
		SteamUGCService.waitingForDownload = item;
		return true;
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
			SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(this.m_UGCQueryHandle);
			this.OnSteamUGCQueryDetailsCompletedCallResult.Set(steamAPICall_t, null);
		}
	}

	public SteamUGCDetails_t GetDetails(PublishedFileId_t item)
	{
		SteamUGCDetails_t steamUGCDetails_t = default(SteamUGCDetails_t);
		if (this.details == null)
		{
			return steamUGCDetails_t;
		}
		foreach (SteamUGCDetails_t steamUGCDetails_t2 in this.details)
		{
			if (steamUGCDetails_t2.m_nPublishedFileId == item)
			{
				steamUGCDetails_t = steamUGCDetails_t2;
				break;
			}
		}
		return steamUGCDetails_t;
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
			foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
			{
				iugceventHandler.OnUGCRefresh();
			}
			this.listPending = false;
		}
		else
		{
			global::Debug.Log(string.Concat(new object[] { "[SteamUGCQueryCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " -- NUm results: ", pCallback.m_unNumResultsReturned, " --Total Matching: ", pCallback.m_unTotalMatchingResults, " -- cached: ", pCallback.m_bCachedData }));
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
			foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
			{
				iugceventHandler.OnUGCRefresh();
			}
			this.listPending = false;
		}
		else if (pCallback.m_eResult == EResult.k_EResultBusy)
		{
			global::Debug.Log(string.Concat(new object[] { "[OnSteamUGCQueryDetailsCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " Resending" }));
			this.listPending = false;
		}
		else
		{
			global::Debug.Log(string.Concat(new object[] { "[OnSteamUGCQueryDetailsCompleted] - handle: ", pCallback.m_handle, " -- Result: ", pCallback.m_eResult, " -- NUm results: ", pCallback.m_unNumResultsReturned, " --Total Matching: ", pCallback.m_unTotalMatchingResults, " -- cached: ", pCallback.m_bCachedData }));
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
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			Texture2D texture2D = null;
			if (!this.previewImages.TryGetValue(this.previews[pCallback.m_hFile], out texture2D) || texture2D == null)
			{
				byte[] array = new byte[pCallback.m_nSizeInBytes];
				SteamRemoteStorage.UGCRead(pCallback.m_hFile, array, array.Length, 0U, EUGCReadAction.k_EUGCRead_ContinueReadingUntilFinished);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(array);
				this.previewImages[this.previews[pCallback.m_hFile]] = texture2D;
			}
		}
		this.m_DownloadPreviewResult.Remove(pCallback.m_hFile);
		if (this.m_DownloadPreviewResult.Count == 0)
		{
			this.doClearList = true;
		}
	}

	private void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback)
	{
		foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
		{
			iugceventHandler.OnUGCItemSubscribed(pCallback);
		}
		this.doClearList = true;
	}

	private void OnItemInstalled(ItemInstalled_t pCallback)
	{
		foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
		{
			iugceventHandler.OnUGCItemInstalled(pCallback);
		}
		this.doClearList = true;
	}

	private void OnItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
		{
			iugceventHandler.OnUGCItemUpdated(pCallback);
		}
		this.doClearList = true;
	}

	private void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
		{
			iugceventHandler.OnUGCItemUnsubscribed(pCallback);
		}
		this.doClearList = true;
	}

	private void OnDownloadItemResult(DownloadItemResult_t pCallback)
	{
		if (SteamUGCService.waitingForDownload == pCallback.m_nPublishedFileId)
		{
			global::Debug.Log("Download complete for waitingForDownload [" + SteamUGCService.waitingForDownload + "]");
			SteamUGCService.waitingForDownload = PublishedFileId_t.Invalid;
		}
		foreach (SteamUGCService.IUGCEventHandler iugceventHandler in this.ugcEventHandlers)
		{
			iugceventHandler.OnUGCItemDownloaded(pCallback);
		}
		this.doClearList = true;
	}

	public static byte[] GetBytesFromZip(PublishedFileId_t item, string[] filesToExtract, out global::System.DateTime lastModified, bool getFirstMatch = false)
	{
		byte[] array = null;
		lastModified = global::System.DateTime.MinValue;
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
					foreach (string text2 in filesToExtract)
					{
						if (text2.Length > 4)
						{
							if (zipFile.ContainsEntry(text2))
							{
								zipEntry = zipFile[text2];
							}
						}
						else
						{
							foreach (ZipEntry zipEntry2 in zipFile.Entries)
							{
								if (zipEntry2.FileName.EndsWith(text2))
								{
									zipEntry = zipEntry2;
									break;
								}
							}
						}
						if (zipEntry != null)
						{
							break;
						}
					}
					if (zipEntry != null)
					{
						zipEntry.Extract(memoryStream);
						memoryStream.Flush();
						array = memoryStream.ToArray();
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	private UGCQueryHandle_t m_UGCQueryHandle;

	protected Callback<RemoteStoragePublishedFileSubscribed_t> m_ItemSubscribed;

	protected Callback<RemoteStoragePublishedFileUpdated_t> m_ItemUpdated;

	protected Callback<RemoteStoragePublishedFileUnsubscribed_t> m_ItemUnsubscribed;

	protected Callback<ItemInstalled_t> m_ItemInstalled;

	protected Callback<DownloadItemResult_t> m_DownloadItemResult;

	protected Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>> m_DownloadPreviewResult = new Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>>();

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryDetailsCompletedCallResult;

	private bool listPending;

	private List<PublishedFileId_t> subscribed;

	private SteamUGCDetails_t[] details;

	private Dictionary<PublishedFileId_t, Texture2D> previewImages = new Dictionary<PublishedFileId_t, Texture2D>();

	private Dictionary<UGCHandle_t, PublishedFileId_t> previews = new Dictionary<UGCHandle_t, PublishedFileId_t>();

	private bool doClearList;

	private static PublishedFileId_t waitingForDownload = PublishedFileId_t.Invalid;

	private static Dictionary<PublishedFileId_t, int> getBytesRetryCount = new Dictionary<PublishedFileId_t, int>();

	private static readonly string[] previewFileNames = new string[] { "preview.png", "preview.png", ".png", ".jpg" };

	public List<SteamUGCService.IUGCEventHandler> ugcEventHandlers = new List<SteamUGCService.IUGCEventHandler>();

	private static SteamUGCService instance;

	private static readonly int MAX_FILE_RETRY_COUNT = 3;

	public interface IUGCEventHandler
	{
		void OnUGCItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback);

		void OnUGCItemInstalled(ItemInstalled_t pCallback);

		void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback);

		void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback);

		void OnUGCItemDownloaded(DownloadItemResult_t pCallback);

		void OnUGCRefresh();
	}

	public class Subscribed
	{
		public Subscribed(SteamUGCDetails_t item)
		{
			this.title = item.m_rgchTitle;
			this.description = item.m_rgchDescription;
			this.fileId = item.m_nPublishedFileId;
			this.lastUpdateTime = (ulong)item.m_rtimeUpdated;
		}

		public string title { get; private set; }

		public string description { get; private set; }

		public PublishedFileId_t fileId { get; private set; }

		public ulong lastUpdateTime { get; private set; }
	}
}
