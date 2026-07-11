using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchListStatusItemUpdater : KMonoBehaviour, IRender1000ms
{
	public static void DestroyInstance()
	{
		FetchListStatusItemUpdater.instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		FetchListStatusItemUpdater.instance = this;
	}

	public void AddFetchList(FetchList2 fetch_list)
	{
		this.fetchLists.Add(fetch_list);
	}

	public void RemoveFetchList(FetchList2 fetch_list)
	{
		this.fetchLists.Remove(fetch_list);
	}

	public void Render1000ms(float dt)
	{
		DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary = DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.Allocate();
		foreach (FetchList2 fetchList in this.fetchLists)
		{
			if (!(fetchList.Destination == null))
			{
				ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList pooledList = null;
				int instanceID = fetchList.Destination.GetInstanceID();
				if (!pooledDictionary.TryGetValue(instanceID, out pooledList))
				{
					pooledList = ListPool<FetchList2, FetchListStatusItemUpdater>.Allocate();
					pooledDictionary[instanceID] = pooledList;
				}
				pooledList.Add(fetchList);
			}
		}
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary3 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		foreach (KeyValuePair<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList> keyValuePair in pooledDictionary)
		{
			ListPool<Tag, FetchListStatusItemUpdater>.PooledList pooledList2 = ListPool<Tag, FetchListStatusItemUpdater>.Allocate();
			Storage destination = keyValuePair.Value[0].Destination;
			foreach (FetchList2 fetchList2 in keyValuePair.Value)
			{
				fetchList2.UpdateRemaining();
				Dictionary<Tag, float> remaining = fetchList2.GetRemaining();
				foreach (KeyValuePair<Tag, float> keyValuePair2 in remaining)
				{
					if (!pooledList2.Contains(keyValuePair2.Key))
					{
						pooledList2.Add(keyValuePair2.Key);
					}
				}
			}
			ListPool<Pickupable, FetchListStatusItemUpdater>.PooledList pooledList3 = ListPool<Pickupable, FetchListStatusItemUpdater>.Allocate();
			foreach (GameObject gameObject in destination.items)
			{
				if (!(gameObject == null))
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if (!(component == null))
					{
						pooledList3.Add(component);
					}
				}
			}
			DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary4 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
			foreach (Tag tag in pooledList2)
			{
				float num = 0f;
				foreach (Pickupable pickupable in pooledList3)
				{
					if (pickupable.KPrefabID.HasTag(tag))
					{
						num += pickupable.TotalAmount;
					}
				}
				pooledDictionary4[tag] = num;
			}
			foreach (Tag tag2 in pooledList2)
			{
				if (!pooledDictionary2.ContainsKey(tag2))
				{
					pooledDictionary2[tag2] = WorldInventory.Instance.GetTotalAmount(tag2);
				}
				if (!pooledDictionary3.ContainsKey(tag2))
				{
					pooledDictionary3[tag2] = WorldInventory.Instance.GetAmount(tag2);
				}
			}
			foreach (FetchList2 fetchList3 in keyValuePair.Value)
			{
				bool flag = false;
				bool flag2 = true;
				bool flag3 = false;
				Dictionary<Tag, float> remaining2 = fetchList3.GetRemaining();
				foreach (KeyValuePair<Tag, float> keyValuePair3 in remaining2)
				{
					Tag key = keyValuePair3.Key;
					float value = keyValuePair3.Value;
					float num2 = pooledDictionary4[key];
					float num3 = pooledDictionary2[key];
					float num4 = pooledDictionary3[key];
					float num5 = Mathf.Min(value, num3);
					float num6 = num4 + num5;
					float minimumAmount = fetchList3.GetMinimumAmount(key);
					if (num2 + num6 < minimumAmount)
					{
						flag = true;
					}
					if (num6 < value)
					{
						flag2 = false;
					}
					if (num2 + num6 > value && value > num6)
					{
						flag3 = true;
					}
				}
				fetchList3.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref fetchList3.waitingForMaterialsHandle, flag2);
				fetchList3.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref fetchList3.materialsUnavailableHandle, flag);
				fetchList3.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref fetchList3.materialsUnavailableForRefillHandle, flag3);
			}
			pooledDictionary4.Recycle();
			pooledList3.Recycle();
			pooledList2.Recycle();
			keyValuePair.Value.Recycle();
		}
		pooledDictionary3.Recycle();
		pooledDictionary2.Recycle();
		pooledDictionary.Recycle();
	}

	public static FetchListStatusItemUpdater instance;

	private List<FetchList2> fetchLists = new List<FetchList2>();
}
