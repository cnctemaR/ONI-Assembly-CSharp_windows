using System;
using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider, IRender200ms
{
	public static void DestroyInstance()
	{
		GlobalChoreProvider.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GlobalChoreProvider.Instance = this;
		this.clearableManager = new ClearableManager();
	}

	protected override void OnWorldRemoved(object data)
	{
		int num = (int)data;
		int parentWorldId = ClusterManager.Instance.GetWorld(num).ParentWorldId;
		List<FetchChore> list;
		if (this.fetchMap.TryGetValue(parentWorldId, out list))
		{
			base.ClearWorldChores<FetchChore>(list, num);
		}
		base.OnWorldRemoved(data);
	}

	protected override void OnWorldParentChanged(object data)
	{
		WorldParentChangedEventArgs e = data as WorldParentChangedEventArgs;
		if (e == null || e.lastParentId == 255)
		{
			return;
		}
		base.OnWorldParentChanged(data);
		List<FetchChore> list;
		if (!this.fetchMap.TryGetValue(e.lastParentId, out list))
		{
			return;
		}
		List<FetchChore> list2;
		if (!this.fetchMap.TryGetValue(e.world.ParentWorldId, out list2))
		{
			list2 = (this.fetchMap[e.world.ParentWorldId] = new List<FetchChore>());
		}
		base.TransferChores<FetchChore>(list, list2, e.world.ParentWorldId);
	}

	public override void AddChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			int myParentWorldId = fetchChore.gameObject.GetMyParentWorldId();
			List<FetchChore> list;
			if (!this.fetchMap.TryGetValue(myParentWorldId, out list))
			{
				list = (this.fetchMap[myParentWorldId] = new List<FetchChore>());
			}
			chore.provider = this;
			list.Add(fetchChore);
			return;
		}
		base.AddChore(chore);
	}

	public override void RemoveChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			int myParentWorldId = fetchChore.gameObject.GetMyParentWorldId();
			List<FetchChore> list;
			if (this.fetchMap.TryGetValue(myParentWorldId, out list))
			{
				list.Remove(fetchChore);
			}
			chore.provider = null;
			return;
		}
		base.RemoveChore(chore);
	}

	public void UpdateFetches(PathProber path_prober)
	{
		List<FetchChore> list = null;
		int myParentWorldId = path_prober.gameObject.GetMyParentWorldId();
		if (!this.fetchMap.TryGetValue(myParentWorldId, out list))
		{
			return;
		}
		this.fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			FetchChore fetchChore = list[i];
			if (!(fetchChore.driver != null) && (!(fetchChore.automatable != null) || !fetchChore.automatable.GetAutomationOnly()))
			{
				if (fetchChore.provider == null)
				{
					fetchChore.Cancel("no provider");
					list[i] = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
				}
				else
				{
					Storage destination = fetchChore.destination;
					if (!(destination == null))
					{
						int navigationCost = component.GetNavigationCost(destination);
						if (navigationCost != -1)
						{
							this.fetches.Add(new GlobalChoreProvider.Fetch
							{
								chore = fetchChore,
								idsHash = fetchChore.tagsHash,
								cost = navigationCost,
								priority = fetchChore.masterPriority,
								category = destination.fetchCategory
							});
						}
					}
				}
			}
		}
		if (this.fetches.Count > 0)
		{
			this.fetches.Sort(GlobalChoreProvider.Comparer);
			int j = 1;
			int num = 0;
			while (j < this.fetches.Count)
			{
				if (!this.fetches[num].IsBetterThan(this.fetches[j]))
				{
					num++;
					this.fetches[num] = this.fetches[j];
				}
				j++;
			}
			this.fetches.RemoveRange(num + 1, this.fetches.Count - num - 1);
		}
		this.clearableManager.CollectAndSortClearables(component);
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		base.CollectChores(consumer_state, succeeded, failed_contexts);
		this.clearableManager.CollectChores(this.fetches, consumer_state, succeeded, failed_contexts);
		int num = CPUBudget.coreCount * 4;
		if (this.fetches.Count > num)
		{
			GlobalChoreProvider.batch_fetch_collector.Reset(this);
			int coreCount = CPUBudget.coreCount;
			int num2 = Math.Min(16, this.fetches.Count / coreCount);
			for (int i = 0; i < this.fetches.Count; i += num2)
			{
				GlobalChoreProvider.batch_fetch_collector.Add(new GlobalChoreProvider.FetchChoreCollectTask(i, Math.Min(i + num2, this.fetches.Count), consumer_state));
			}
			GlobalJobManager.Run(GlobalChoreProvider.batch_fetch_collector);
			for (int j = 0; j < coreCount; j++)
			{
				GlobalChoreProvider.batch_fetch_collector.GetWorkItem(j).Finish(succeeded, failed_contexts);
			}
			return;
		}
		for (int k = 0; k < this.fetches.Count; k++)
		{
			this.fetches[k].chore.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded, failed_contexts, false);
		}
	}

	public HandleVector<int>.Handle RegisterClearable(Clearable clearable)
	{
		return this.clearableManager.RegisterClearable(clearable);
	}

	public void UnregisterClearable(HandleVector<int>.Handle handle)
	{
		this.clearableManager.UnregisterClearable(handle);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		GlobalChoreProvider.Instance = null;
	}

	public void Render200ms(float dt)
	{
		this.UpdateStorageFetchableBits();
	}

	private void UpdateStorageFetchableBits()
	{
		ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
		ChoreType foodFetch = Db.Get().ChoreTypes.FoodFetch;
		this.storageFetchableTags.Clear();
		List<int> worldIDsSorted = ClusterManager.Instance.GetWorldIDsSorted();
		for (int i = 0; i < worldIDsSorted.Count; i++)
		{
			List<FetchChore> list;
			if (this.fetchMap.TryGetValue(worldIDsSorted[i], out list))
			{
				for (int j = 0; j < list.Count; j++)
				{
					FetchChore fetchChore = list[j];
					if ((fetchChore.choreType == storageFetch || fetchChore.choreType == foodFetch) && fetchChore.destination)
					{
						int num = Grid.PosToCell(fetchChore.destination);
						if (MinionGroupProber.Get().IsReachable(num, fetchChore.destination.GetOffsets(num)))
						{
							this.storageFetchableTags.UnionWith(fetchChore.tags);
						}
					}
				}
			}
		}
	}

	public bool ClearableHasDestination(Pickupable pickupable)
	{
		KPrefabID kprefabID = pickupable.KPrefabID;
		return this.storageFetchableTags.Contains(kprefabID.PrefabTag);
	}

	public static GlobalChoreProvider Instance;

	public Dictionary<int, List<FetchChore>> fetchMap = new Dictionary<int, List<FetchChore>>();

	public List<GlobalChoreProvider.Fetch> fetches = new List<GlobalChoreProvider.Fetch>();

	private static readonly GlobalChoreProvider.FetchComparer Comparer = new GlobalChoreProvider.FetchComparer();

	private ClearableManager clearableManager;

	private HashSet<Tag> storageFetchableTags = new HashSet<Tag>();

	private static WorkItemCollection<GlobalChoreProvider.FetchChoreCollectTask, GlobalChoreProvider> batch_fetch_collector = new WorkItemCollection<GlobalChoreProvider.FetchChoreCollectTask, GlobalChoreProvider>();

	public struct Fetch
	{
		public bool IsBetterThan(GlobalChoreProvider.Fetch fetch)
		{
			if (this.category != fetch.category)
			{
				return false;
			}
			if (this.idsHash != fetch.idsHash)
			{
				return false;
			}
			if (this.chore.choreType != fetch.chore.choreType)
			{
				return false;
			}
			if (this.priority.priority_class > fetch.priority.priority_class)
			{
				return true;
			}
			if (this.priority.priority_class == fetch.priority.priority_class)
			{
				if (this.priority.priority_value > fetch.priority.priority_value)
				{
					return true;
				}
				if (this.priority.priority_value == fetch.priority.priority_value)
				{
					return this.cost <= fetch.cost;
				}
			}
			return false;
		}

		public FetchChore chore;

		public int idsHash;

		public int cost;

		public PrioritySetting priority;

		public Storage.FetchCategory category;
	}

	private struct FetchChoreCollectTask : IWorkItem<GlobalChoreProvider>
	{
		public FetchChoreCollectTask(int start, int end, ChoreConsumerState consumer_state)
		{
			this.start = start;
			this.end = end;
			this.consumer_state = consumer_state;
			this.succeeded = ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.Allocate();
			this.failed = ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.Allocate();
			this.incomplete = ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.Allocate();
		}

		public void Run(GlobalChoreProvider context)
		{
			for (int i = this.start; i < this.end; i++)
			{
				context.fetches[i].chore.CollectChoresFromGlobalChoreProvider(this.consumer_state, this.succeeded, this.incomplete, this.failed, false);
			}
		}

		public void Finish(List<Chore.Precondition.Context> combined_succeeded, List<Chore.Precondition.Context> combined_failed)
		{
			combined_succeeded.AddRange(this.succeeded);
			this.succeeded.Clear();
			this.succeeded.Recycle();
			combined_failed.AddRange(this.failed);
			this.failed.Clear();
			this.failed.Recycle();
			foreach (Chore.Precondition.Context context in this.incomplete)
			{
				context.FinishPreconditions();
				if (context.IsSuccess())
				{
					combined_succeeded.Add(context);
				}
				else
				{
					combined_failed.Add(context);
				}
			}
			this.incomplete.Clear();
			this.incomplete.Recycle();
		}

		private int start;

		private int end;

		private ChoreConsumerState consumer_state;

		public ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.PooledList succeeded;

		public ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.PooledList failed;

		public ListPool<Chore.Precondition.Context, GlobalChoreProvider.FetchChoreCollectTask>.PooledList incomplete;
	}

	private class FetchComparer : IComparer<GlobalChoreProvider.Fetch>
	{
		public int Compare(GlobalChoreProvider.Fetch a, GlobalChoreProvider.Fetch b)
		{
			int num = b.priority.priority_class - a.priority.priority_class;
			if (num != 0)
			{
				return num;
			}
			int num2 = b.priority.priority_value - a.priority.priority_value;
			if (num2 != 0)
			{
				return num2;
			}
			return a.cost - b.cost;
		}
	}

	private struct FindTopPriorityTask : IWorkItem<object>
	{
		public FindTopPriorityTask(int start, int end, List<Prioritizable> worldCollection)
		{
			this.start = start;
			this.end = end;
			this.worldCollection = worldCollection;
			this.found = false;
		}

		public void Run(object context)
		{
			if (GlobalChoreProvider.FindTopPriorityTask.abort)
			{
				return;
			}
			int num = this.start;
			while (num != this.end && this.worldCollection.Count > num)
			{
				if (!(this.worldCollection[num] == null) && this.worldCollection[num].IsTopPriority())
				{
					this.found = true;
					break;
				}
				num++;
			}
			if (this.found)
			{
				GlobalChoreProvider.FindTopPriorityTask.abort = true;
			}
		}

		private int start;

		private int end;

		private List<Prioritizable> worldCollection;

		public bool found;

		public static bool abort;
	}
}
