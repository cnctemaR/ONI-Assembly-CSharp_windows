using System;
using Klei.AI;

public class StressMonitor : GameStateMachine<StressMonitor, StressMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = true;
		default_state = this.satisfied;
		this.root.ToggleUrge(Db.Get().Urges.Relax);
		this.satisfied.Transition(this.stressed, (StressMonitor.Instance smi) => smi.IsStressed()).ToggleExpression(Db.Get().Expressions.Neutral, null);
		this.stressed.ToggleStatusItem(Db.Get().DuplicantStatusItems.Stressed, null).Transition(this.satisfied, (StressMonitor.Instance smi) => !smi.IsStressed()).TriggerOnEnter(GameHashes.Stressed, null)
			.ToggleExpression(Db.Get().Expressions.Unhappy, null);
	}

	private const float StressedThreshold = 60f;

	public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget>.State stressed;

	public new class Instance : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.stress = Db.Get().Amounts.Stress.Lookup(base.gameObject);
		}

		public bool IsStressed()
		{
			return this.stress.value > 60f;
		}

		public AmountInstance stress;
	}
}
