using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Klei;
using Newtonsoft.Json;
using UnityEngine;

public class KleiItems : ThreadedHttps<KleiItems>
{
	public static IEnumerable<KleiItems.ItemData> IterateInventory(Dictionary<string, string> item_to_permit)
	{
		if (KleiItems.InventoryData.AllItems != null)
		{
			foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
			{
				string text;
				if (item_to_permit.TryGetValue(item.ItemType, out text))
				{
					KleiItems.ItemData itemData;
					itemData.PermitId = text;
					itemData.ItemId = item.ItemId;
					itemData.IsOpened = item.IsOpened;
					yield return itemData;
				}
			}
			List<KleiItems.Item>.Enumerator enumerator = default(List<KleiItems.Item>.Enumerator);
		}
		yield break;
		yield break;
	}

	public static bool HasUnopenedItem(Dictionary<string, string> item_to_permit)
	{
		if (KleiItems.InventoryData.AllItems != null)
		{
			foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
			{
				if (item_to_permit.ContainsKey(item.ItemType) && !item.IsOpened)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public static bool HasUnclaimedRewards(HashSet<string> claimable)
	{
		for (int i = 0; i < KleiItems.InventoryData.UnclaimedRewards.Count; i++)
		{
			if (claimable.Contains(KleiItems.InventoryData.UnclaimedRewards[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasItem(string itemType)
	{
		if (KleiItems.InventoryData.ItemsByType == null)
		{
			return false;
		}
		List<KleiItems.Item> list;
		if (!KleiItems.InventoryData.ItemsByType.TryGetValue(itemType, out list))
		{
			return false;
		}
		if (list != null)
		{
			return list.Count<KleiItems.Item>((KleiItems.Item x) => x.IsOpened) > 0;
		}
		return false;
	}

	public static int GetOwnedItemCount(string itemType)
	{
		if (KleiItems.InventoryData.ItemsByType == null)
		{
			return 0;
		}
		List<KleiItems.Item> list;
		if (!KleiItems.InventoryData.ItemsByType.TryGetValue(itemType, out list))
		{
			return 0;
		}
		if (list == null)
		{
			return 0;
		}
		return list.Count<KleiItems.Item>((KleiItems.Item x) => x.IsOpened);
	}

	public static float SecondsToNextTick()
	{
		return ThreadedHttps<KleiItems>.Instance.TimeToNextTick;
	}

	public static void AddRequestInventoryRefresh()
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.GetAllItems, null);
	}

	public static void AddRequestItemOpened(ulong itemId)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.SetItemOpened, itemId);
	}

	public static void AddRequestTick()
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.Tick, null);
	}

	public static void AddRequestUserRewardsInfo()
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.GetUserRewardsInfo, null);
	}

	public static void AddInventoryRefreshCallback(KleiItems.InventoryRefreshCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.InventoryRefreshCbs.Add(cb);
	}

	public static void RemoveInventoryRefreshCallback(KleiItems.InventoryRefreshCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.InventoryRefreshCbs.Remove(cb);
	}

	public static void AddUserRewardInfoReceivedCallback(KleiItems.UserRewardInfoReceivedCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.UserRewardsInfoReceivedCbs.Add(cb);
	}

	public static void RemoveUserRewardInfoReceivedCallback(KleiItems.UserRewardInfoReceivedCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.UserRewardsInfoReceivedCbs.Remove(cb);
	}

