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

		private static Util.IterationInstruction FindClimbableTree(object obj, ref ClimbableTreeMonitor.Instance.FindClimableTreeContext context)
		{
			KMonoBehaviour kmonoBehaviour = obj as KMonoBehaviour;
			if (kmonoBehaviour.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				return Util.IterationInstruction.Continue;
			}
			int num = Grid.PosToCell(kmonoBehaviour);
			if (!context.navigator.CanReach(num))
			{
				return Util.IterationInstruction.Continue;
			}
			ForestTreeSeedMonitor component = kmonoBehaviour.GetComponent<ForestTreeSeedMonitor>();
			StorageLocker component2 = kmonoBehaviour.GetComponent<StorageLocker>();
			if (component != null)
			{
				if (!component.ExtraSeedAvailable)
				{
					return Util.IterationInstruction.Continue;
				}
			}
			else
			{
				if (!(component2 != null))
				{
					return Util.IterationInstruction.Continue;
				}
				Storage component3 = component2.GetComponent<Storage>();
				if (!component3.allowItemRemoval)
				{
					return Util.IterationInstruction.Continue;
				}
				if (component3.IsEmpty())
				{
					return Util.IterationInstruction.Continue;
				}
			}
			context.targets.Add(kmonoBehaviour);
			return Util.IterationInstruction.Continue;
		}

		private void FindClimbableTree()
		{
			this.climbTarget = null;
			Vector3 position = base.master.transform.GetPosition();
			Extents extents = new Extents(Grid.PosToCell(position), 10);
			ClimbableTreeMonitor.Instance.FindClimableTreeContext findClimableTreeContext;
			findClimableTreeContext.navigator = base.GetComponent<Navigator>();
			findClimableTreeContext.targets = ListPool<KMonoBehaviour, ClimbableTreeMonitor>.Allocate();
			GameScenePartitioner.Instance.ReadonlyVisitEntries<ClimbableTreeMonitor.Instance.FindClimableTreeContext>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.plants, ClimbableTreeMonitor.Instance.FindClimbableTreeVisitor, ref findClimableTreeContext);
			GameScenePartitioner.Instance.ReadonlyVisitEntries<ClimbableTreeMonitor.Instance.FindClimableTreeContext>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.completeBuildings, ClimbableTreeMonitor.Instance.FindClimbableTreeVisitor, ref findClimableTreeContext);
			if (findClimableTreeContext.targets.Count > 0)
			{
				int num = global::UnityEngine.Random.Range(0, findClimableTreeContext.targets.Count);
				KMonoBehaviour kmonoBehaviour = findClimableTreeContext.targets[num];
				this.climbTarget = kmonoBehaviour.gameObject;
			}
			findClimableTreeContext.targets.Recycle();
		}

		public void OnClimbComplete()
		{
			this.climbTarget = null;
		}

		public GameObject climbTarget;

		public float nextSearchTime;

		private static GameScenePartitioner.VisitorRef<ClimbableTreeMonitor.Instance.FindClimableTreeContext> FindClimbableTreeVisitor = new GameScenePartitioner.VisitorRef<ClimbableTreeMonitor.Instance.FindClimableTreeContext>(ClimbableTreeMonitor.Instance.FindClimbableTree);

		private struct FindClimableTreeContext
		{
			public Navigator navigator;

			public ListPool<KMonoBehaviour, ClimbableTreeMonitor>.PooledList targets;
		}
	}
}
