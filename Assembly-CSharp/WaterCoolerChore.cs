using System;
using Klei;
using Klei.AI;
using TUNING;

public class WaterCoolerChore : Chore<WaterCoolerChore.StatesInstance>, IWorkerPrioritizable
{
	public WaterCoolerChore(IStateMachineTarget master, Workable chat_workable, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(Db.Get().ChoreTypes.Relax, master, master.GetComponent<ChoreProvider>(), true, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new WaterCoolerChore.StatesInstance(this);
		base.smi.sm.chitchatlocator.Set(chat_workable, base.smi);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, chat_workable);
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		base.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.drinker.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.trackingEffect) && component.HasEffect(this.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.specificEffect) && component.HasEffect(this.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	public int basePriority = RELAXATION.PRIORITY.TIER2;

	public string specificEffect = "Socialized";

	public string trackingEffect = "RecentlySocialized";

	public class States : GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.drink_move;
			base.Target(this.drinker);
			this.drink_move.InitializeStates(this.drinker, this.masterTarget, this.drink, null, null, null);
			this.drink.ToggleAnims("anim_interacts_watercooler_kanim", 0f, "").DefaultState(this.drink.drink);
			this.drink.drink.Face(this.masterTarget, 0.5f).PlayAnim("working_pre").QueueAnim("working_loop", false, null)
				.OnAnimQueueComplete(this.drink.post);
			this.drink.post.Enter("Drink", new StateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State.Callback(this.Drink)).PlayAnim("working_pst").OnAnimQueueComplete(this.chat_move);
			this.chat_move.InitializeStates(this.drinker, this.chitchatlocator, this.chat, null, null, null);
			this.chat.ToggleWork<SocialGatheringPointWorkable>(this.chitchatlocator, this.success, null, null);
			this.success.ReturnSuccess();
		}

		private void Drink(WaterCoolerChore.StatesInstance smi)
		{
			Storage storage = this.masterTarget.Get<Storage>(smi);
			Worker worker = this.stateTarget.Get<Worker>(smi);
			Tag tag = storage.items[0].PrefabID();
			float num;
			SimUtil.DiseaseInfo diseaseInfo;
			float num2;
			storage.ConsumeAndGetDisease(tag, 1f, out num, out diseaseInfo, out num2);
			GermExposureMonitor.Instance smi2 = worker.GetSMI<GermExposureMonitor.Instance>();
			if (smi2 != null)
			{
				smi2.TryInjectDisease(diseaseInfo.idx, diseaseInfo.count, tag, Sickness.InfectionVector.Digestion);
			}
			Effects component = worker.GetComponent<Effects>();
			if (!string.IsNullOrEmpty(smi.master.trackingEffect))
			{
				component.Add(smi.master.trackingEffect, true);
			}
			if (tag == SimHashes.Milk.CreateTag())
			{
				component.Add("DuplicantGotMilk", true);
			}
		}

		public StateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.TargetParameter drinker;

		public StateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.TargetParameter chitchatlocator;

		public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.ApproachSubState<WaterCooler> drink_move;

		public WaterCoolerChore.States.DrinkStates drink;

		public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.ApproachSubState<IApproachable> chat_move;

		public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State chat;

		public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State success;

		public class DrinkStates : GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State
		{
			public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State drink;

			public GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.State post;
		}
	}

	public class StatesInstance : GameStateMachine<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object>.GameInstance
	{
		public StatesInstance(WaterCoolerChore master)
			: base(master)
		{
		}
	}
}
