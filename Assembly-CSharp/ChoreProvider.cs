using System;
using System.Collections.Generic;

[SkipSaveFileSerialization]
public class ChoreProvider : KMonoBehaviour
{
	public IEnumerator<Chore> GetEnumerator()
	{
		return this.chores.GetEnumerator();
	}

	public virtual Chore AddChore(Chore chore)
	{
		chore.provider = this;
		this.chores.Add(chore);
		return chore;
	}

	public virtual Chore RemoveChore(Chore chore)
	{
		if (chore == null)
		{
			return null;
		}
		this.chores.Remove(chore);
		return chore;
	}

	public void CollectChores(ChoreConsumer chore_consumer, List<Chore.Precondition.Context> contexts)
	{
		for (int i = 0; i < this.chores.Count; i++)
		{
			Chore chore = this.chores[i];
			chore.CollectChores(chore_consumer, contexts, false);
		}
	}

	public List<Chore> chores = new List<Chore>();
}
