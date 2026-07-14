using System;
using Klei.AI;

public class PlanktonCoral : GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.healthy;
		this.wilted.EventTransition(GameHashes.WiltRecover, this.healthy, GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.Not(new StateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.Transition.ConditionCallback(PlanktonCoral.IsWilted)));
		this.healthy.DefaultState(this.healthy.breathing);
		this.healthy.breathing.EventTransition(GameHashes.Grow, this.healthy.clogged, new StateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.Transition.ConditionCallback(PlanktonCoral.IsFullyGrown)).EventTransition(GameHashes.Wilt, this.wilted, new StateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.Transition.ConditionCallback(PlanktonCoral.IsWilted));
		this.healthy.clogged.EventTransition(GameHashes.Harvest, this.healthy.breathing, null);
	}

	public static bool IsWilted(PlanktonCoral.Instance smi)
	{
		return smi.IsWilted;
	}

	public static bool IsFullyGrown(PlanktonCoral.Instance smi)
	{
		return smi.IsFullyGrown;
	}

	public const string INHALE_ANIM_NAME = "inhale";

	public const string EXHALE_ANIM_NAME = "exhale";

	public GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.State wilted;

	public PlanktonCoral.HealthyStates healthy;

	public class Def : StateMachine.BaseDef
	{
	}

	public class HealthyStates : GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.State
	{
		public GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.State breathing;

		public GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.State clogged;
	}

	public new class Instance : GameStateMachine<PlanktonCoral, PlanktonCoral.Instance, IStateMachineTarget, PlanktonCoral.Def>.GameInstance
	{
		public bool IsFullyGrown
		{
			get
			{
				return this.growing != null && this.growing.IsGrown();
			}
		}

		public bool IsWilted
		{
			get
			{
				return this.wiltCondition != null && this.wiltCondition.IsWilting();
			}
		}

		public Instance(IStateMachineTarget master, PlanktonCoral.Def def)
			: base(master, def)
		{
			this.wiltCondition = base.GetComponent<WiltCondition>();
			this.growing = base.GetComponent<Growing>();
		}

		private WiltCondition wiltCondition;

		private Growing growing;

		public AttributeModifier GrowModifier;
	}
}
