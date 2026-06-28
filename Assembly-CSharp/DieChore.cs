using System;

public class DieChore : Chore<DieChore.StatesInstance>
{
	public DieChore(IStateMachineTarget master, Death death)
		: base(Db.Get().ChoreTypes.Die, master, master.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new DieChore.StatesInstance(this, death);
	}

	public class StatesInstance : GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.GameInstance
	{
		public StatesInstance(DieChore master, Death death)
			: base(master)
		{
			base.sm.death.Set(death, base.smi);
		}

		public void PlayPreAnim()
		{
			string preAnim = base.sm.death.Get(base.smi).preAnim;
			base.GetComponent<KAnimControllerBase>().Play(preAnim, KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public class States : GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.dying;
			this.dying.OnAnimQueueComplete(this.dead).Enter("PlayAnim", delegate(DieChore.StatesInstance smi)
			{
				smi.PlayPreAnim();
			});
			this.dead.ReturnSuccess();
		}

		public GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.State dying;

		public GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.State dead;

		public StateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.ResourceParameter<Death> death;
	}
}
