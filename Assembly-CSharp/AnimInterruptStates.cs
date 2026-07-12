using System;

public class AnimInterruptStates : GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.play_anim;
		this.play_anim.Enter(new StateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>.State.Callback(this.PlayAnim)).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.PlayInterruptAnim, false);
	}

	private void PlayAnim(AnimInterruptStates.Instance smi)
	{
		KBatchedAnimController kbatchedAnimController = smi.Get<KBatchedAnimController>();
		HashedString[] anims = smi.GetSMI<AnimInterruptMonitor.Instance>().anims;
		kbatchedAnimController.Play(anims[0], KAnim.PlayMode.Once, 1f, 0f);
		for (int i = 1; i < anims.Length; i++)
		{
			kbatchedAnimController.Queue(anims[i], KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>.State play_anim;

	public GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>.GameInstance
	{
		public Instance(Chore<AnimInterruptStates.Instance> chore, AnimInterruptStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.PlayInterruptAnim);
		}
	}
}
