using System;
using KSerialization;
using TUNING;

public class EquippableBalloon : StateMachineComponent<EquippableBalloon.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.transitionTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.JOY_REACTION_DURATION;
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public class StatesInstance : GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.GameInstance
	{
		public StatesInstance(EquippableBalloon master)
			: base(master)
		{
		}

		[Serialize]
		public float transitionTime;
	}

	public class States : GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.serializable = true;
			this.root.Transition(this.destroy, (EquippableBalloon.StatesInstance smi) => GameClock.Instance.GetTime() >= smi.transitionTime, UpdateRate.SIM_200ms);
			this.destroy.Enter(delegate(EquippableBalloon.StatesInstance smi)
			{
				smi.master.GetComponent<Equippable>().Unassign();
			});
		}

		public GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.State destroy;
	}
}
