using System;
using UnityEngine;

public class StressEmoteChore : Chore<StressEmoteChore.StatesInstance>
{
	public StressEmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		base.AddPrecondition(ChorePreconditions.IsMoving, null);
		base.AddPrecondition(ChorePreconditions.IsOffLadder, null);
		base.AddPrecondition(ChorePreconditions.NotInTube, null);
		base.AddPrecondition(ChorePreconditions.IsAwake, null);
		this.getStatusItem = get_status_item;
		this.smi = new StressEmoteChore.StatesInstance(this, target.gameObject, emote_kanim, emote_anims, play_mode);
	}

	protected override StatusItem GetStatusItem()
	{
		return (this.getStatusItem == null) ? base.GetStatusItem() : this.getStatusItem();
	}

	public override string ToString()
	{
		if (this.smi.emoteKAnim.IsValid())
		{
			return "StressEmoteChore<" + this.smi.emoteKAnim + ">";
		}
		return "StressEmoteChore<" + this.smi.emoteAnims[0] + ">";
	}

	private Func<StatusItem> getStatusItem;

	public class StatesInstance : GameStateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore, object>.GameInstance
	{
		public StatesInstance(StressEmoteChore master, GameObject emoter, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode)
			: base(master)
		{
			this.emoteKAnim = emote_kanim;
			this.emoteAnims = emote_anims;
			this.mode = mode;
			base.sm.emoter.Set(emoter, base.smi);
		}

		public HashedString[] emoteAnims;

		public HashedString emoteKAnim;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;
	}

	public class States : GameStateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.emoter);
			this.root.ToggleAnims((StressEmoteChore.StatesInstance smi) => smi.emoteKAnim).ToggleThought(Db.Get().Thoughts.Unhappy, null).PlayAnims((StressEmoteChore.StatesInstance smi) => smi.emoteAnims, (StressEmoteChore.StatesInstance smi) => smi.mode)
				.OnAnimQueueComplete(null);
		}

		public StateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore, object>.TargetParameter emoter;
	}
}
