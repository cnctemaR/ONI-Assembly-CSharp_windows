using System;
using UnityEngine;

public class ManualControlIdleChore : Chore<ManualControlIdleChore.StatesInstance>
{
	public ManualControlIdleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.ManualControlIdle, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true)
	{
		this.smi = new ManualControlIdleChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<ManualControlIdleChore.States, ManualControlIdleChore.StatesInstance, ManualControlIdleChore>.GameInstance
	{
		public StatesInstance(ManualControlIdleChore master, GameObject idler)
			: base(master)
		{
			base.sm.idler.Set(idler, base.smi);
		}
	}

	public class States : GameStateMachine<ManualControlIdleChore.States, ManualControlIdleChore.StatesInstance, ManualControlIdleChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.Target(this.idler);
			this.idle.PlayAnim("idle_default", KAnim.PlayMode.Loop, null);
		}

		public StateMachine<ManualControlIdleChore.States, ManualControlIdleChore.StatesInstance, ManualControlIdleChore>.TargetParameter idler;

		public GameStateMachine<ManualControlIdleChore.States, ManualControlIdleChore.StatesInstance, ManualControlIdleChore>.State idle;
	}
}
