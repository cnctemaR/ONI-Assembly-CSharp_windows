using System;

public class CritterElementMonitor : GameStateMachine<CritterElementMonitor, CritterElementMonitor.Instance, IStateMachineTarget>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update("UpdateInElement", delegate(CritterElementMonitor.Instance smi, float dt)
		{
			smi.UpdateCurrentElement(dt);
		}, UpdateRate.SIM_1000ms, false);
	}

	public new class Instance : GameStateMachine<CritterElementMonitor, CritterElementMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public event Action<float> OnUpdateEggChances;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateCurrentElement(float dt)
		{
			this.OnUpdateEggChances(dt);
		}
	}
}
