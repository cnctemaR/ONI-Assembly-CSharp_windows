using System;
using UnityEngine;

public class MentalBreakChore : Chore<MentalBreakChore.StatesInstance>
{
	public MentalBreakChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.MentalBreak, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true)
	{
		this.smi = new MentalBreakChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<MentalBreakChore.States, MentalBreakChore.StatesInstance, MentalBreakChore>.GameInstance
	{
		public StatesInstance(MentalBreakChore master, GameObject brokenperson)
			: base(master)
		{
			base.sm.brokenperson.Set(brokenperson, base.smi);
		}
	}

	public class States : GameStateMachine<MentalBreakChore.States, MentalBreakChore.StatesInstance, MentalBreakChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.broken;
			base.Target(this.brokenperson);
			this.broken.PlayAnim("fetal_loop", KAnim.PlayMode.Loop, null);
		}

		public StateMachine<MentalBreakChore.States, MentalBreakChore.StatesInstance, MentalBreakChore>.TargetParameter brokenperson;

		public GameStateMachine<MentalBreakChore.States, MentalBreakChore.StatesInstance, MentalBreakChore>.State broken;
	}
}
