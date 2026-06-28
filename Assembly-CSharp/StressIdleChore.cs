using System;
using UnityEngine;

public class StressIdleChore : Chore<StressIdleChore.StatesInstance>
{
	public StressIdleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.StressIdle, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new StressIdleChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.GameInstance
	{
		public StatesInstance(StressIdleChore master, GameObject idler)
			: base(master)
		{
			base.sm.idler.Set(idler, base.smi);
		}
	}

	public class States : GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.Target(this.idler);
			this.idle.PlayAnim("idle_default", KAnim.PlayMode.Loop, null);
		}

		public StateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.TargetParameter idler;

		public GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.State idle;
	}
}
