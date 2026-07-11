using System;
using STRINGS;
using UnityEngine;

public class IncubatingStates : GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.incubator;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.IN_INCUBATOR.NAME, CREATURES.STATUSITEMS.IN_INCUBATOR.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.incubator.DefaultState(this.incubator.idle).ToggleTag(GameTags.Creatures.Deliverable).TagTransition(GameTags.Creatures.InIncubator, null, true);
		this.incubator.idle.Enter("VariantUpdate", new StateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.State.Callback(IncubatingStates.VariantUpdate)).PlayAnim("incubator_idle_loop").OnAnimQueueComplete(this.incubator.choose);
		this.incubator.choose.Transition(this.incubator.variant, new StateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.Transition.ConditionCallback(IncubatingStates.DoVariant), UpdateRate.SIM_200ms).Transition(this.incubator.idle, GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.Not(new StateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.Transition.ConditionCallback(IncubatingStates.DoVariant)), UpdateRate.SIM_200ms);
		this.incubator.variant.PlayAnim("incubator_variant").OnAnimQueueComplete(this.incubator.idle);
	}

	public static bool DoVariant(IncubatingStates.Instance smi)
	{
		return smi.variant_time == 0;
	}

	public static void VariantUpdate(IncubatingStates.Instance smi)
	{
		if (smi.variant_time <= 0)
		{
			smi.variant_time = global::UnityEngine.Random.Range(3, 7);
			return;
		}
		smi.variant_time--;
	}

	public IncubatingStates.IncubatorStates incubator;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.GameInstance
	{
		public Instance(Chore<IncubatingStates.Instance> chore, IncubatingStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(IncubatingStates.Instance.IsInIncubator, null);
		}

		public int variant_time = 3;

		public static readonly Chore.Precondition IsInIncubator = new Chore.Precondition
		{
			id = "IsInIncubator",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.InIncubator);
			}
		};
	}

	public class IncubatorStates : GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.State
	{
		public GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.State idle;

		public GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.State choose;

		public GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>.State variant;
	}
}
