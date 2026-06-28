using System;
using UnityEngine;

public class ReactEmoteChore : Chore<ReactEmoteChore.StatesInstance>
{
	public ReactEmoteChore(IStateMachineTarget target, ChoreType chore_type, Reactable reactable, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		base.AddPrecondition(ChorePreconditions.IsMoving, null);
		base.AddPrecondition(ChorePreconditions.IsOffLadder, null);
		base.AddPrecondition(ChorePreconditions.IsAwake, null);
		this.getStatusItem = get_status_item;
		this.smi = new ReactEmoteChore.StatesInstance(this, target.gameObject, reactable, emote_kanim, emote_anims, play_mode);
	}

	protected override StatusItem GetStatusItem()
	{
		return (this.getStatusItem == null) ? base.GetStatusItem() : this.getStatusItem();
	}

	public override string ToString()
	{
		if (this.smi.emoteKAnim.IsValid())
		{
			return "ReactEmoteChore<" + this.smi.emoteKAnim + ">";
		}
		return "ReactEmoteChore<" + this.smi.emoteAnims[0] + ">";
	}

	private Func<StatusItem> getStatusItem;

	public class StatesInstance : GameStateMachine<ReactEmoteChore.States, ReactEmoteChore.StatesInstance, ReactEmoteChore, object>.GameInstance
	{
		public StatesInstance(ReactEmoteChore master, GameObject emoter, Reactable reactable, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode)
			: base(master)
		{
			this.emoteKAnim = emote_kanim;
			this.emoteAnims = emote_anims;
			this.mode = mode;
			base.sm.reactable.Set(reactable, base.smi);
			base.sm.emoter.Set(emoter, base.smi);
		}

		public HashedString[] emoteAnims;

		public HashedString emoteKAnim;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;
	}

	public class States : GameStateMachine<ReactEmoteChore.States, ReactEmoteChore.StatesInstance, ReactEmoteChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.emoter);
			this.root.ToggleThought((ReactEmoteChore.StatesInstance smi) => this.reactable.Get(smi).thought).ToggleExpression((ReactEmoteChore.StatesInstance smi) => this.reactable.Get(smi).expression).ToggleAnims((ReactEmoteChore.StatesInstance smi) => smi.emoteKAnim)
				.ToggleThought(Db.Get().Thoughts.Unhappy, null)
				.PlayAnims((ReactEmoteChore.StatesInstance smi) => smi.emoteAnims, (ReactEmoteChore.StatesInstance smi) => smi.mode)
				.OnAnimQueueComplete(null)
				.Enter(delegate(ReactEmoteChore.StatesInstance smi)
				{
					smi.master.GetComponent<Facing>().Face(Grid.CellToPos(this.reactable.Get(smi).CellPosition));
				});
		}

		public StateMachine<ReactEmoteChore.States, ReactEmoteChore.StatesInstance, ReactEmoteChore, object>.TargetParameter emoter;

		public StateMachine<ReactEmoteChore.States, ReactEmoteChore.StatesInstance, ReactEmoteChore, object>.ObjectParameter<Reactable> reactable;
	}
}
