using System;
using System.Collections.Generic;

[SkipSaveFileSerialization]
public class ChoreProvider : KMonoBehaviour
{
	public string Name { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Name = base.name;
	}

	public virtual void AddChore(Chore chore)
	{
		chore.provider = this;
		this.chores.Add(chore);
	}

	public virtual void RemoveChore(Chore chore)
	{
		if (chore == null)
		{
			return;
		}
		chore.provider = null;
		this.chores.Remove(chore);
	}

	public virtual void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		for (int i = 0; i < this.chores.Count; i++)
		{
			Chore chore = this.chores[i];
			chore.CollectChores(consumer_state, succeeded, failed_contexts, false);
		}
	}

	public List<Chore> chores = new List<Chore>();
}
