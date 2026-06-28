using System;

public class QuarantineFeedableMonitor : GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.EventTransition(GameHashes.AddUrge, this.hungry, (QuarantineFeedableMonitor.Instance smi) => smi.IsHungry());
		this.hungry.EventTransition(GameHashes.RemoveUrge, this.satisfied, (QuarantineFeedableMonitor.Instance smi) => !smi.IsHungry());
	}

	public GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.State hungry;

	public new class Instance : GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsHungry()
		{
			return base.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
		}
	}
}
