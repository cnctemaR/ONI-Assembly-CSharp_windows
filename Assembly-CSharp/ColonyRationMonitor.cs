using System;

public class ColonyRationMonitor : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Update("UpdateOutOfRations", delegate(ColonyRationMonitor.Instance smi)
		{
			smi.UpdateIsOutOfRations();
		});
		this.satisfied.ParamTransition<bool>(this.isOutOfRations, this.outofrations, (ColonyRationMonitor.Instance smi, bool p) => p).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
		this.outofrations.ParamTransition<bool>(this.isOutOfRations, this.satisfied, (ColonyRationMonitor.Instance smi, bool p) => !p).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
	}

	public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget>.State outofrations;

	private StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget>.BoolParameter isOutOfRations = new StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget>.BoolParameter();

	public new class Instance : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.UpdateIsOutOfRations();
		}

		public void UpdateIsOutOfRations()
		{
			bool flag = true;
			foreach (Edible edible in Components.Edibles)
			{
				if (edible.GetComponent<Pickupable>().UnreservedAmount > 0f)
				{
					flag = false;
					break;
				}
			}
			base.smi.sm.isOutOfRations.Set(flag, base.smi);
		}

		public bool IsOutOfRations()
		{
			return base.smi.sm.isOutOfRations.Get(base.smi);
		}
	}
}
