using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ChoreProvider")]
public class ChoreProvider : KMonoBehaviour
{
	public string Name { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game.Instance.Subscribe(880851192, new Action<object>(this.OnWorldParentChanged));
		Game.Instance.Subscribe(586301400, new Action<object>(this.OnMinionMigrated));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Name = base.name;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(880851192, new Action<object>(this.OnWorldParentChanged));
		Game.Instance.Unsubscribe(586301400, new Action<object>(this.OnMinionMigrated));
	}

	protected virtual void OnWorldParentChanged(object data)
	{
		WorldParentChangedEventArgs e = data as WorldParentChangedEventArgs;
		List<Chore> list;
		if (e == null || e.lastParentId == (int)ClusterManager.INVALID_WORLD_IDX || e.lastParentId == e.world.ParentWorldId || !this.choreWorldMap.TryGetValue(e.lastParentId, out list))
		{
			return;
		}
		List<Chore> list2;
		if (!this.choreWorldMap.TryGetValue(e.world.ParentWorldId, out list2))
		{
			list2 = (this.choreWorldMap[e.world.ParentWorldId] = new List<Chore>());
		}
		this.TransferChores<Chore>(list, list2, e.world.ParentWorldId);
	}

	protected virtual void OnMinionMigrated(object data)
	{
		MinionMigrationEventArgs e = data as MinionMigrationEventArgs;
		List<Chore> list;
		if (e == null || !(e.minionId.gameObject == base.gameObject) || e.prevWorldId == e.targetWorldId || !this.choreWorldMap.TryGetValue(e.prevWorldId, out list))
		{
			return;
		}
		List<Chore> list2;
		if (!this.choreWorldMap.TryGetValue(e.targetWorldId, out list2))
		{
			list2 = (this.choreWorldMap[e.targetWorldId] = new List<Chore>());
		}
		this.TransferChores<Chore>(list, list2, e.targetWorldId);
	}

	protected void TransferChores<T>(List<T> oldChores, List<T> newChores, int transferId) where T : Chore
	{
		int num = oldChores.Count - 1;
		for (int i = num; i >= 0; i--)
		{
			T t = oldChores[i];
			if (t.isNull)
			{
				DebugUtil.DevLogError(string.Concat(new string[]
				{
					"[",
					t.GetType().Name,
					"] ",
					t.GetReportName(null),
					" has no target"
				}));
			}
			else if (t.gameObject.GetMyParentWorldId() == transferId)
			{
				newChores.Add(t);
				oldChores[i] = oldChores[num];
				oldChores.RemoveAt(num--);
			}
		}
	}

	public virtual void AddChore(Chore chore)
	{
		chore.provider = this;
		List<Chore> list = null;
		int myParentWorldId = chore.gameObject.GetMyParentWorldId();
		if (!this.choreWorldMap.TryGetValue(myParentWorldId, out list))
		{
			list = (this.choreWorldMap[myParentWorldId] = new List<Chore>());
		}
		list.Add(chore);
	}

	public virtual void RemoveChore(Chore chore)
	{
		if (chore == null)
		{
			return;
		}
		chore.provider = null;
		List<Chore> list = null;
		int myParentWorldId = chore.gameObject.GetMyParentWorldId();
		if (this.choreWorldMap.TryGetValue(myParentWorldId, out list))
		{
			list.Remove(chore);
		}
	}

	public virtual void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		List<Chore> list = null;
		int myParentWorldId = consumer_state.gameObject.GetMyParentWorldId();
		if (!this.choreWorldMap.TryGetValue(myParentWorldId, out list))
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].CollectChores(consumer_state, succeeded, failed_contexts, false);
		}
	}

	public Dictionary<int, List<Chore>> choreWorldMap = new Dictionary<int, List<Chore>>();
}