	public void Update()
	{
		if (this.RequestCompleted)
		{
			KleiItems.Request activeRequest = this.ActiveRequest;
			if (!string.IsNullOrEmpty(this.Response))
			{
				if (activeRequest.Type == KleiItems.Request.RequestType.GetAllItems)
				{
					this.OnInventoryRecieved(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.SetItemOpened)
				{
					this.OnItemOpenedReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.Tick)
				{
					this.OnTickReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.GetUserRewardsInfo)
				{
					this.OnRewardsInfoReply(this.Response);
				}
			}
			this.RequestStarted = false;
			this.RequestCompleted = false;
			this.ActiveRequest = default(KleiItems.Request);
			this.Response = null;
		}
		if (this.WaitForReAuthentication)
		{
			return;
		}
		if (!this.RequestStarted && this.Requests.Count > 0)
		{
			KleiItems.Request request = this.Requests[0];
			this.Requests.RemoveAt(0);
			bool flag = false;
			if (request.Type == KleiItems.Request.RequestType.GetAllItems)
			{
				flag = this.RetrieveInventory();
			}
			else if (request.Type == KleiItems.Request.RequestType.SetItemOpened)
			{
				flag = this.RequestItemOpened((ulong)request.Data);
			}
			else if (request.Type == KleiItems.Request.RequestType.Tick)
			{
				flag = this.RequestTick();
			}
			else if (request.Type == KleiItems.Request.RequestType.GetUserRewardsInfo)
			{
				flag = this.RequestUserRewardsInfo();
			}
			if (flag)
			{
				this.RequestStarted = true;
				this.ActiveRequest = request;
			}
		}
		this.TimeToNextTick -= Time.unscaledDeltaTime;
		if (this.TimeToNextTick <= 0f)
		{
			KleiItems.AddRequestTick();
			this.TimeToNextTick += 360f;
		}
	}

	public KleiItems()
	{
		this.serviceName = "KleiItems";
		this.TimeToNextTick = 360f;
		KleiItems.InventoryData.AllItems = new List<KleiItems.Item>();
		KleiItems.InventoryData.ItemsByType = new Dictionary<string, List<KleiItems.Item>>();
		KleiItems.InventoryData.UnclaimedRewards = new List<string>();
	}

	private void AddRequest(KleiItems.Request.RequestType type, object data)
	{
		KleiItems.Request request;
		request.Type = type;
		request.Data = data;
		if (!this.Requests.Contains(request))
		{
			this.Requests.Add(request);
		}
	}

	private void StartHttpsRequest(string url)
	{
		this.LIVE_ENDPOINT = url;
		base.Start();
	}

	private void EndHttpsRequest()
	{
		base.End();
	}

	protected override void OnReplyRecieved(WebResponse response)
	{
		string text = "";
		if (response != null)
		{
			Stream responseStream = response.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			text = streamReader.ReadToEnd();
			streamReader.Close();
			responseStream.Close();
		}
		this.Response = text;
		this.RequestCompleted = true;
		this.EndHttpsRequest();
	}

	private void HandleError(KleiItems.Request req, string errorCode)
	{
		if (errorCode == "E_EXPIRED_TOKEN" || errorCode == "E_INVALID_TOKEN")
		{
			this.WaitForReAuthentication = true;
			this.AddRequest(req.Type, req.Data);
			ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.OnAuthenticateComplete), true);
		}
	}

	private void OnAuthenticateComplete()
	{
		this.WaitForReAuthentication = false;
	}

	private bool RetrieveInventory()
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "clientitems/ONI/GetAllItems");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object> { { "ClientToken", kleiToken } });
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnInventoryRecieved(string response)
	{
		KleiItems.InventoryReply inventoryReply = JsonConvert.DeserializeObject<KleiItems.InventoryReply>(response);
		if (!inventoryReply.Error)
		{
			KleiItems.InventoryData.AllItems.Clear();
			KleiItems.InventoryData.ItemsByType.Clear();
			if (inventoryReply.Items != null)
			{
				for (int i = 0; i < inventoryReply.Items.Length; i++)
				{
					KleiItems.InventoryReply.Item item = inventoryReply.Items[i];
					KleiItems.Item item2;
					item2.ItemId = item.ItemID;
					item2.ItemType = item.ItemType;
					item2.IsOpened = item.Context != 3;
					KleiItems.InventoryData.AllItems.Add(item2);
					List<KleiItems.Item> list;
					if (KleiItems.InventoryData.ItemsByType.TryGetValue(item2.ItemType, out list))
					{
						list.Add(item2);
					}
					else
					{
						KleiItems.InventoryData.ItemsByType[item2.ItemType] = new List<KleiItems.Item> { item2 };
					}
				}
			}
			this.SaveInventoryCache();
			using (List<KleiItems.InventoryRefreshCallback>.Enumerator enumerator = this.InventoryRefreshCbs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KleiItems.InventoryRefreshCallback inventoryRefreshCallback = enumerator.Current;
					inventoryRefreshCallback();
				}
				return;
			}
		}
		this.HandleError(this.ActiveRequest, inventoryReply.ErrorCode);
	}

	private bool RequestItemOpened(ulong itemId)
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "clientitems/ONI/SetItemOpened");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object>
		{
			{ "ClientToken", kleiToken },
			{ "ItemID", itemId }
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnItemOpenedReply(string response)
	{
		KleiItems.SetItemOpenedReply setItemOpenedReply = JsonConvert.DeserializeObject<KleiItems.SetItemOpenedReply>(response);
		if (!setItemOpenedReply.Error)
		{
			KleiItems.AddRequestInventoryRefresh();
			return;
		}
		this.HandleError(this.ActiveRequest, setItemOpenedReply.ErrorCode);
	}

	private bool RequestTick()
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "clientitems/ONI/Tick");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object> { { "Token", kleiToken } });
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnTickReply(string response)
	{
		KleiItems.TickReply tickReply = JsonConvert.DeserializeObject<KleiItems.TickReply>(response);
		if (!tickReply.Error)
		{
			if (tickReply.GiftReceived)
			{
				KleiItems.AddRequestInventoryRefresh();
				return;
			}
		}
		else
		{
			this.HandleError(this.ActiveRequest, tickReply.ErrorCode);
		}
	}

	private bool RequestUserRewardsInfo()
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "clientitems/ONI/GetUserRewardsInfo");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object> { { "Token", kleiToken } });
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnRewardsInfoReply(string response)
	{
		KleiItems.RewardsInfoReply rewardsInfoReply = JsonConvert.DeserializeObject<KleiItems.RewardsInfoReply>(response);
		if (!rewardsInfoReply.Error)
		{
			KleiItems.InventoryData.UnclaimedRewards.Clear();
			foreach (KeyValuePair<string, KleiItems.RewardsInfoReply.Info> keyValuePair in rewardsInfoReply.ItemRewards)
			{
				if (!keyValuePair.Value.Claimed)
				{
					KleiItems.InventoryData.UnclaimedRewards.Add(keyValuePair.Key);
				}
			}
			using (List<KleiItems.UserRewardInfoReceivedCallback>.Enumerator enumerator2 = this.UserRewardsInfoReceivedCbs.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KleiItems.UserRewardInfoReceivedCallback userRewardInfoReceivedCallback = enumerator2.Current;
					userRewardInfoReceivedCallback();
				}
				return;
			}
		}
		this.HandleError(this.ActiveRequest, rewardsInfoReply.ErrorCode);
	}

	private static uint hash(string s, uint seed = 0U)
	{
		uint num = seed;
		for (int i = 0; i < s.Length; i++)
		{
			num = (uint)s[i] + (num << 6) + (num << 16) - num;
		}
		return num;
	}

	private string GetCachePath(string userId)
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return Path.Combine(text, userId + ".json");
	}

	private void SaveInventoryCache()
	{
		if (!DistributionPlatform.Initialized)
		{
			return;
		}
		string text = DistributionPlatform.Inst.LocalUser.Id.ToString();
		KleiItems.InventoryCache inventoryCache;
		inventoryCache.items = new SortedDictionary<ulong, KleiItems.Item>();
		foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
		{
			inventoryCache.items[item.ItemId] = item;
		}
		inventoryCache.checksum = KleiItems.hash(text, 0U);
		foreach (KeyValuePair<ulong, KleiItems.Item> keyValuePair in inventoryCache.items)
		{
			KleiItems.Item value = keyValuePair.Value;
			inventoryCache.checksum = KleiItems.hash(value.ItemId.ToString(), inventoryCache.checksum);
			inventoryCache.checksum = KleiItems.hash(value.ItemType, inventoryCache.checksum);
			inventoryCache.checksum = KleiItems.hash(value.IsOpened.ToString(), inventoryCache.checksum);
		}
		string file_path = this.GetCachePath(text);
		string data = JsonConvert.SerializeObject(inventoryCache);
		FileUtil.DoIODialog(delegate
		{
			using (FileStream fileStream = File.Open(file_path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				byte[] bytes = new ASCIIEncoding().GetBytes(data);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}, file_path, 0);
	}

	public void LoadInventoryCache()
	{
		if (!DistributionPlatform.Inst.Initialized)
		{
			return;
		}
		string userId = DistributionPlatform.Inst.LocalUser.Id.ToString();
		string file_path = this.GetCachePath(userId);
		if (File.Exists(file_path))
		{
			FileUtil.DoIODialog(delegate
			{
				KleiItems.InventoryData.AllItems.Clear();
				KleiItems.InventoryData.ItemsByType.Clear();
				string text = File.ReadAllText(file_path);
				KleiItems.InventoryCache inventoryCache;
				try
				{
					inventoryCache = JsonConvert.DeserializeObject<KleiItems.InventoryCache>(text);
				}
				catch (JsonSerializationException)
				{
					return;
				}
				uint num = KleiItems.hash(userId, 0U);
				foreach (KeyValuePair<ulong, KleiItems.Item> keyValuePair in inventoryCache.items)
				{
					KleiItems.Item value = keyValuePair.Value;
					num = KleiItems.hash(value.ItemId.ToString(), num);
					num = KleiItems.hash(value.ItemType, num);
					num = KleiItems.hash(value.IsOpened.ToString(), num);
				}
				if (num != inventoryCache.checksum)
				{
					inventoryCache.items.Clear();
				}
				foreach (KeyValuePair<ulong, KleiItems.Item> keyValuePair2 in inventoryCache.items)
				{
					KleiItems.Item value2 = keyValuePair2.Value;
					KleiItems.InventoryData.AllItems.Add(value2);
					List<KleiItems.Item> list;
					if (KleiItems.InventoryData.ItemsByType.TryGetValue(value2.ItemType, out list))
					{
						list.Add(value2);
					}
					else
					{
						KleiItems.InventoryData.ItemsByType[value2.ItemType] = new List<KleiItems.Item> { value2 };
					}
				}
			}, file_path, 0);
		}
	}

	private static KleiItems.Inventory InventoryData;

	private List<KleiItems.Request> Requests = new List<KleiItems.Request>();

	private bool RequestStarted;

	private bool RequestCompleted;

	private KleiItems.Request ActiveRequest;

	private string Response;

	private bool WaitForReAuthentication;

	private const float SECONDS_PER_TICK = 360f;

	private float TimeToNextTick;

	private List<KleiItems.InventoryRefreshCallback> InventoryRefreshCbs = new List<KleiItems.InventoryRefreshCallback>();

	private List<KleiItems.UserRewardInfoReceivedCallback> UserRewardsInfoReceivedCbs = new List<KleiItems.UserRewardInfoReceivedCallback>();

	public struct ItemData
	{
		public string PermitId;

		public ulong ItemId;

		public bool IsOpened;
	}

	public delegate void InventoryRefreshCallback();

	public delegate void UserRewardInfoReceivedCallback();

	private struct Item
	{
		public string ItemType;

		public ulong ItemId;

		public bool IsOpened;
	}

	private struct Inventory
	{
		public List<KleiItems.Item> AllItems;

		public Dictionary<string, List<KleiItems.Item>> ItemsByType;

		public List<string> UnclaimedRewards;
	}

	private struct Request
	{
		public KleiItems.Request.RequestType Type;

		public object Data;

		public enum RequestType
		{
			GetAllItems,
			SetItemOpened,
			Tick,
			GetUserRewardsInfo
		}
	}

	private struct InventoryReply
	{
		public bool Error;

		public string ErrorCode;

		public KleiItems.InventoryReply.Item[] Items;

		public struct Item
		{
			public ulong ItemID;

			public string ItemType;

			public int Context;
		}
	}

	private struct SetItemOpenedReply
	{
		public bool Error;

		public string ErrorCode;
	}

	private struct TickReply
	{
		public bool Error;

		public string ErrorCode;

		public bool GiftReceived;
	}

	private struct RewardsInfoReply
	{
		public bool Error;

		public string ErrorCode;

		public Dictionary<string, KleiItems.RewardsInfoReply.Info> ItemRewards;

		public struct Info
		{
			public bool Claimed;
		}
	}

	private struct InventoryCache
	{
		public SortedDictionary<ulong, KleiItems.Item> items;

		public uint checksum;
	}

	private static class ItemLogger
	{
		[Conditional("UNITY_EDITOR")]
		public static void LogInfo(string header, string payload)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogRequest(KleiItems.Request.RequestType service, string payload)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogResponse(KleiItems.Request.RequestType service, string response)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogError(KleiItems.Request.RequestType service, string errorCode)
		{
		}
	}
}
