using System;
using STRINGS;

internal class CallAdultStates : GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre;
		GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.SLEEPING.NAME;
		string text2 = CREATURES.STATUSITEMS.SLEEPING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		this.pre.QueueAnim("call_pre", false, null).OnAnimQueueComplete(this.loop);
		this.loop.QueueAnim("call_loop", false, null).OnAnimQueueComplete(this.pst);
		this.pst.QueueAnim("call_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.CallAdultBehaviour, false);
	}

	public GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.State pre;

	public GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.State loop;

	public GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.State pst;

	public GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>.GameInstance
	{
		public Instance(Chore<CallAdultStates.Instance> chore, CallAdultStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.CallAdultBehaviour);
		}
	}
}
