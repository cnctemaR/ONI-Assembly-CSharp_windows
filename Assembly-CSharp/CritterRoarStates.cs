using System;

public class CritterRoarStates : GameStateMachine<CritterRoarStates, CritterRoarStates.Instance, IStateMachineTarget, CritterRoarStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.roar;
		this.roar.PlayAnims((CritterRoarStates.Instance smi) => CritterRoarStates.ANIM_SEQUENCE, KAnim.PlayMode.Once).ScheduleGoTo(10f, this.behaviourComplete).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.BehaviourComplete(CritterRoarStates.TAG, false);
	}

	private readonly GameStateMachine<CritterRoarStates, CritterRoarStates.Instance, IStateMachineTarget, CritterRoarStates.Def>.State roar;

	private readonly GameStateMachine<CritterRoarStates, CritterRoarStates.Instance, IStateMachineTarget, CritterRoarStates.Def>.State behaviourComplete;

	private const float FALLBACK_TIMEOUT = 10f;

	private static HashedString ANIM = "roar";

	private static readonly HashedString[] ANIM_SEQUENCE = new HashedString[] { CritterRoarStates.ANIM };

	private static Tag TAG = CritterRoarMonitor.TAG;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CritterRoarStates, CritterRoarStates.Instance, IStateMachineTarget, CritterRoarStates.Def>.GameInstance
	{
		public Instance(Chore<CritterRoarStates.Instance> chore, CritterRoarStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, CritterRoarStates.TAG);
		}
	}
}
