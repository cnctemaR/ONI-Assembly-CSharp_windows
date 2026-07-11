using System;
using UnityEngine;

public class SighChore : Chore<SighChore.StatesInstance>
{
	public SighChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Sigh, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, null)
	{
		this.smi = new SighChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<SighChore.States, SighChore.StatesInstance, SighChore, object>.GameInstance
	{
		public StatesInstance(SighChore master, GameObject sigher)
			: base(master)
		{
			base.sm.sigher.Set(sigher, base.smi);
		}
	}

	public class States : GameStateMachine<SighChore.States, SighChore.StatesInstance, SighChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.sigher);
			this.root.PlayAnim("emote_depressed").OnAnimQueueComplete(null);
		}

		public StateMachine<SighChore.States, SighChore.StatesInstance, SighChore, object>.TargetParameter sigher;
	}
}
