using System;
using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GlobalChoreProvider.Instance = this;
		this.clearableManager = new ClearableManager();
	}

	public override Chore AddChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			this.fetchChores.Add(fetchChore);
		}
		return base.AddChore(chore);
	}

	public override Chore RemoveChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			this.fetchChores.Remove(fetchChore);
		}
		return base.RemoveChore(chore);
	}

	public void UpdateFetches(PathProber path_prober)
	{
		this.fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		foreach (FetchChore fetchChore in this.fetchChores)
		{
			int num = -1;
			if (fetchChore.destination != null)
			{
				if (fetchChore.automatable != null && fetchChore.automatable.GetAutomationOnly())
				{
					continue;
				}
				num = component.GetNavigationCost(fetchChore.destination);
			}
			if (num != -1)
			{
				if (!(fetchChore.driver != null))
				{
					this.fetches.Add(new GlobalChoreProvider.Fetch
					{
						chore = fetchChore,
						tagBitsHash = fetchChore.tagBitsHash,
						cost = num,
						priority = fetchChore.masterPriority,
						category = fetchChore.destination.fetchCategory
					});
				}
			}
		}
		if (this.fetches.Count > 0)
		{
			this.fetches.Sort(GlobalChoreProvider.Comparer);
			int i = 1;
			int num2 = 0;
			while (i < this.fetches.Count)
			{
				if (!this.fetches[num2].IsBetterThan(this.fetches[i]))
				{
					num2++;
					this.fetches[num2] = this.fetches[i];
				}
				i++;
			}
			this.fetches.RemoveRange(num2 + 1, this.fetches.Count - num2 - 1);
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

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();

	public List<GlobalChoreProvider.Fetch> fetches = new List<GlobalChoreProvider.Fetch>();

	private static readonly GlobalChoreProvider.FetchComparer Comparer = new GlobalChoreProvider.FetchComparer();

	private ClearableManager clearableManager;

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
}
