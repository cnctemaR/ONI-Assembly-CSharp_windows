using System;
using UnityEngine;

public class MoveToQuarantineChore : Chore<MoveToQuarantineChore.StatesInstance>
{
	public MoveToQuarantineChore(IStateMachineTarget target, KMonoBehaviour quarantine_area)
		: base(Db.Get().ChoreTypes.MoveToQuarantine, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, null)
	{
		this.smi = new MoveToQuarantineChore.StatesInstance(this, target.gameObject);
		this.smi.sm.locator.Set(quarantine_area.gameObject, this.smi);
	}

	public class StatesInstance : GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.GameInstance
	{
		public StatesInstance(MoveToQuarantineChore master, GameObject quarantined)
			: base(master)
		{
			base.sm.quarantined.Set(quarantined, base.smi);
		}
	}

	public class States : GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			this.approach.InitializeStates(this.quarantined, this.locator, this.success, null, null, null);
			this.success.ReturnSuccess();
		}

		public StateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.TargetParameter locator;

		public StateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.TargetParameter quarantined;

		public GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.ApproachSubState<IApproachable> approach;

		public GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.State success;
	}
}
