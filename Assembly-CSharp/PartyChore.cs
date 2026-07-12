using System;
using Klei.AI;
using TUNING;

public class PartyChore : Chore<PartyChore.StatesInstance>, IWorkerPrioritizable
{
	public PartyChore(IStateMachineTarget master, Workable chat_workable, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), true, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new PartyChore.StatesInstance(this);
		base.smi.sm.chitchatlocator.Set(chat_workable, base.smi);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, chat_workable);
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.partyer.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
		base.smi.sm.partyer.Get(base.smi).gameObject.AddTag(GameTags.Partying);
	}

	protected override void End(string reason)
	{
		if (base.smi.sm.partyer.Get(base.smi) != null)
		{
			base.smi.sm.partyer.Get(base.smi).gameObject.RemoveTag(GameTags.Partying);
		}
		base.End(reason);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		return true;
	}

	public int basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;

	public const string specificEffect = "Socialized";

	public const string trackingEffect = "RecentlySocialized";

	public class States : GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.stand;
			base.Target(this.partyer);
			this.stand.InitializeStates(this.partyer, this.masterTarget, this.chat, null, null, null);
			this.chat_move.InitializeStates(this.partyer, this.chitchatlocator, this.chat, null, null, null);
			this.chat.ToggleWork<Workable>(this.chitchatlocator, this.success, null, null);
			this.success.Enter(delegate(PartyChore.StatesInstance smi)
			{
				this.partyer.Get(smi).gameObject.GetComponent<Effects>().Add("RecentlyPartied", true);
			}).ReturnSuccess();
		}

		public StateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.TargetParameter partyer;

		public StateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.TargetParameter chitchatlocator;

		public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.ApproachSubState<IApproachable> stand;

		public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.ApproachSubState<IApproachable> chat_move;

		public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State chat;

		public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State success;
	}

	public class StatesInstance : GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.GameInstance
	{
		public StatesInstance(PartyChore master)
			: base(master)
		{
		}
	}
}
