using System;
using STRINGS;

public class FallStates : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.FALLING.NAME, CREATURES.STATUSITEMS.FALLING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.loop.PlayAnim((FallStates.Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().anim, KAnim.PlayMode.Loop).ToggleGravity().EventTransition(GameHashes.Landed, this.snaptoground, null)
			.Transition(this.pst, (FallStates.Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation(true), UpdateRate.SIM_33ms);
		this.snaptoground.Enter(delegate(FallStates.Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().SnapToGround();
		}).GoTo(this.pst);
		this.pst.Enter(new StateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State.Callback(FallStates.PlayLandAnim)).BehaviourComplete(GameTags.Creatures.Falling, false);
	}

	private static void PlayLandAnim(FallStates.Instance smi)
	{
		smi.GetComponent<KBatchedAnimController>().Queue(smi.def.getLandAnim(smi), KAnim.PlayMode.Loop, 1f, 0f);
	}

	private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State loop;

	private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State snaptoground;

	private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
		public Func<FallStates.Instance, string> getLandAnim = (FallStates.Instance smi) => "idle_loop";
	}

	public new class Instance : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.GameInstance
	{
		public Instance(Chore<FallStates.Instance> chore, FallStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Falling);
		}
	}
}
