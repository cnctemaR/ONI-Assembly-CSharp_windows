using System;
using System.Collections.Generic;
using System.Linq;
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
			ListPool<KMonoBehaviour, ClimbableTreeMonitor>.PooledList pooledList = ListPool<KMonoBehaviour, ClimbableTreeMonitor>.Allocate();
			Vector3 position = base.master.transform.GetPosition();
			Extents extents = new Extents(Grid.PosToCell(position), 10);
			Navigator component = base.GetComponent<Navigator>();
			IEnumerable<object> enumerable = GameScenePartitioner.Instance.AsyncSafeEnumerate(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.plants);
			IEnumerable<object> enumerable2 = GameScenePartitioner.Instance.AsyncSafeEnumerate(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.completeBuildings);
			foreach (object obj in enumerable.Concat<object>(enumerable2))
			{
				KMonoBehaviour kmonoBehaviour = obj as KMonoBehaviour;
				if (!kmonoBehaviour.HasTag(GameTags.Creatures.ReservedByCreature))
				{
					int num = Grid.PosToCell(kmonoBehaviour);
					if (component.CanReach(num))
					{
						ForestTreeSeedMonitor component2 = kmonoBehaviour.GetComponent<ForestTreeSeedMonitor>();
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
							if (!component4.allowItemRemoval || component4.IsEmpty())
							{
								continue;
							}
						}
						pooledList.Add(kmonoBehaviour);
					}
				}
			}
			if (pooledList.Count > 0)
			{
				int num2 = global::UnityEngine.Random.Range(0, pooledList.Count);
				KMonoBehaviour kmonoBehaviour2 = pooledList[num2];
				this.climbTarget = kmonoBehaviour2.gameObject;
			}
			pooledList.Recycle();
		}

		public void OnClimbComplete()
		{
			this.climbTarget = null;
		}

		public GameObject climbTarget;

		public float nextSearchTime;
	}
}
