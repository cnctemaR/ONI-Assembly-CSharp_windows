using System;
using UnityEngine;

public class ClimbableTreeMonitor : GameStateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.WantsToClimbTree, (ClimbableTreeMonitor.Instance smi) => smi.UpdateHasClimbable(), delegate(ClimbableTreeMonitor.Instance smi)
		{
			smi.OnClimbComplete();
		});
	}

	private const int MAX_NAV_COST = 2147483647;

	public class Def : StateMachine.BaseDef
	{
		public float searchMinInterval = 60f;

		public float searchMaxInterval = 120f;
	}

	public new class Instance : GameStateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ClimbableTreeMonitor.Def def)
			: base(master, def)
		{
			this.RefreshSearchTime();
		}

		private void RefreshSearchTime()
		{
			this.nextSearchTime = Time.time + Mathf.Lerp(base.def.searchMinInterval, base.def.searchMaxInterval, global::UnityEngine.Random.value);
		}

		public bool UpdateHasClimbable()
		{
			if (this.climbTarget == null)
			{
				if (Time.time < this.nextSearchTime)
				{
					return false;
				}
				this.FindClimbableTree();
				this.RefreshSearchTime();
			}
			return this.climbTarget != null;
		}

		private void FindClimbableTree()
		{
			this.climbTarget = null;
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			ListPool<KMonoBehaviour, ClimbableTreeMonitor>.PooledList pooledList2 = ListPool<KMonoBehaviour, ClimbableTreeMonitor>.Allocate();
			Vector3 position = base.master.transform.GetPosition();
			Extents extents = new Extents(Grid.PosToCell(position), 10);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.plants, pooledList);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, pooledList);
			Navigator component = base.GetComponent<Navigator>();
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
			{
				KMonoBehaviour kmonoBehaviour = scenePartitionerEntry.obj as KMonoBehaviour;
				if (!kmonoBehaviour.HasTag(GameTags.Creatures.ReservedByCreature))
				{
					int num = Grid.PosToCell(kmonoBehaviour);
					if (component.CanReach(num))
					{
						BuddingTrunk component2 = kmonoBehaviour.GetComponent<BuddingTrunk>();
						StorageLocker component3 = kmonoBehaviour.GetComponent<StorageLocker>();
						if (component2 != null)
						{
							if (!component2.ExtraSeedAvailable)
							{
								continue;
							}
						}
						else
						{
							if (!(component3 != null))
							{
								continue;
							}
							Storage component4 = component3.GetComponent<Storage>();
							if (!component4.allowItemRemoval)
							{
								continue;
							}
							if (component4.IsEmpty())
							{
								continue;
							}
						}
						pooledList2.Add(kmonoBehaviour);
					}
				}
			}
			if (pooledList2.Count > 0)
			{
				int num2 = global::UnityEngine.Random.Range(0, pooledList2.Count);
				KMonoBehaviour kmonoBehaviour2 = pooledList2[num2];
				this.climbTarget = kmonoBehaviour2.gameObject;
			}
			pooledList.Recycle();
			pooledList2.Recycle();
		}

		public void OnClimbComplete()
		{
			this.climbTarget = null;
		}

		public GameObject climbTarget;

		public float nextSearchTime;
	}
}
