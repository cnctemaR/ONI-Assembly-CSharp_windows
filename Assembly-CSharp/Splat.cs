using System;

public class Splat : GameStateMachine<Splat, Splat.StatesInstance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleChore((Splat.StatesInstance smi) => new WorkChore<SplatWorkable>(Db.Get().ChoreTypes.Mop, smi.master, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true), this.complete);
		this.complete.Enter(delegate(Splat.StatesInstance smi)
		{
			Util.KDestroyGameObject(smi.master.gameObject);
		});
	}

	public GameStateMachine<Splat, Splat.StatesInstance, IStateMachineTarget, object>.State complete;

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<Splat, Splat.StatesInstance, IStateMachineTarget, object>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, Splat.Def def)
			: base(master, def)
		{
		}
	}
}
