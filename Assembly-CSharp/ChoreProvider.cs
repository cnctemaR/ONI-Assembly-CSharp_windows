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
		Game.Instance.Subscribe(1142724171, new Action<object>(this.OnEntityMigrated));
	}

	protected override void OnSpawn()
	{
		if (ClusterManager.Instance != null)
		{
			ClusterManager.Instance.Subscribe(-1078710002, new Action<object>(this.OnWorldRemoved));
		}
		base.OnSpawn();
		this.Name = base.name;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(880851192, new Action<object>(this.OnWorldParentChanged));
		Game.Instance.Unsubscribe(586301400, new Action<object>(this.OnMinionMigrated));
		Game.Instance.Unsubscribe(1142724171, new Action<object>(this.OnEntityMigrated));
		if (ClusterManager.Instance != null)
		{
			ClusterManager.Instance.Unsubscribe(-1078710002, new Action<object>(this.OnWorldRemoved));
		}
	}

	protected virtual void OnWorldRemoved(object data)
	{
		int num = (int)data;
		int parentWorldId = ClusterManager.Instance.GetWorld(num).ParentWorldId;
		List<Chore> list;
		if (this.choreWorldMap.TryGetValue(parentWorldId, out list))
		{
			this.ClearWorldChores<Chore>(list, num);
		}
	}

	protected virtual void OnWorldParentChanged(object data)
	{
		WorldParentChangedEventArgs e = data as WorldParentChangedEventArgs;
		List<Chore> list;
		if (e == null || e.lastParentId == 255 || e.lastParentId == e.world.ParentWorldId || !this.choreWorldMap.TryGetValue(e.lastParentId, out list))
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

	protected virtual void OnEntityMigrated(object data)
	{
		MigrationEventArgs e = data as MigrationEventArgs;
		List<Chore> list;
		if (e == null || !(e.entity == base.gameObject) || e.prevWorldId == e.targetWorldId || !this.choreWorldMap.TryGetValue(e.prevWorldId, out list))
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

	protected void ClearWorldChores<T>(List<T> chores, int worldId) where T : Chore
	{
		int num = chores.Count - 1;
		for (int i = num; i >= 0; i--)
		{
			if (chores[i].gameObject.GetMyWorldId() == worldId)
			{
				chores[i] = chores[num];
				chores.RemoveAt(num--);
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
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].provider == null)
			{
				list[i].Cancel("no provider");
				list[i] = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
			}
		}
		int num = CPUBudget.coreCount * 4;
		if (list.Count < num)
		{
			using (List<Chore>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chore chore = enumerator.Current;
					chore.CollectChores(consumer_state, succeeded, failed_contexts, false);
				}
				return;
			}
		}
		ChoreProvider.batch_chore_collector.Reset(list);
		int coreCount = CPUBudget.coreCount;
		int num2 = Math.Min(24, list.Count / coreCount);
		for (int j = 0; j < list.Count; j += num2)
		{
			ChoreProvider.batch_chore_collector.Add(new ChoreProvider.ChoreProviderCollectTask(j, Math.Min(j + num2, list.Count), consumer_state));
		}
		GlobalJobManager.Run(ChoreProvider.batch_chore_collector);
		for (int k = 0; k < ChoreProvider.batch_chore_collector.Count; k++)
		{
			ChoreProvider.batch_chore_collector.GetWorkItem(k).Finish(succeeded, failed_contexts);
		}
	}

	public Dictionary<int, List<Chore>> choreWorldMap = new Dictionary<int, List<Chore>>();

	private static WorkItemCollection<ChoreProvider.ChoreProviderCollectTask, List<Chore>> batch_chore_collector = new WorkItemCollection<ChoreProvider.ChoreProviderCollectTask, List<Chore>>();

	private struct ChoreProviderCollectTask : IWorkItem<List<Chore>>
	{
		public ChoreProviderCollectTask(int start, int end, ChoreConsumerState consumer_state)
		{
			this.start = start;
			this.end = end;
			this.consumer_state = consumer_state;
			this.succeeded = ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.Allocate();
			this.failed = ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.Allocate();
			this.incomplete = ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.Allocate();
		}

		public void Run(List<Chore> chores)
		{
			for (int i = this.start; i < this.end; i++)
			{
				chores[i].CollectChores(this.consumer_state, this.succeeded, this.incomplete, this.failed, false);
			}
		}

		public void Finish(List<Chore.Precondition.Context> combined_succeeded, List<Chore.Precondition.Context> combined_failed)
		{
			combined_succeeded.AddRange(this.succeeded);
			this.succeeded.Recycle();
			combined_failed.AddRange(this.failed);
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
			this.incomplete.Recycle();
		}

		private int start;

		private int end;

		private ChoreConsumerState consumer_state;

		public ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.PooledList succeeded;

		public ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.PooledList failed;

		public ListPool<Chore.Precondition.Context, ChoreProvider.ChoreProviderCollectTask>.PooledList incomplete;
	}
}
