using System;
using Klei.AI;

public class StressMonitor : GameStateMachine<StressMonitor, StressMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = true;
		default_state = this.satisfied;
		this.root.ToggleUrge(Db.Get().Urges.Relax);
		this.satisfied.Transition(this.stressed.tier1, (StressMonitor.Instance smi) => smi.stress.value >= 60f).ToggleExpression(Db.Get().Expressions.Neutral, null);
		this.stressed.ToggleStatusItem(Db.Get().DuplicantStatusItems.Stressed, null).Transition(this.satisfied, (StressMonitor.Instance smi) => smi.stress.value < 60f).TriggerOnEnter(GameHashes.Stressed, null);
		this.stressed.tier1.Transition(this.stressed.tier2, (StressMonitor.Instance smi) => smi.stress.value >= 100f);
		this.stressed.tier2.TriggerOnEnter(GameHashes.StressedHadEnough, null).Transition(this.stressed.tier1, (StressMonitor.Instance smi) => smi.stress.value < 100f);
	}

	private const float StressThreshold_One = 60f;

	private const float StressThreshold_Two = 100f;

	public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public StressMonitor.Stressed stressed;

	public class Stressed : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier1;

		public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier2;
	}

	public new class Instance : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.stress = Db.Get().Amounts.Stress.Lookup(base.gameObject);
		}

		public bool IsStressed()
		{
			return base.IsInsideState(base.sm.stressed);
		}

		public bool HasHadEnough()
		{
			return this.stress.value >= 100f;
		}

		public AmountInstance stress;
	}
}
