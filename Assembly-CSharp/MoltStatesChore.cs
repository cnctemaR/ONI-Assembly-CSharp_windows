using System;

public class MoltStatesChore : GameStateMachine<MoltStatesChore, MoltStatesChore.Instance, IStateMachineTarget, MoltStatesChore.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.molting;
		this.molting.PlayAnim((MoltStatesChore.Instance smi) => smi.def.moltAnimName, KAnim.PlayMode.Once).ScheduleGoTo(5f, this.complete).OnAnimQueueComplete(this.complete);
		this.complete.BehaviourComplete(GameTags.Creatures.ReadyToMolt, false);
	}

	public GameStateMachine<MoltStatesChore, MoltStatesChore.Instance, IStateMachineTarget, MoltStatesChore.Def>.State molting;

	public GameStateMachine<MoltStatesChore, MoltStatesChore.Instance, IStateMachineTarget, MoltStatesChore.Def>.State complete;

	public class Def : StateMachine.BaseDef
	{
		public string moltAnimName;
	}

	public new class Instance : GameStateMachine<MoltStatesChore, MoltStatesChore.Instance, IStateMachineTarget, MoltStatesChore.Def>.GameInstance
	{
		public Instance(Chore<MoltStatesChore.Instance> chore, MoltStatesChore.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.ReadyToMolt);
		}
	}
}
