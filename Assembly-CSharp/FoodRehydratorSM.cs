using System;

public class FoodRehydratorSM : GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EnterTransition(this.off, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsFunctional).EnterTransition(this.on, (FoodRehydratorSM.StatesInstance smi) => smi.operational.IsFunctional);
		this.off.PlayAnim("off", KAnim.PlayMode.Loop).EnterTransition(this.on, (FoodRehydratorSM.StatesInstance smi) => smi.operational.IsFunctional).EventTransition(GameHashes.FunctionalChanged, this.on, (FoodRehydratorSM.StatesInstance smi) => smi.operational.IsFunctional);
		this.on.PlayAnim("on", KAnim.PlayMode.Loop).EnterTransition(this.off, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsFunctional).EnterTransition(this.active, (FoodRehydratorSM.StatesInstance smi) => smi.operational.IsActive)
			.EventTransition(GameHashes.FunctionalChanged, this.off, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsFunctional)
			.EventTransition(GameHashes.ActiveChanged, this.active, (FoodRehydratorSM.StatesInstance smi) => smi.operational.IsActive);
		this.active.EnterTransition(this.off, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsFunctional).EnterTransition(this.on, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsActive).EventTransition(GameHashes.FunctionalChanged, this.off, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsFunctional)
			.EventTransition(GameHashes.ActiveChanged, this.postactive, (FoodRehydratorSM.StatesInstance smi) => !smi.operational.IsActive);
		this.postactive.OnAnimQueueComplete(this.on);
	}

	private GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>.State off;

	private GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>.State on;

	private GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>.State active;

	private GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>.State postactive;

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<FoodRehydratorSM, FoodRehydratorSM.StatesInstance, IStateMachineTarget, FoodRehydratorSM.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, FoodRehydratorSM.Def def)
			: base(master, def)
		{
		}

		[MyCmpReq]
		public Operational operational;
	}
}
