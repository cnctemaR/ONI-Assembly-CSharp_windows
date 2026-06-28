using System;
using System.Collections.Generic;
using System.Diagnostics;

public class FetchManagerUpdater
{
	public static void UpdatePickups(PathProber path_prober, List<Pickupable> pickupables, Worker worker)
	{
		if (pickupables.Count > FetchManagerUpdater.Pickups.Length)
		{
			FetchManagerUpdater.Pickups = new FetchManagerUpdater.Pickup[pickupables.Count];
		}
		FetchManagerUpdater.PickupCount = 0;
		Navigator component = worker.GetComponent<Navigator>();
		pickupables.RemoveAll((Pickupable x) => x == null);
		for (int i = 0; i < pickupables.Count; i++)
		{
			Pickupable pickupable = pickupables[i];
			if (pickupable.CouldBePickedUp(component.gameObject))
			{
				int navigationCost = pickupable.GetNavigationCost(component, pickupable.cachedCell);
				if (navigationCost != PathProber.InvalidCost)
				{
					FetchManagerUpdater.Pickup pickup = default(FetchManagerUpdater.Pickup);
					pickup.Pickupable = pickupable;
					pickup.PrefabTagHash = pickupable.KPrefabID.PrefabTag.GetHash();
					pickup.PathCost = (ushort)navigationCost;
					pickup.masterPriority = 0;
					pickup.freshness = 0;
					if (pickupable.storage != null)
					{
						Prioritizable prioritizable = pickupable.storage.prioritizable;
						if (prioritizable != null)
						{
							pickup.masterPriority = prioritizable.GetMasterPriority().priority_value;
						}
					}
					Rottable.Instance rottable = pickupable.rottable;
					if (rottable != null)
					{
						pickup.freshness = (int)rottable.RotValue;
					}
					else
					{
						pickup.freshness = int.MaxValue;
					}
					FetchManagerUpdater.Pickups[FetchManagerUpdater.PickupCount++] = pickup;
				}
			}
		}
		Array.Sort<FetchManagerUpdater.Pickup>(FetchManagerUpdater.Pickups, 0, FetchManagerUpdater.PickupCount, FetchManagerUpdater.Comparer);
		int num = FetchManagerUpdater.PickupCount;
		int num2 = 0;
		for (int j = 1; j < FetchManagerUpdater.PickupCount; j++)
		{
			if (FetchManagerUpdater.IsBetter(ref FetchManagerUpdater.Pickups[num2], ref FetchManagerUpdater.Pickups[j]))
			{
				num--;
			}
			else
			{
				num2++;
				if (j > num2)
				{
					FetchManagerUpdater.Pickups[num2] = FetchManagerUpdater.Pickups[j];
				}
			}
		}
		FetchManagerUpdater.PickupCount = num;
	}

	private static bool IsBetter(ref FetchManagerUpdater.Pickup a, ref FetchManagerUpdater.Pickup b)
	{
		bool flag = a.PathCost <= b.PathCost;
		bool flag2 = a.Pickupable.KPrefabID.GetTabBits().AreEqual(b.Pickupable.KPrefabID.GetTabBits());
		bool flag3 = a.masterPriority == b.masterPriority;
		bool flag4 = a.freshness == b.freshness;
		return flag && flag2 && flag3 && flag4;
	}

	public static bool IsFetchablePickup(KPrefabID pickup_id, Storage source, float pickup_unreserved_amount, float pickup_min_unit, float maximum_requested, TagBits tag_bits, Tag[] required_tags, Tag[] forbid_tags, Storage destination)
	{
		if (pickup_id == null)
		{
			return false;
		}
		if (required_tags != null)
		{
			foreach (Tag tag in required_tags)
			{
				if (!pickup_id.HasTag(tag))
				{
					return false;
				}
			}
		}
		if (forbid_tags != null)
		{
			foreach (Tag tag2 in forbid_tags)
			{
				if (pickup_id.HasTag(tag2))
				{
					return false;
				}
			}
		}
		if (source != null && destination.allowItemRemoval)
		{
			int num = 10;
			if (destination.prioritizable != null)
			{
				num = destination.prioritizable.GetMasterPriority().priority_value;
			}
			int num2 = 10;
			if (source.prioritizable != null)
			{
				num2 = source.prioritizable.GetMasterPriority().priority_value;
			}
			if (num <= num2)
			{
				return false;
			}
		}
		return pickup_id.GetTabBits().HasAny(tag_bits) && pickup_unreserved_amount > 0f;
	}

	public static void FindFetchTarget(Worker worker, Storage destination, List<Pickupable> pickupables, TagBits tag_bits, Tag[] required_tags, Tag[] forbid_tags, float required_amount, ref Pickupable workable)
	{
		workable = null;
		int num = int.MaxValue;
		for (int i = 0; i < FetchManagerUpdater.PickupCount; i++)
		{
			FetchManagerUpdater.Pickup pickup = FetchManagerUpdater.Pickups[i];
			bool flag = FetchManagerUpdater.IsFetchablePickup(pickup.Pickupable.KPrefabID, pickup.Pickupable.storage, pickup.Pickupable.UnreservedAmount, pickup.Pickupable.MinTakeAmount, required_amount, tag_bits, required_tags, forbid_tags, destination);
			if (flag && (int)pickup.PathCost < num)
			{
				workable = pickup.Pickupable;
				num = (int)pickup.PathCost;
			}
		}
	}

	public static void FreeResources()
	{
		FetchManagerUpdater.Pickups = new FetchManagerUpdater.Pickup[128];
	}

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	private static FetchManagerUpdater.Pickup[] Pickups = new FetchManagerUpdater.Pickup[128];

	private static int PickupCount;

	private static FetchManagerUpdater.PickupComparer Comparer = new FetchManagerUpdater.PickupComparer();

	[DebuggerDisplay("{Pickupable.gameObject.name}")]
	private struct Pickup
	{
		public Pickupable Pickupable;

		public int PrefabTagHash;

		public ushort PathCost;

		public int masterPriority;

		public int freshness;
	}

	private class PickupComparer : IComparer<FetchManagerUpdater.Pickup>
	{
		public int Compare(FetchManagerUpdater.Pickup a, FetchManagerUpdater.Pickup b)
		{
			int num = a.PrefabTagHash - b.PrefabTagHash;
			if (num != 0)
			{
				return num;
			}
			if (a.masterPriority != b.masterPriority)
			{
				return a.masterPriority - b.masterPriority;
			}
			if (a.PathCost != b.PathCost)
			{
				return (int)(a.PathCost - b.PathCost);
			}
			return a.freshness - b.freshness;
		}
	}
}
