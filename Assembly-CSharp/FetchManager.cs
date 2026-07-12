using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FetchManager")]
public class FetchManager : KMonoBehaviour, ISim1000ms
{
	private static int QuantizeRotValue(float rot_value)
	{
		return (int)(4f * rot_value);
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name, int count)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample(string region_name, int count)
	{
	}

	public HandleVector<int>.Handle Add(Pickupable pickupable)
	{
		Tag tag = pickupable.KPrefabID.PrefabID();
		FetchManager.FetchablesByPrefabId fetchablesByPrefabId = null;
		if (!this.prefabIdToFetchables.TryGetValue(tag, out fetchablesByPrefabId))
		{
			fetchablesByPrefabId = new FetchManager.FetchablesByPrefabId(tag);
			this.prefabIdToFetchables[tag] = fetchablesByPrefabId;
		}
		return fetchablesByPrefabId.AddPickupable(pickupable);
	}

	public void Remove(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle)
	{
		FetchManager.FetchablesByPrefabId fetchablesByPrefabId;
		if (this.prefabIdToFetchables.TryGetValue(prefab_tag, out fetchablesByPrefabId))
		{
			fetchablesByPrefabId.RemovePickupable(fetchable_handle);
		}
	}

	public void UpdateStorage(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle, Storage storage)
	{
		FetchManager.FetchablesByPrefabId fetchablesByPrefabId;
		if (this.prefabIdToFetchables.TryGetValue(prefab_tag, out fetchablesByPrefabId))
		{
			fetchablesByPrefabId.UpdateStorage(fetchable_handle, storage);
		}
	}

	public void UpdateTags(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle)
	{
		this.prefabIdToFetchables[prefab_tag].UpdateTags(fetchable_handle);
	}

