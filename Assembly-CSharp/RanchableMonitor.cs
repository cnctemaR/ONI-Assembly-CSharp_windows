using System;

public class RanchableMonitor : GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.WantsToGetRanched, (RanchableMonitor.Instance smi) => smi.ShouldGoGetRanched(), null);
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>.GameInstance
	{
		public ChoreConsumer ChoreConsumer { get; private set; }

		public Navigator NavComponent
		{
			get
			{
				return this.navComponent;
			}
		}

		public RanchedStates.Instance States
		{
			get
			{
				if (this.states == null)
				{
					this.states = this.controller.GetSMI<RanchedStates.Instance>();
				}
				return this.states;
			}
		}

		public Instance(IStateMachineTarget master, RanchableMonitor.Def def)
			: base(master, def)
		{
			this.ChoreConsumer = base.GetComponent<ChoreConsumer>();
			this.navComponent = base.GetComponent<Navigator>();
		}

		public bool ShouldGoGetRanched()
		{
			return this.TargetRanchStation != null && this.TargetRanchStation.IsRunning() && this.TargetRanchStation.HasRancher;
		}

		public RanchStation.Instance TargetRanchStation;

		private Navigator navComponent;

		private RanchedStates.Instance states;
	}
}
