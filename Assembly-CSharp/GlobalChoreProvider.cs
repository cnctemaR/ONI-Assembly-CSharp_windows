using System;
using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider
{
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

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();
}
