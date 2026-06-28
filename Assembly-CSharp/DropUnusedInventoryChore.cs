using System;

public class DropUnusedInventoryChore : Chore<DropUnusedInventoryChore.StatesInstance>
{
	public DropUnusedInventoryChore(ChoreType chore_type, IStateMachineTarget target)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new DropUnusedInventoryChore.StatesInstance(this);
	}

	public class StatesInstance : GameStateMachine<DropUnusedInventoryChore.States, DropUnusedInventoryChore.StatesInstance, DropUnusedInventoryChore, object>.GameInstance
	{
		public StatesInstance(DropUnusedInventoryChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<DropUnusedInventoryChore.States, DropUnusedInventoryChore.StatesInstance, DropUnusedInventoryChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.dropping;
			this.dropping.Enter(delegate(DropUnusedInventoryChore.StatesInstance smi)
			{
				smi.GetComponent<Storage>().DropAll();
			}).GoTo(this.success);
			this.success.ReturnSuccess();
		}

		public GameStateMachine<DropUnusedInventoryChore.States, DropUnusedInventoryChore.StatesInstance, DropUnusedInventoryChore, object>.State dropping;

		public GameStateMachine<DropUnusedInventoryChore.States, DropUnusedInventoryChore.StatesInstance, DropUnusedInventoryChore, object>.State success;
	}
}
