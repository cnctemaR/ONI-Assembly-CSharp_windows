using System;
using STRINGS;
using UnityEngine;

public class TreeClimbStates : GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.moving;
		this.root.Enter(new StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State.Callback(TreeClimbStates.SetTarget)).Enter(delegate(TreeClimbStates.Instance smi)
		{
			if (!TreeClimbStates.ReserveClimbable(smi))
			{
				smi.GoTo(this.behaviourcomplete);
			}
		}).Exit(new StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State.Callback(TreeClimbStates.UnreserveClimbable))
			.ToggleStatusItem(CREATURES.STATUSITEMS.RUMMAGINGSEED.NAME, CREATURES.STATUSITEMS.RUMMAGINGSEED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.moving.MoveTo(new Func<TreeClimbStates.Instance, int>(TreeClimbStates.GetClimbableCell), this.climbing, this.behaviourcomplete, false);
		this.climbing.DefaultState(this.climbing.pre);
		this.climbing.pre.PlayAnim("rummage_pre").OnAnimQueueComplete(this.climbing.loop);
		this.climbing.loop.QueueAnim("rummage_loop", true, null).ScheduleGoTo(3.5f, this.climbing.pst).Update(new Action<TreeClimbStates.Instance, float>(TreeClimbStates.Rummage), UpdateRate.SIM_1000ms, false);
		this.climbing.pst.QueueAnim("rummage_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToClimbTree, false);
	}

	private static void SetTarget(TreeClimbStates.Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<ClimbableTreeMonitor.Instance>().climbTarget, smi, false);
	}

	private static bool ReserveClimbable(TreeClimbStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null && !gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
		{
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
			return true;
		}
		return false;
	}

	private static void UnreserveClimbable(TreeClimbStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void Rummage(TreeClimbStates.Instance smi, float dt)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			BuddingTrunk component = gameObject.GetComponent<BuddingTrunk>();
			if (component)
			{
				component.ExtractExtraSeed();
				return;
			}
			Storage component2 = gameObject.GetComponent<Storage>();
			if (component2 && component2.items.Count > 0)
			{
				int num = global::UnityEngine.Random.Range(0, component2.items.Count - 1);
				GameObject gameObject2 = component2.items[num];
				Pickupable pickupable = (gameObject2 ? gameObject2.GetComponent<Pickupable>() : null);
				if (pickupable && pickupable.UnreservedAmount > 0.01f)
				{
					smi.Toss(pickupable);
				}
			}
		}
	}

	private static int GetClimbableCell(TreeClimbStates.Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}

	public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.ApproachSubState<Uprootable> moving;

	public TreeClimbStates.ClimbState climbing;

	public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State behaviourcomplete;

	public StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.TargetParameter target;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.GameInstance
	{
		public Instance(Chore<TreeClimbStates.Instance> chore, TreeClimbStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToClimbTree);
			this.storage = base.GetComponent<Storage>();
		}

		public void Toss(Pickupable pu)
		{
			Pickupable pickupable = pu.Take(Mathf.Min(1f, pu.UnreservedAmount));
			if (pickupable != null)
			{
				this.storage.Store(pickupable.gameObject, true, false, true, false);
				this.storage.Drop(pickupable.gameObject, true);
				this.Throw(pickupable.gameObject);
			}
		}

		private void Throw(GameObject ore_go)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			int num = Grid.PosToCell(position);
			int num2 = Grid.CellAbove(num);
			Vector2 zero;
			if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
			{
				zero = Vector2.zero;
			}
			else
			{
				position.y += 0.5f;
				zero = new Vector2(global::UnityEngine.Random.Range(TreeClimbStates.Instance.VEL_MIN.x, TreeClimbStates.Instance.VEL_MAX.x), global::UnityEngine.Random.Range(TreeClimbStates.Instance.VEL_MIN.y, TreeClimbStates.Instance.VEL_MAX.y));
			}
			ore_go.transform.SetPosition(position);
			if (GameComps.Fallers.Has(ore_go))
			{
				GameComps.Fallers.Remove(ore_go);
			}
			GameComps.Fallers.Add(ore_go, zero);
		}

		private Storage storage;

		private static readonly Vector2 VEL_MIN = new Vector2(-1f, 2f);

		private static readonly Vector2 VEL_MAX = new Vector2(1f, 4f);
	}

	public class ClimbState : GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State
	{
		public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State pre;

		public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State loop;

		public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State pst;
	}
}
