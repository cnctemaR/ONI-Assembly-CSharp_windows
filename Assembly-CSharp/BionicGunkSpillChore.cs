using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicGunkSpillChore : Chore<BionicGunkSpillChore.StatesInstance>
{
	public static bool HasSuit(BionicGunkSpillChore.StatesInstance smi)
	{
		return smi.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
	}

	public static void ExpellGunkUpdate(BionicGunkSpillChore.StatesInstance smi, float dt)
	{
		float num = GunkMonitor.GUNK_CAPACITY * (dt / 10f);
		if (num >= smi.gunkMonitor.CurrentGunkMass)
		{
			smi.GoTo(smi.sm.pst);
			return;
		}
		smi.gunkMonitor.ExpellGunk(num, null);
	}

	public BionicGunkSpillChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.ExpellGunk, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BionicGunkSpillChore.StatesInstance(this, target.gameObject);
	}

	public const float EVENT_DURATION = 10f;

	public const string PRE_ANIM_NAME = "oiloverload_pre";

	public const string LOOP_ANIM_NAME = "oiloverload_loop";

	public const string PST_ANIM_NAME = "overload_pst";

	public const string SUIT_PRE_ANIM_NAME = "oiloverload_helmet_pre";

	public const string SUIT_LOOP_ANIM_NAME = "oiloverload_helmet_loop";

	public const string SUIT_PST_ANIM_NAME = "oiloverload_helmet_pst";

	public class States : GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.enter;
			base.Target(this.worker);
			this.root.ToggleAnims("anim_bionic_oil_overload_kanim", 0f).ToggleEffect("ExpellingGunk").ToggleTag(GameTags.MakingMess)
				.DoNotification((BionicGunkSpillChore.StatesInstance smi) => smi.stressfullyEmptyingGunk)
				.Enter(delegate(BionicGunkSpillChore.StatesInstance smi)
				{
					if (Sim.IsRadiationEnabled() && smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0f)
					{
						smi.master.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, null);
					}
				});
			this.enter.DefaultState(this.enter.noSuit);
			this.enter.noSuit.EventTransition(GameHashes.EquippedItemEquipper, this.enter.suit, new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit)).PlayAnim("oiloverload_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.running);
			this.enter.suit.EventTransition(GameHashes.UnequippedItemEquipper, this.enter.noSuit, GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Not(new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit))).PlayAnim("oiloverload_helmet_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.running);
			this.running.DefaultState(this.running.noSuit).Update(new Action<BionicGunkSpillChore.StatesInstance, float>(BionicGunkSpillChore.ExpellGunkUpdate), UpdateRate.SIM_200ms, false);
			this.running.noSuit.EventTransition(GameHashes.EquippedItemEquipper, this.running.suit, new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit)).PlayAnim("oiloverload_loop", KAnim.PlayMode.Loop);
			this.running.suit.EventTransition(GameHashes.UnequippedItemEquipper, this.running.noSuit, GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Not(new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit))).PlayAnim("oiloverload_helmet_loop", KAnim.PlayMode.Loop);
			this.pst.DefaultState(this.pst.noSuit);
			this.pst.noSuit.EventTransition(GameHashes.EquippedItemEquipper, this.pst.suit, new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit)).PlayAnim("overload_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete);
			this.pst.suit.EventTransition(GameHashes.UnequippedItemEquipper, this.pst.noSuit, GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Not(new StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.Transition.ConditionCallback(BionicGunkSpillChore.HasSuit))).PlayAnim("oiloverload_helmet_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete);
			this.complete.ReturnSuccess();
		}

		public BionicGunkSpillChore.States.SuitAnimState enter;

		public BionicGunkSpillChore.States.SuitAnimState running;

		public BionicGunkSpillChore.States.SuitAnimState pst;

		public GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.State complete;

		public StateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.TargetParameter worker;

		public class SuitAnimState : GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.State
		{
			public GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.State noSuit;

			public GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.State suit;
		}
	}

	public class StatesInstance : GameStateMachine<BionicGunkSpillChore.States, BionicGunkSpillChore.StatesInstance, BionicGunkSpillChore, object>.GameInstance
	{
		public StatesInstance(BionicGunkSpillChore master, GameObject worker)
			: base(master)
		{
			this.gunkMonitor = worker.GetSMI<GunkMonitor.Instance>();
			base.sm.worker.Set(worker, base.smi, false);
		}

		public Notification stressfullyEmptyingGunk = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGOIL.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGOIL.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true, false, false);

		public GunkMonitor.Instance gunkMonitor;
	}
}
