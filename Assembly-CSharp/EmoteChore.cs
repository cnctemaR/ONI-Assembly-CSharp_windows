using System;
using UnityEngine;

public class EmoteChore : Chore<EmoteChore.StatesInstance>
{
	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString[] emote_anims, Func<StatusItem> get_status_item = null)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0, null)
	{
		this.smi = new EmoteChore.StatesInstance(this, target.gameObject, null, emote_anims, KAnim.PlayMode.Once);
		this.getStatusItem = get_status_item;
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0, null)
	{
		this.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote_kanim, emote_anims, play_mode);
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0, null)
	{
		this.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote_kanim, emote_anims, KAnim.PlayMode.Once);
		this.getStatusItem = get_status_item;
	}

	protected override StatusItem GetStatusItem()
	{
		return (this.getStatusItem == null) ? base.GetStatusItem() : this.getStatusItem();
	}

	public override string ToString()
	{
		if (this.smi.emoteKAnim.IsValid)
		{
			return "EmoteChore<" + this.smi.emoteKAnim + ">";
		}
		return "EmoteChore<" + this.smi.emoteAnims[0] + ">";
	}

	private Func<StatusItem> getStatusItem;

	public class StatesInstance : GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.GameInstance
	{
		public StatesInstance(EmoteChore master, GameObject emoter, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode)
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

	public class States : GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.emoter);
			this.root.ToggleAnims((EmoteChore.StatesInstance smi) => smi.emoteKAnim).PlayAnims((EmoteChore.StatesInstance smi) => smi.emoteAnims, (EmoteChore.StatesInstance smi) => smi.mode).OnAnimQueueComplete(null);
		}

		public StateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.TargetParameter emoter;
	}
}
