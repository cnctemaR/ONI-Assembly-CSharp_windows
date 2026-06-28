using System;
using UnityEngine;

public class ManualControlGoToChore : Chore<ManualControlGoToChore.StatesInstance>
{
	public ManualControlGoToChore(IStateMachineTarget target, Vector3 pos)
		: base(Db.Get().ChoreTypes.ManualControlGoTo, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true)
	{
		this.smi = new ManualControlGoToChore.StatesInstance(this, target.gameObject, pos);
	}

	public class StatesInstance : GameStateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.GameInstance
	{
		public StatesInstance(ManualControlGoToChore master, GameObject approacher, Vector3 pos)
			: base(master)
		{
			base.sm.movePos.Set(pos, this);
			base.sm.approacher.Set(approacher, base.smi);
		}

		public void CreateLocator(Vector3 pos)
		{
			GameObject gameObject = ChoreHelpers.CreateLocator("ManualControlGotoLocator", base.sm.movePos.Get(this));
			base.sm.locator.Set(gameObject, this);
		}

		public void DestroyLocator()
		{
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}
	}

	public class States : GameStateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.approacher);
			this.root.Enter("CreateLocator", delegate(ManualControlGoToChore.StatesInstance smi)
			{
				smi.CreateLocator(smi.master.transform.position);
			}).Exit("DestroyLocator", delegate(ManualControlGoToChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.approach.InitializeStates(this.approacher, this.locator, this.success, null, null, null);
			this.success.ReturnSuccess();
		}

		public StateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.TargetParameter locator;

		public StateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.TargetParameter approacher;

		public StateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.Vector3Parameter movePos;

		public GameStateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.ApproachSubState<Approachable> approach;

		public GameStateMachine<ManualControlGoToChore.States, ManualControlGoToChore.StatesInstance, ManualControlGoToChore>.State success;
	}
}
