using System;
using System.Collections.Generic;

internal class ClearableManager
{
	public HandleVector<int>.Handle RegisterClearable(Clearable clearable)
	{
		return this.markedClearables.Allocate(new ClearableManager.MarkedClearable
		{
			clearable = clearable,
			pickupable = clearable.GetComponent<Pickupable>(),
			prioritizable = clearable.GetComponent<Prioritizable>()
		});
	}

	public void UnregisterClearable(HandleVector<int>.Handle handle)
	{
		this.markedClearables.Free(handle);
	}

	private static void CollectSortedClearables(Navigator navigator, KCompactedVector<ClearableManager.MarkedClearable> clearables, List<ClearableManager.SortedClearable> sorted_clearables)
	{
		sorted_clearables.Clear();
		foreach (ClearableManager.MarkedClearable markedClearable in clearables.GetDataList())
		{
			int navigationCost = markedClearable.pickupable.GetNavigationCost(navigator, markedClearable.pickupable.cachedCell);
			if (navigationCost != -1)
			{
				sorted_clearables.Add(new ClearableManager.SortedClearable
				{
					pickupable = markedClearable.pickupable,
					masterPriority = markedClearable.prioritizable.GetMasterPriority(),
					cost = navigationCost
				});
			}
		}
		sorted_clearables.Sort(ClearableManager.SortedClearable.comparer);
	}

	public void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		ChoreType transport = Db.Get().ChoreTypes.Transport;
		int personalPriority = consumer_state.consumer.GetPersonalPriority(transport);
		int num = ((!Game.Instance.advancedPersonalPriorities) ? transport.priority : transport.explicitPriority);
		ClearableManager.CollectSortedClearables(consumer_state.navigator, this.markedClearables, this.sortedClearables);
		bool flag = false;
		foreach (ClearableManager.SortedClearable sortedClearable in this.sortedClearables)
		{
			Pickupable pickupable = sortedClearable.pickupable;
			PrioritySetting masterPriority = sortedClearable.masterPriority;
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			context.personalPriority = personalPriority;
			KPrefabID kprefabID = pickupable.KPrefabID;
			kprefabID.UpdateTagBits();
			foreach (GlobalChoreProvider.Fetch fetch in GlobalChoreProvider.Instance.fetches)
			{
				bool flag2 = kprefabID.HasAnyTags_AssumeLaundered(ref fetch.chore.tagBits);
				if (flag2)
				{
					context.Set(fetch.chore, consumer_state, false, pickupable);
					context.choreTypeForPermission = transport;
					context.RunPreconditions();
					if (context.IsSuccess())
					{
						context.masterPriority = masterPriority;
						context.priority = num;
						context.interruptPriority = transport.interruptPriority;
						succeeded.Add(context);
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
	}

	private KCompactedVector<ClearableManager.MarkedClearable> markedClearables = new KCompactedVector<ClearableManager.MarkedClearable>(0);

	private List<ClearableManager.SortedClearable> sortedClearables = new List<ClearableManager.SortedClearable>();

	private struct MarkedClearable
	{
		public Clearable clearable;

		public Pickupable pickupable;

		public Prioritizable prioritizable;
	}

	private struct SortedClearable
	{
		public Pickupable pickupable;

		public PrioritySetting masterPriority;

		public int cost;

		public static ClearableManager.SortedClearable.Comparer comparer = new ClearableManager.SortedClearable.Comparer();

		public class Comparer : IComparer<ClearableManager.SortedClearable>
		{
			public int Compare(ClearableManager.SortedClearable a, ClearableManager.SortedClearable b)
			{
				int num = b.masterPriority.priority_value - a.masterPriority.priority_value;
				if (num == 0)
				{
					return a.cost - b.cost;
				}
				return num;
			}
		}
	}
}
