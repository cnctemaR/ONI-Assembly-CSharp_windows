using System;
using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider, ISim200ms, IRender200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GlobalChoreProvider.Instance = this;
		this.clearableManager = new ClearableManager();
	}

	public override void AddChore(Chore chore)
	{
		base.AddChore(chore);
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			this.fetchChores.Add(fetchChore);
		}
	}

	public override void RemoveChore(Chore chore)
	{
		base.RemoveChore(chore);
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			this.fetchChores.Remove(fetchChore);
		}
	}

	public void UpdateFetches(PathProber path_prober)
	{
		this.fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		foreach (FetchChore fetchChore in this.fetchChores)
		{
			if (!(fetchChore.driver != null) && (!(fetchChore.automatable != null) || !fetchChore.automatable.GetAutomationOnly()))
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
							tagBitsHash = fetchChore.tagBitsHash,
							cost = navigationCost,
							priority = fetchChore.masterPriority,
							category = destination.fetchCategory
						});
					}
				}
			}
		}
		if (this.fetches.Count > 0)
		{
			this.fetches.Sort(GlobalChoreProvider.Comparer);
			int i = 1;
			int num = 0;
			while (i < this.fetches.Count)
			{
				if (!this.fetches[num].IsBetterThan(this.fetches[i]))
				{
					num++;
					this.fetches[num] = this.fetches[i];
				}
				i++;
			}
			this.fetches.RemoveRange(num + 1, this.fetches.Count - num - 1);
		}
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		base.CollectChores(consumer_state, succeeded, failed_contexts);
		this.clearableManager.CollectChores(consumer_state, succeeded, failed_contexts);
		foreach (GlobalChoreProvider.Fetch fetch in this.fetches)
		{
			fetch.chore.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded, failed_contexts, false);
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

	public void Sim200ms(float time_delta)
	{
		GlobalChoreProvider.find_top_priority_job.Reset(null);
		GlobalChoreProvider.FindTopPriorityTask.abort = false;
		int num = 512;
		for (int i = 0; i < Components.Prioritizables.Items.Count; i += num)
		{
			int num2 = i + num;
			if (Components.Prioritizables.Items.Count < num2)
			{
				num2 = Components.Prioritizables.Items.Count;
			}
			GlobalChoreProvider.find_top_priority_job.Add(new GlobalChoreProvider.FindTopPriorityTask(i, num2));
		}
		GlobalJobManager.Run(GlobalChoreProvider.find_top_priority_job);
		bool flag = false;
		for (int num3 = 0; num3 != GlobalChoreProvider.find_top_priority_job.Count; num3++)
		{
			if (GlobalChoreProvider.find_top_priority_job.GetWorkItem(num3).found)
			{
				flag = true;
				break;
			}
		}
		VignetteManager.Instance.Get().HasTopPriorityChore(flag);
	}

	public void Render200ms(float dt)
	{
		this.UpdateStorageFetchableBits();
	}

	private void UpdateStorageFetchableBits()
	{
		ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
		ChoreType foodFetch = Db.Get().ChoreTypes.FoodFetch;
		this.storageFetchableBits.ClearAll();
		foreach (FetchChore fetchChore in this.fetchChores)
		{
			if ((fetchChore.choreType == storageFetch || fetchChore.choreType == foodFetch) && fetchChore.destination)
			{
				int num = Grid.PosToCell(fetchChore.destination);
				if (MinionGroupProber.Get().IsReachable(num, fetchChore.destination.GetOffsets(num)))
				{
					this.storageFetchableBits.Or(ref fetchChore.tagBits);
				}
			}
		}
	}

	public bool ClearableHasDestination(Pickupable pickupable)
	{
		KPrefabID kprefabID = pickupable.KPrefabID;
		kprefabID.UpdateTagBits();
		return kprefabID.HasAnyTags_AssumeLaundered(ref this.storageFetchableBits);
	}

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();

	public List<GlobalChoreProvider.Fetch> fetches = new List<GlobalChoreProvider.Fetch>();

	private static readonly GlobalChoreProvider.FetchComparer Comparer = new GlobalChoreProvider.FetchComparer();

	private ClearableManager clearableManager;

	private TagBits storageFetchableBits;

	private static WorkItemCollection<GlobalChoreProvider.FindTopPriorityTask, object> find_top_priority_job = new WorkItemCollection<GlobalChoreProvider.FindTopPriorityTask, object>();

	public struct Fetch
	{
		public bool IsBetterThan(GlobalChoreProvider.Fetch fetch)
		{
			if (this.category != fetch.category)
			{
				return false;
			}
			if (this.tagBitsHash != fetch.tagBitsHash)
			{
				return false;
			}
			if (this.chore.choreType != fetch.chore.choreType)
			{
				return false;
			}
			if (!this.chore.tagBits.AreEqual(ref fetch.chore.tagBits))
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

		public int tagBitsHash;

		public int cost;

		public PrioritySetting priority;

		public Storage.FetchCategory category;
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
		public FindTopPriorityTask(int start, int end)
		{
			this.start = start;
			this.end = end;
			this.found = false;
		}

		public void Run(object context)
		{
			if (GlobalChoreProvider.FindTopPriorityTask.abort)
			{
				return;
			}
			for (int num = this.start; num != this.end; num++)
			{
				if (Components.Prioritizables.Items[num].IsTopPriority())
				{
					this.found = true;
					break;
				}
			}
			if (this.found)
			{
				GlobalChoreProvider.FindTopPriorityTask.abort = true;
			}
		}

		private int start;

		private int end;

		public bool found;

		public static bool abort;
	}
}