	public void Sim1000ms(float dt)
	{
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> keyValuePair in this.prefabIdToFetchables)
		{
			keyValuePair.Value.Sim1000ms(dt);
		}
	}

	public void UpdatePickups(PathProber path_prober, Worker worker)
	{
		Navigator component = worker.GetComponent<Navigator>();
		this.updatePickupsWorkItems.Reset(null);
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> keyValuePair in this.prefabIdToFetchables)
		{
			FetchManager.FetchablesByPrefabId value = keyValuePair.Value;
			value.UpdateOffsetTables();
			this.updatePickupsWorkItems.Add(new FetchManager.UpdatePickupWorkItem
			{
				fetchablesByPrefabId = value,
				pathProber = path_prober,
				navigator = component,
				worker = worker.gameObject
			});
		}
		OffsetTracker.isExecutingWithinJob = true;
		GlobalJobManager.Run(this.updatePickupsWorkItems);
		OffsetTracker.isExecutingWithinJob = false;
		this.pickups.Clear();
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> keyValuePair2 in this.prefabIdToFetchables)
		{
			this.pickups.AddRange(keyValuePair2.Value.finalPickups);
		}
		this.pickups.Sort(FetchManager.ComparerNoPriority);
	}

	public static bool IsFetchablePickup(Pickupable pickup, FetchChore chore, Storage destination)
	{
		KPrefabID kprefabID = pickup.KPrefabID;
		Storage storage = pickup.storage;
		if (pickup.UnreservedAmount <= 0f)
		{
			return false;
		}
		if (kprefabID == null)
		{
			return false;
		}
		if (chore.criteria == FetchChore.MatchCriteria.MatchID && !chore.tags.Contains(kprefabID.PrefabTag))
		{
			return false;
		}
		if (chore.criteria == FetchChore.MatchCriteria.MatchTags && !kprefabID.HasTag(chore.tagsFirst))
		{
			return false;
		}
		if (chore.requiredTag.IsValid && !kprefabID.HasTag(chore.requiredTag))
		{
			return false;
		}
		if (kprefabID.HasAnyTags(chore.forbiddenTags))
		{
			return false;
		}
		if (storage != null)
		{
			if (!storage.ignoreSourcePriority && destination.ShouldOnlyTransferFromLowerPriority && destination.masterPriority <= storage.masterPriority)
			{
				return false;
			}
			if (destination.storageNetworkID != -1 && destination.storageNetworkID == storage.storageNetworkID)
			{
				return false;
			}
		}
		return true;
	}

	public static Pickupable FindFetchTarget(List<Pickupable> pickupables, Storage destination, FetchChore chore)
	{
		foreach (Pickupable pickupable in pickupables)
		{
			if (FetchManager.IsFetchablePickup(pickupable, chore, destination))
			{
				return pickupable;
			}
		}
		return null;
	}

	public Pickupable FindFetchTarget(Storage destination, FetchChore chore)
	{
		foreach (FetchManager.Pickup pickup in this.pickups)
		{
			if (FetchManager.IsFetchablePickup(pickup.pickupable, chore, destination))
			{
				return pickup.pickupable;
			}
		}
		return null;
	}

	public static bool IsFetchablePickup_Exclude(KPrefabID pickup_id, Storage source, float pickup_unreserved_amount, HashSet<Tag> exclude_tags, Tag required_tag, Storage destination)
	{
		if (pickup_unreserved_amount <= 0f)
		{
			return false;
		}
		if (pickup_id == null)
		{
			return false;
		}
		if (exclude_tags.Contains(pickup_id.PrefabTag))
		{
			return false;
		}
		if (!pickup_id.HasTag(required_tag))
		{
			return false;
		}
		if (source != null)
		{
			if (!source.ignoreSourcePriority && destination.ShouldOnlyTransferFromLowerPriority && destination.masterPriority <= source.masterPriority)
			{
				return false;
			}
			if (destination.storageNetworkID != -1 && destination.storageNetworkID == source.storageNetworkID)
			{
				return false;
			}
		}
		return true;
	}

	public Pickupable FindEdibleFetchTarget(Storage destination, HashSet<Tag> exclude_tags, Tag required_tag)
	{
		FetchManager.Pickup pickup = new FetchManager.Pickup
		{
			PathCost = ushort.MaxValue,
			foodQuality = int.MinValue
		};
		int num = int.MaxValue;
		foreach (FetchManager.Pickup pickup2 in this.pickups)
		{
			Pickupable pickupable = pickup2.pickupable;
			if (FetchManager.IsFetchablePickup_Exclude(pickupable.KPrefabID, pickupable.storage, pickupable.UnreservedAmount, exclude_tags, required_tag, destination))
			{
				int num2 = (int)pickup2.PathCost + (5 - pickup2.foodQuality) * 50;
				if (num2 < num)
				{
					pickup = pickup2;
					num = num2;
				}
			}
		}
		return pickup.pickupable;
	}

	public static TagBits disallowedTagBits = new TagBits(GameTags.Preserved);

	public static TagBits disallowedTagMask = TagBits.MakeComplement(ref FetchManager.disallowedTagBits);

	private static readonly FetchManager.PickupComparerIncludingPriority ComparerIncludingPriority = new FetchManager.PickupComparerIncludingPriority();

	private static readonly FetchManager.PickupComparerNoPriority ComparerNoPriority = new FetchManager.PickupComparerNoPriority();

	private List<FetchManager.Pickup> pickups = new List<FetchManager.Pickup>();

	public Dictionary<Tag, FetchManager.FetchablesByPrefabId> prefabIdToFetchables = new Dictionary<Tag, FetchManager.FetchablesByPrefabId>();

	private WorkItemCollection<FetchManager.UpdatePickupWorkItem, object> updatePickupsWorkItems = new WorkItemCollection<FetchManager.UpdatePickupWorkItem, object>();

	public struct Fetchable
	{
		public Pickupable pickupable;

		public int tagBitsHash;

		public int masterPriority;

		public int freshness;

		public int foodQuality;
	}

	[DebuggerDisplay("{pickupable.name}")]
	public struct Pickup
	{
		public Pickupable pickupable;

		public int tagBitsHash;

		public ushort PathCost;

		public int masterPriority;

		public int freshness;

		public int foodQuality;
	}

	private class PickupComparerIncludingPriority : IComparer<FetchManager.Pickup>
	{
		public int Compare(FetchManager.Pickup a, FetchManager.Pickup b)
		{
			int num = a.tagBitsHash.CompareTo(b.tagBitsHash);
			if (num != 0)
			{
				return num;
			}
			num = b.masterPriority.CompareTo(a.masterPriority);
			if (num != 0)
			{
				return num;
			}
			num = a.PathCost.CompareTo(b.PathCost);
			if (num != 0)
			{
				return num;
			}
			num = b.foodQuality.CompareTo(a.foodQuality);
			if (num != 0)
			{
				return num;
			}
			return b.freshness.CompareTo(a.freshness);
		}
	}

	private class PickupComparerNoPriority : IComparer<FetchManager.Pickup>
	{
		public int Compare(FetchManager.Pickup a, FetchManager.Pickup b)
		{
			int num = a.PathCost.CompareTo(b.PathCost);
			if (num != 0)
			{
				return num;
			}
			num = b.foodQuality.CompareTo(a.foodQuality);
			if (num != 0)
			{
				return num;
			}
			return b.freshness.CompareTo(a.freshness);
		}
	}

	public class FetchablesByPrefabId
	{
		public Tag prefabId { get; private set; }

		public FetchablesByPrefabId(Tag prefab_id)
		{
			this.prefabId = prefab_id;
			this.fetchables = new KCompactedVector<FetchManager.Fetchable>(0);
			this.rotUpdaters = new Dictionary<HandleVector<int>.Handle, Rottable.Instance>();
			this.finalPickups = new List<FetchManager.Pickup>();
		}

		public HandleVector<int>.Handle AddPickupable(Pickupable pickupable)
		{
			int num = 5;
			Edible component = pickupable.GetComponent<Edible>();
			if (component != null)
			{
				num = component.GetQuality();
			}
			int num2 = 0;
			if (pickupable.storage != null)
			{
				Prioritizable prioritizable = pickupable.storage.prioritizable;
				if (prioritizable != null)
				{
					num2 = prioritizable.GetMasterPriority().priority_value;
				}
			}
			Rottable.Instance smi = pickupable.GetSMI<Rottable.Instance>();
			int num3 = 0;
			if (!smi.IsNullOrStopped())
			{
				num3 = FetchManager.QuantizeRotValue(smi.RotValue);
			}
			KPrefabID kprefabID = pickupable.KPrefabID;
			TagBits tagBits = new TagBits(ref FetchManager.disallowedTagMask);
			kprefabID.AndTagBits(ref tagBits);
			HandleVector<int>.Handle handle = this.fetchables.Allocate(new FetchManager.Fetchable
			{
				pickupable = pickupable,
				foodQuality = num,
				freshness = num3,
				masterPriority = num2,
				tagBitsHash = tagBits.GetHashCode()
			});
			if (!smi.IsNullOrStopped())
			{
				this.rotUpdaters[handle] = smi;
			}
			return handle;
		}

		public void RemovePickupable(HandleVector<int>.Handle fetchable_handle)
		{
			this.fetchables.Free(fetchable_handle);
			this.rotUpdaters.Remove(fetchable_handle);
		}

		public void UpdatePickups(PathProber path_prober, Navigator worker_navigator, GameObject worker_go)
		{
			this.GatherPickupablesWhichCanBePickedUp(worker_go);
			this.GatherReachablePickups(worker_navigator);
			this.finalPickups.Sort(FetchManager.ComparerIncludingPriority);
			if (this.finalPickups.Count > 0)
			{
				FetchManager.Pickup pickup = this.finalPickups[0];
				TagBits tagBits = new TagBits(ref FetchManager.disallowedTagMask);
				pickup.pickupable.KPrefabID.AndTagBits(ref tagBits);
				int num = pickup.tagBitsHash;
				int num2 = this.finalPickups.Count;
				int num3 = 0;
				for (int i = 1; i < this.finalPickups.Count; i++)
				{
					bool flag = false;
					FetchManager.Pickup pickup2 = this.finalPickups[i];
					TagBits tagBits2 = default(TagBits);
					int tagBitsHash = pickup2.tagBitsHash;
					if (pickup.masterPriority == pickup2.masterPriority)
					{
						tagBits2 = new TagBits(ref FetchManager.disallowedTagMask);
						pickup2.pickupable.KPrefabID.AndTagBits(ref tagBits2);
						if (pickup2.tagBitsHash == num && tagBits2.AreEqual(ref tagBits))
						{
							flag = true;
						}
					}
					if (flag)
					{
						num2--;
					}
					else
					{
						num3++;
						pickup = pickup2;
						tagBits = tagBits2;
						num = tagBitsHash;
						if (i > num3)
						{
							this.finalPickups[num3] = pickup2;
						}
					}
				}
				this.finalPickups.RemoveRange(num2, this.finalPickups.Count - num2);
			}
		}

		private void GatherPickupablesWhichCanBePickedUp(GameObject worker_go)
		{
			this.pickupsWhichCanBePickedUp.Clear();
			foreach (FetchManager.Fetchable fetchable in this.fetchables.GetDataList())
			{
				Pickupable pickupable = fetchable.pickupable;
				if (pickupable.CouldBePickedUpByMinion(worker_go))
				{
					this.pickupsWhichCanBePickedUp.Add(new FetchManager.Pickup
					{
						pickupable = pickupable,
						tagBitsHash = fetchable.tagBitsHash,
						PathCost = ushort.MaxValue,
						masterPriority = fetchable.masterPriority,
						freshness = fetchable.freshness,
						foodQuality = fetchable.foodQuality
					});
				}
			}
		}

		public void UpdateOffsetTables()
		{
			foreach (FetchManager.Fetchable fetchable in this.fetchables.GetDataList())
			{
				fetchable.pickupable.GetOffsets(fetchable.pickupable.cachedCell);
			}
		}

		private void GatherReachablePickups(Navigator navigator)
		{
			this.cellCosts.Clear();
			this.finalPickups.Clear();
			foreach (FetchManager.Pickup pickup in this.pickupsWhichCanBePickedUp)
			{
				Pickupable pickupable = pickup.pickupable;
				int num = -1;
				if (!this.cellCosts.TryGetValue(pickupable.cachedCell, out num))
				{
					num = pickupable.GetNavigationCost(navigator, pickupable.cachedCell);
					this.cellCosts[pickupable.cachedCell] = num;
				}
				if (num != -1)
				{
					this.finalPickups.Add(new FetchManager.Pickup
					{
						pickupable = pickupable,
						tagBitsHash = pickup.tagBitsHash,
						PathCost = (ushort)num,
						masterPriority = pickup.masterPriority,
						freshness = pickup.freshness,
						foodQuality = pickup.foodQuality
					});
				}
			}
		}

		public void UpdateStorage(HandleVector<int>.Handle fetchable_handle, Storage storage)
		{
			FetchManager.Fetchable data = this.fetchables.GetData(fetchable_handle);
			int num = 0;
			Pickupable pickupable = data.pickupable;
			if (pickupable.storage != null)
			{
				Prioritizable prioritizable = pickupable.storage.prioritizable;
				if (prioritizable != null)
				{
					num = prioritizable.GetMasterPriority().priority_value;
				}
			}
			data.masterPriority = num;
			this.fetchables.SetData(fetchable_handle, data);
		}

		public void UpdateTags(HandleVector<int>.Handle fetchable_handle)
		{
			FetchManager.Fetchable data = this.fetchables.GetData(fetchable_handle);
			TagBits tagBits = new TagBits(ref FetchManager.disallowedTagMask);
			data.pickupable.KPrefabID.AndTagBits(ref tagBits);
			data.tagBitsHash = tagBits.GetHashCode();
			this.fetchables.SetData(fetchable_handle, data);
		}

		public void Sim1000ms(float dt)
		{
			foreach (KeyValuePair<HandleVector<int>.Handle, Rottable.Instance> keyValuePair in this.rotUpdaters)
			{
				HandleVector<int>.Handle key = keyValuePair.Key;
				Rottable.Instance value = keyValuePair.Value;
				FetchManager.Fetchable data = this.fetchables.GetData(key);
				data.freshness = FetchManager.QuantizeRotValue(value.RotValue);
				this.fetchables.SetData(key, data);
			}
		}

		public KCompactedVector<FetchManager.Fetchable> fetchables;

		public List<FetchManager.Pickup> finalPickups = new List<FetchManager.Pickup>();

		private Dictionary<HandleVector<int>.Handle, Rottable.Instance> rotUpdaters;

		private List<FetchManager.Pickup> pickupsWhichCanBePickedUp = new List<FetchManager.Pickup>();

		private Dictionary<int, int> cellCosts = new Dictionary<int, int>();
	}

	private struct UpdatePickupWorkItem : IWorkItem<object>
	{
		public void Run(object shared_data)
		{
			this.fetchablesByPrefabId.UpdatePickups(this.pathProber, this.navigator, this.worker);
		}

		public FetchManager.FetchablesByPrefabId fetchablesByPrefabId;

		public PathProber pathProber;

		public Navigator navigator;

		public GameObject worker;
	}
}
