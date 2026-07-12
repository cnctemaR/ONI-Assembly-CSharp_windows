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
	public static IEnumerable<KleiItems.ItemData> IterateInventory(Dictionary<string, string> item_to_permit, HashSet<string> boxes)
	{
		if (KleiItems.InventoryData.AllItems != null)
		{
			int actual_item_count = 0;
			KleiItems.ItemData[] items = new KleiItems.ItemData[KleiItems.InventoryData.AllItems.Count];
			int num;
			foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
			{
				string text;
				if (item_to_permit.TryGetValue(item.ItemType, out text))
				{
					items[actual_item_count].Id = text;
					items[actual_item_count].ItemId = item.ItemId;
					items[actual_item_count].IsOpened = item.IsOpened;
					num = actual_item_count + 1;
					actual_item_count = num;
				}
				else if (boxes.Contains(item.ItemType))
				{
					items[actual_item_count].Id = item.ItemType;
					items[actual_item_count].ItemId = item.ItemId;
					items[actual_item_count].IsOpened = item.IsOpened;
					num = actual_item_count + 1;
					actual_item_count = num;
				}
			}
			for (int i = 0; i < actual_item_count; i = num)
			{
				yield return items[i];
				num = i + 1;
			}
			items = null;
		}
		yield break;
	}

	public static bool HasUnopenedItem(Dictionary<string, string> item_to_permit, HashSet<string> boxes)
	{
		if (KleiItems.InventoryData.AllItems != null)
		{
			foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
			{
				if ((item_to_permit.ContainsKey(item.ItemType) || boxes.Contains(item.ItemType)) && !item.IsOpened)
				{
					return true;
				}
			}
			return false;
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

	public static ulong GetItemInstanceID(string itemType)
	{
		if (KleiItems.InventoryData.ItemsByType == null)
		{
			global::Debug.LogError("Tried to get item from null inventory");
			return 0UL;
		}
		List<KleiItems.Item> list;
		if (KleiItems.InventoryData.ItemsByType.TryGetValue(itemType, out list))
		{
			return list[0].ItemId;
		}
		global::Debug.LogError("No instance of requested itemType found in inventory: " + itemType);
		return 0UL;
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

	public static ulong GetFilamentAmount()
	{
		return KleiItems.InventoryData.Filament;
	}

	public static bool TryGetBarterPrice(string itemType, out ulong buyPrice, out ulong sellPrice)
	{
		buyPrice = (sellPrice = 0UL);
		if (KleiItems.InventoryData.BarterPrices == null || KleiItems.InventoryData.BarterPrices.Count<KeyValuePair<string, Pair<ulong, ulong>>>() == 0)
		{
			return false;
		}
		Pair<ulong, ulong> pair;
		if (KleiItems.InventoryData.BarterPrices.TryGetValue(itemType, out pair))
		{
			buyPrice = pair.first;
			sellPrice = pair.second;
			return true;
		}
		return false;
	}

	public static float SecondsToNextTick()
	{
		return ThreadedHttps<KleiItems>.Instance.TimeToNextTick;
	}

	public static void AddRequestInventoryRefresh(KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.GetAllItems, null, cb);
	}

	public static void AddRequestItemOpened(ulong itemId, KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.SetItemOpened, itemId, cb);
	}

	public static void AddRequestMysteryBoxOpened(ulong itemId, KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.OpenMysteryBox, itemId, cb);
	}

	public static void AddRequestTick(KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.Tick, null, cb);
	}

	public static void AddInventoryRefreshCallback(KleiItems.InventoryRefreshCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.InventoryRefreshCbs.Add(cb);
	}

	public static void RemoveInventoryRefreshCallback(KleiItems.InventoryRefreshCallback cb)
	{
		ThreadedHttps<KleiItems>.Instance.InventoryRefreshCbs.Remove(cb);
	}

	public static void AddRequestGetPricingInfo(KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.GetPricingInfo, null, cb);
	}

	public static void AddRequestBarterGainItem(string itemType, KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.BarterGainItem, itemType, cb);
	}

	public static void AddRequestBarterLoseItem(ulong itemId, KleiItems.ResponseCallback cb = null)
	{
		ThreadedHttps<KleiItems>.Instance.AddRequest(KleiItems.Request.RequestType.BarterLoseItem, itemId, cb);
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
				else if (activeRequest.Type == KleiItems.Request.RequestType.OpenMysteryBox)
				{
					this.OnOpenMysteryBoxReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.Tick)
				{
					this.OnTickReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.GetPricingInfo)
				{
					this.OnGetPricingInfoReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.BarterGainItem)
				{
					this.OnBarterGainItemReply(this.Response);
				}
				else if (activeRequest.Type == KleiItems.Request.RequestType.BarterLoseItem)
				{
					this.OnBarterLoseItemReply(this.Response);
				}
			}
			else
			{
				this.HandleError(activeRequest, "NULL");
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
				flag = this.RequestItemOpened((ulong)request.Data, request.Cb);
			}
			else if (request.Type == KleiItems.Request.RequestType.OpenMysteryBox)
			{
				flag = this.RequestOpenMysteryBox((ulong)request.Data);
			}
			else if (request.Type == KleiItems.Request.RequestType.Tick)
			{
				flag = this.RequestTick();
			}
			else if (request.Type == KleiItems.Request.RequestType.GetPricingInfo)
			{
				flag = this.RequestGetPricingInfo();
			}
			else if (request.Type == KleiItems.Request.RequestType.BarterGainItem)
			{
				flag = this.RequestBarterGainItem((string)request.Data);
			}
			else if (request.Type == KleiItems.Request.RequestType.BarterLoseItem)
			{
				flag = this.RequestBarterLoseItem((ulong)request.Data);
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
			KleiItems.AddRequestTick(null);
			this.TimeToNextTick += 360f;
		}
	}

	public KleiItems()
	{
		this.serviceName = "KleiItems";
		this.TimeToNextTick = 360f;
		KleiItems.InventoryData.AllItems = new List<KleiItems.Item>();
		KleiItems.InventoryData.ItemsByType = new Dictionary<string, List<KleiItems.Item>>();
		KleiItems.InventoryData.Filament = 0UL;
		KleiItems.InventoryData.BarterPrices = new Dictionary<string, Pair<ulong, ulong>>();
		this.RetryCount = 0;
	}

	private void AddRequest(KleiItems.Request.RequestType type, object data, KleiItems.ResponseCallback cb)
	{
		KleiItems.Request request;
		request.Type = type;
		request.Data = data;
		request.Cb = cb;
		this.Requests.Add(request);
	}

	private void StartHttpsRequest(string url)
	{
		this.LIVE_ENDPOINT = url;
		this.quitOnError = false;
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

	private void HandleSuccess(KleiItems.Request req)
	{
		if (req.Cb != null)
		{
			KleiItems.Result result;
			result.Success = true;
			req.Cb(result);
		}
	}

	private void HandleError(KleiItems.Request req, string errorCode)
	{
		if (errorCode == "E_EXPIRED_TOKEN" || errorCode == "E_INVALID_TOKEN")
		{
			this.WaitForReAuthentication = true;
			this.AddRequest(req.Type, req.Data, req.Cb);
			ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.OnAuthenticateComplete), true);
			return;
		}
		if (req.Cb != null)
		{
			KleiItems.Result result;
			result.Success = false;
			req.Cb(result);
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
					item2.IsOpened = item.Context != 3 && item.Context != 4;
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
			if (inventoryReply.CurrencyMap != null)
			{
				inventoryReply.CurrencyMap.TryGetValue("FILAMENT", out KleiItems.InventoryData.Filament);
			}
			this.SaveInventoryCache();
			foreach (KleiItems.InventoryRefreshCallback inventoryRefreshCallback in this.InventoryRefreshCbs)
			{
				inventoryRefreshCallback();
			}
			this.HandleSuccess(this.ActiveRequest);
			return;
		}
		this.HandleError(this.ActiveRequest, inventoryReply.ErrorCode);
	}

	private bool RequestItemOpened(ulong itemId, KleiItems.ResponseCallback cb)
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
			KleiItems.AddRequestInventoryRefresh(this.ActiveRequest.Cb);
			return;
		}
		this.HandleError(this.ActiveRequest, setItemOpenedReply.ErrorCode);
	}

	private bool RequestOpenMysteryBox(ulong itemId)
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "clientitems/ONI/OpenMysteryBox");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object>
		{
			{ "ClientToken", kleiToken },
			{ "ItemID", itemId }
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnOpenMysteryBoxReply(string response)
	{
		KleiItems.OpenMysteryBoxReply openMysteryBoxReply = JsonConvert.DeserializeObject<KleiItems.OpenMysteryBoxReply>(response);
		if (!openMysteryBoxReply.Error)
		{
			KleiItems.AddRequestInventoryRefresh(this.ActiveRequest.Cb);
			return;
		}
		this.HandleError(this.ActiveRequest, openMysteryBoxReply.ErrorCode);
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
		if (tickReply.Error)
		{
			this.HandleError(this.ActiveRequest, tickReply.ErrorCode);
			return;
		}
		if (tickReply.GiftReceived)
		{
			KleiItems.AddRequestInventoryRefresh(this.ActiveRequest.Cb);
			return;
		}
		this.HandleSuccess(this.ActiveRequest);
	}

	private bool RequestGetPricingInfo()
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "iap/ONI/GetPricingInfo");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object>
		{
			{ "ClientToken", kleiToken },
			{ "PricingHash", 0 }
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnGetPricingInfoReply(string response)
	{
		KleiItems.GetPricingInfoReply getPricingInfoReply = JsonConvert.DeserializeObject<KleiItems.GetPricingInfoReply>(response);
		if (!getPricingInfoReply.Error)
		{
			KleiItems.InventoryData.BarterPrices.Clear();
			using (Dictionary<string, KleiItems.GetPricingInfoReply.BarterDef>.Enumerator enumerator = getPricingInfoReply.BarterDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, KleiItems.GetPricingInfoReply.BarterDef> keyValuePair = enumerator.Current;
					string key = keyValuePair.Key;
					KleiItems.GetPricingInfoReply.BarterDef value = keyValuePair.Value;
					if (value.Currency == "FILAMENT")
					{
						KleiItems.InventoryData.BarterPrices[key] = new Pair<ulong, ulong>(value.Buy, value.Sell);
					}
				}
				return;
			}
		}
		this.HandleError(this.ActiveRequest, getPricingInfoReply.ErrorCode);
	}

	private bool RequestBarterGainItem(string item_type)
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		ulong first = KleiItems.InventoryData.BarterPrices[item_type].first;
		if (first > KleiItems.InventoryData.Filament)
		{
			return false;
		}
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "iap/ONI/BarterGainItem");
		string text = JsonConvert.SerializeObject(new Dictionary<string, object>
		{
			{ "ClientToken", kleiToken },
			{ "ItemType", item_type },
			{ "ExpectedPrice", first },
			{
				"ExpectedBalance",
				KleiItems.InventoryData.Filament
			},
			{ "CurrencyName", "FILAMENT" },
			{
				"TransID",
				global::UnityEngine.Random.Range(1, int.MaxValue).ToString()
			}
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnBarterGainItemReply(string response)
	{
		KleiItems.BarterGainItemReply barterGainItemReply = JsonConvert.DeserializeObject<KleiItems.BarterGainItemReply>(response);
		if (!barterGainItemReply.Error)
		{
			if (this.ActiveRequest.Cb != null)
			{
				KleiItems.Result result;
				result.Success = true;
				this.ActiveRequest.Cb(result);
			}
			KleiItems.AddRequestInventoryRefresh(null);
			return;
		}
		this.HandleError(this.ActiveRequest, barterGainItemReply.ErrorCode);
	}

	private bool RequestBarterLoseItem(ulong item_id)
	{
		string kleiToken = KleiAccount.KleiToken;
		if (string.IsNullOrEmpty(kleiToken))
		{
			return false;
		}
		string text = null;
		foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
		{
			if (item.ItemId == item_id)
			{
				text = item.ItemType;
			}
		}
		if (text == null)
		{
			return false;
		}
		if (!KleiItems.InventoryData.BarterPrices.ContainsKey(text))
		{
			return false;
		}
		ulong second = KleiItems.InventoryData.BarterPrices[text].second;
		this.StartHttpsRequest(KleiItemsConfig.SERVER_URL + "iap/ONI/BarterLoseItem");
		string text2 = JsonConvert.SerializeObject(new Dictionary<string, object>
		{
			{ "ClientToken", kleiToken },
			{ "ItemID", item_id },
			{ "ExpectedPrice", second },
			{
				"ExpectedBalance",
				KleiItems.InventoryData.Filament
			},
			{ "CurrencyName", "FILAMENT" },
			{
				"TransID",
				global::UnityEngine.Random.Range(1, int.MaxValue).ToString()
			}
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text2);
		base.PutPacket(bytes, false);
		return true;
	}

	private void OnBarterLoseItemReply(string response)
	{
		KleiItems.BarterLoseItemReply barterLoseItemReply = JsonConvert.DeserializeObject<KleiItems.BarterLoseItemReply>(response);
		if (!barterLoseItemReply.Error)
		{
			if (this.ActiveRequest.Cb != null)
			{
				KleiItems.Result result;
				result.Success = true;
				this.ActiveRequest.Cb(result);
			}
			KleiItems.AddRequestInventoryRefresh(null);
			return;
		}
		this.HandleError(this.ActiveRequest, barterLoseItemReply.ErrorCode);
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
			if (item.IsOpened)
			{
				inventoryCache.items[item.ItemId] = item;
			}
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
				KleiItems.InventoryData.Filament = 0UL;
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

	public struct ItemData
	{
		public string Id;

		public ulong ItemId;

		public bool IsOpened;
	}

	public struct Result
	{
		public bool Success;
	}

	public delegate void ResponseCallback(KleiItems.Result r);

	public delegate void InventoryRefreshCallback();

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

		public ulong Filament;

		public Dictionary<string, Pair<ulong, ulong>> BarterPrices;
	}

	private struct Request
	{
		public KleiItems.Request.RequestType Type;

		public object Data;

		public KleiItems.ResponseCallback Cb;

		public enum RequestType
		{
			GetAllItems,
			SetItemOpened,
			OpenMysteryBox,
			Tick,
			GetPricingInfo,
			BarterLoseItem,
			BarterGainItem
		}
	}

	private struct InventoryReply
	{
		public bool Error;

		public string ErrorCode;

		public KleiItems.InventoryReply.Item[] Items;

		public Dictionary<string, ulong> CurrencyMap;

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

	private struct OpenMysteryBoxReply
	{
		public bool Error;

		public string ErrorCode;

		public KleiItems.OpenMysteryBoxReply.Item[] Items;

		public struct Item
		{
			public ulong ItemID;

			public string ItemType;

			public int Context;
		}
	}

	private struct TickReply
	{
		public bool Error;

		public string ErrorCode;

		public bool GiftReceived;
	}

	private struct GetPricingInfoReply
	{
		public bool Error;

		public string ErrorCode;

		public Dictionary<string, KleiItems.GetPricingInfoReply.BarterDef> BarterDefs;

		public struct BarterDef
		{
			public string Currency;

			public ulong Buy;

			public ulong Sell;
		}
	}

	private struct BarterGainItemReply
	{
		public bool Error;

		public string ErrorCode;

		public ulong ItemID;

		public string ItemType;

		public ulong NewCurrency;

		public string CurrencyName;
	}

	private struct BarterLoseItemReply
	{
		public bool Error;

		public string ErrorCode;

		public ulong NewCurrency;

		public string CurrencyName;
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
