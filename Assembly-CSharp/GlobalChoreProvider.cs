using System;
using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider
{
	public GlobalChoreProvider.Fetch[] fetches { get; private set; }

	public int fetchCount { get; private set; }

	protected override void OnPrefabInit()
	{
		GlobalChoreProvider.Instance = this;
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
		if (this.fetches == null || this.fetches.Length < this.fetchChores.Count)
		{
			this.fetches = new GlobalChoreProvider.Fetch[this.fetchChores.Count * 2];
		}
		this.fetchCount = 0;
		for (int i = 0; i < this.fetchChores.Count; i++)
		{
			FetchChore fetchChore = this.fetchChores[i];
			int num = PathProber.InvalidCost;
			if (fetchChore.destination != null)
			{
				num = path_prober.GetCost(Grid.PosToCell(fetchChore.destination));
			}
			if (num != PathProber.InvalidCost)
			{
				GlobalChoreProvider.Fetch fetch = default(GlobalChoreProvider.Fetch);
				fetch.chore = this.fetchChores[i];
				fetch.tags = this.fetchChores[i].tags;
				fetch.cost = num;
				fetch.priority = fetchChore.masterPriority;
				this.fetches[this.fetchCount] = fetch;
				this.fetchCount++;
			}
		}
		if (this.fetchCount > 0)
		{
			Array.Sort<GlobalChoreProvider.Fetch>(this.fetches, 0, this.fetchCount, GlobalChoreProvider.Comparer);
			int j = 1;
			int num2 = 0;
			while (j < this.fetchCount)
			{
				if (!this.fetches[num2].IsBetterThan(this.fetches[j]))
				{
					num2++;
					this.fetches[num2] = this.fetches[j];
				}
				j++;
			}
			this.fetchCount = num2 + 1;
		}
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		GlobalChoreProvider.Instance = null;
	}

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();

	private static GlobalChoreProvider.FetchComparer Comparer = new GlobalChoreProvider.FetchComparer();

	public struct Fetch
	{
		public bool IsBetterThan(GlobalChoreProvider.Fetch fetch)
		{
			bool flag = this.priority.priority_class > fetch.priority.priority_class || (this.priority.priority_class == fetch.priority.priority_class && this.priority.priority_value > fetch.priority.priority_value);
			bool flag2 = this.cost <= fetch.cost;
			bool flag3 = true;
			if (this.tags.Length == fetch.tags.Length)
			{
				for (int i = 0; i < this.tags.Length; i++)
				{
					Tag tag = this.tags[i];
					flag3 = false;
					for (int j = 0; j < fetch.tags.Length; j++)
					{
						Tag tag2 = fetch.tags[j];
						if (tag == tag2)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						break;
					}
				}
			}
			else
			{
				flag3 = false;
			}
			return flag && flag2 && flag3;
		}

		public FetchChore chore;

		public Tag[] tags;

		public int cost;

		public PrioritySetting priority;
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
