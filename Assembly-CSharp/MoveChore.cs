using System;
using UnityEngine;

public class MoveChore : Chore<MoveChore.StatesInstance>
{
	public MoveChore(IStateMachineTarget target, ChoreType chore_type, Func<MoveChore.StatesInstance, int> get_cell_callback, bool update_cell = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new MoveChore.StatesInstance(this, target.gameObject, get_cell_callback, update_cell);
	}

	public class StatesInstance : GameStateMachine<MoveChore.States, MoveChore.StatesInstance, MoveChore, object>.GameInstance
	{
		public StatesInstance(MoveChore master, GameObject mover, Func<MoveChore.StatesInstance, int> get_cell_callback, bool update_cell = false)
			: base(master)
		{
			this.getCellCallback = get_cell_callback;
			base.sm.mover.Set(mover, base.smi);
		}

		public Func<MoveChore.StatesInstance, int> getCellCallback;
	}

	public class States : GameStateMachine<MoveChore.States, MoveChore.StatesInstance, MoveChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.mover);
			this.root.MoveTo((MoveChore.StatesInstance smi) => smi.getCellCallback(smi), null, null, false);
		}

		public GameStateMachine<MoveChore.States, MoveChore.StatesInstance, MoveChore, object>.ApproachSubState<Approachable> approach;

		public StateMachine<MoveChore.States, MoveChore.StatesInstance, MoveChore, object>.TargetParameter mover;

		public StateMachine<MoveChore.States, MoveChore.StatesInstance, MoveChore, object>.TargetParameter locator;
	}
}
