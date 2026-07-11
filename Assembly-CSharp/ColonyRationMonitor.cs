using System;
using System.Collections.Generic;

public class ColonyRationMonitor : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Update("UpdateOutOfRations", delegate(ColonyRationMonitor.Instance smi, float dt)
		{
			smi.UpdateIsOutOfRations();
		}, UpdateRate.SIM_200ms, false);
		this.satisfied.ParamTransition<bool>(this.isOutOfRations, this.outofrations, GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.IsTrue).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
		this.outofrations.ParamTransition<bool>(this.isOutOfRations, this.satisfied, GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.IsFalse).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
	}

	public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.State outofrations;

	private StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.BoolParameter isOutOfRations = new StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.BoolParameter();

	public new class Instance : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.UpdateIsOutOfRations();
		}

		public void UpdateIsOutOfRations()
		{
			bool flag = true;
			using (List<Edible>.Enumerator enumerator = Components.Edibles.Items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetComponent<Pickupable>().UnreservedAmount > 0f)
					{
						flag = false;
						break;
					}
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
