using System;
using Klei.AI;
using UnityEngine;

public class EmoteChore : Chore<EmoteChore.StatesInstance>
{
	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, Emote emote, int emoteIterations = 1, Func<StatusItem> get_status_item = null)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote, KAnim.PlayMode.Once, emoteIterations, false);
		this.getStatusItem = get_status_item;
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, Emote emote, KAnim.PlayMode play_mode, int emoteIterations = 1, bool flip_x = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote, play_mode, emoteIterations, flip_x);
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString animFile, HashedString[] anims, Func<StatusItem> get_status_item = null)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new EmoteChore.StatesInstance(this, target.gameObject, animFile, anims, KAnim.PlayMode.Once, false);
		this.getStatusItem = get_status_item;
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString animFile, HashedString[] anims, KAnim.PlayMode play_mode, bool flip_x = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new EmoteChore.StatesInstance(this, target.gameObject, animFile, anims, play_mode, flip_x);
	}

	protected override StatusItem GetStatusItem()
	{
		if (this.getStatusItem == null)
		{
			return base.GetStatusItem();
		}
		return this.getStatusItem();
	}

	public override string ToString()
	{
		if (base.smi.animFile != null)
		{
			return "EmoteChore<" + base.smi.animFile.GetData().name + ">";
		}
		string text = "EmoteChore<";
		HashedString hashedString = base.smi.emoteAnims[0];
		return text + hashedString.ToString() + ">";
	}

	public void PairReactable(SelfEmoteReactable reactable)
	{
		this.reactable = reactable;
	}

	protected new virtual void End(string reason)
	{
		if (this.reactable != null)
		{
			this.reactable.PairEmote(null);
			this.reactable.Cleanup();
			this.reactable = null;
		}
		base.End(reason);
	}

	private Func<StatusItem> getStatusItem;

	private SelfEmoteReactable reactable;

	public class StatesInstance : GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.GameInstance
	{
		public StatesInstance(EmoteChore master, GameObject emoter, Emote emote, KAnim.PlayMode mode, int emoteIterations, bool flip_x)
			: base(master)
		{
			this.mode = mode;
			this.animFile = EmoteChore.StatesInstance.ResolveAnimFile(emoter, emote);
			emote.CollectStepAnims(out this.emoteAnims, emoteIterations);
			base.sm.emoter.Set(emoter, base.smi, false);
		}

		public StatesInstance(EmoteChore master, GameObject emoter, HashedString animFile, HashedString[] anims, KAnim.PlayMode mode, bool flip_x)
			: base(master)
		{
			this.mode = mode;
			this.animFile = Assets.GetAnim(animFile);
			this.emoteAnims = anims;
			base.sm.emoter.Set(emoter, base.smi, false);
		}

		private static KAnimFile ResolveAnimFile(GameObject emoter, Emote emote)
		{
			Navigator navigator;
			if (!(emote.ManifestSwimAnimSet() != null) || !emoter.TryGetComponent<Navigator>(out navigator) || navigator.CurrentNavType != NavType.Swim)
			{
				return emote.AnimSet;
			}
			return emote.ManifestSwimAnimSet();
		}

		public KAnimFile animFile;

		public HashedString[] emoteAnims;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;
	}

	public class States : GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.emoter);
			this.root.ToggleAnims((EmoteChore.StatesInstance smi) => smi.animFile).PlayAnims((EmoteChore.StatesInstance smi) => smi.emoteAnims, (EmoteChore.StatesInstance smi) => smi.mode).ScheduleGoTo(10f, this.finish)
				.OnAnimQueueComplete(this.finish);
			this.finish.ReturnSuccess();
		}

		public StateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.TargetParameter emoter;

		public GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.State finish;
	}
}
