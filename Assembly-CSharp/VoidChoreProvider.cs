using System;
using System.Collections.Generic;

public class VoidChoreProvider : ChoreProvider
{
	public static void DestroyInstance()
	{
		VoidChoreProvider.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		VoidChoreProvider.Instance = this;
	}

	public override void AddChore(Chore chore)
	{
	}

	public override void RemoveChore(Chore chore)
	{
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
	}

	public static VoidChoreProvider Instance;
}
