using System;

public class AnimInterruptMonitor : GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.Behaviours.PlayInterruptAnim, new StateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>.Transition.ConditionCallback(AnimInterruptMonitor.ShoulPlayAnim), new Action<AnimInterruptMonitor.Instance>(AnimInterruptMonitor.ClearAnim));
	}

	private static bool ShoulPlayAnim(AnimInterruptMonitor.Instance smi)
	{
		return smi.anim.IsValid;
	}

	private static void ClearAnim(AnimInterruptMonitor.Instance smi)
	{
		smi.anim = HashedString.Invalid;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AnimInterruptMonitor.Def def)
			: base(master, def)
		{
		}

		public void PlayAnim(HashedString anim)
		{
			this.anim = anim;
			base.GetComponent<CreatureBrain>().UpdateBrain();
		}

		public HashedString anim;
	}
}
