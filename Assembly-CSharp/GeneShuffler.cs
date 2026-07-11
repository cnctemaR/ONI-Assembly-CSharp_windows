using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeneShuffler : Workable
{
	public bool WorkComplete
	{
		get
		{
			return this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.working.complete);
		}
	}

	public bool IsWorking
	{
		get
		{
			return this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.working);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.assignable.OnAssign += this.Assign;
		this.lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.showProgressBar = false;
		this.geneShufflerSMI = new GeneShuffler.GeneShufflerSM.Instance(this);
		this.RefreshRechargeChore();
		this.RefreshConsumedState();
		base.Subscribe<GeneShuffler>(-1697596308, GeneShuffler.OnStorageChangeDelegate);
		this.geneShufflerSMI.StartSM();
	}

	private void Assign(IAssignableIdentity new_assignee)
	{
		this.CancelChore();
		if (new_assignee != null)
		{
			this.ActivateChore();
		}
	}

	private void Recharge()
	{
		this.SetConsumed(false);
		this.RequestRecharge(false);
		this.RefreshRechargeChore();
		this.RefreshSideScreen();
	}

	private void SetConsumed(bool consumed)
	{
		this.IsConsumed = consumed;
		this.RefreshConsumedState();
	}

	private void RefreshConsumedState()
	{
		this.geneShufflerSMI.sm.isCharged.Set(!this.IsConsumed, this.geneShufflerSMI);
	}

	private void OnStorageChange(object data)
	{
		if (this.storage_recursion_guard)
		{
			return;
		}
		this.storage_recursion_guard = true;
		if (this.IsConsumed)
		{
			for (int i = this.storage.items.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = this.storage.items[i];
				if (!(gameObject == null) && gameObject.HasTag(GeneShuffler.RechargeTag))
				{
					this.storage.ConsumeIgnoringDisease(gameObject);
					this.Recharge();
					break;
				}
			}
		}
		this.storage_recursion_guard = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.notification = new Notification(MISC.NOTIFICATIONS.GENESHUFFLER.NAME, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.GENESHUFFLER.TOOLTIP + notificationList.ReduceMessages(false), null, false, 0f, null, null, null);
		this.notifier.Add(this.notification, "");
		this.DeSelectBuilding();
	}

	private void DeSelectBuilding()
	{
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, true);
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		if (this.chore != null)
		{
			this.chore.Cancel("aborted");
		}
		this.notifier.Remove(this.notification);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (this.chore != null)
		{
			this.chore.Cancel("stopped");
		}
		this.notifier.Remove(this.notification);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		CameraController.Instance.CameraGoTo(base.transform.GetPosition(), 1f, false);
		this.ApplyRandomTrait(worker);
		this.assignable.Unassign();
		this.DeSelectBuilding();
		this.notifier.Remove(this.notification);
	}

	private void ApplyRandomTrait(Worker worker)
	{
		Traits component = worker.GetComponent<Traits>();
		List<string> list = new List<string>();
		foreach (DUPLICANTSTATS.TraitVal traitVal in DUPLICANTSTATS.GENESHUFFLERTRAITS)
		{
			if (!component.HasTrait(traitVal.id))
			{
				list.Add(traitVal.id);
			}
		}
		if (list.Count > 0)
		{
			string text = list[global::UnityEngine.Random.Range(0, list.Count)];
			Trait trait = Db.Get().traits.TryGet(text);
			worker.GetComponent<Traits>().Add(trait);
			InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text2 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_SUCCESS, worker.GetProperName(), trait.Name, trait.GetTooltip());
			infoDialogScreen.SetHeader(UI.GENESHUFFLERMESSAGE.HEADER).AddPlainText(text2);
			this.SetConsumed(true);
			return;
		}
		InfoDialogScreen infoDialogScreen2 = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		string text3 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_FAILURE, worker.GetProperName());
		infoDialogScreen2.SetHeader(UI.GENESHUFFLERMESSAGE.HEADER).AddPlainText(text3);
	}

	private void ActivateChore()
	{
		global::Debug.Assert(this.chore == null);
		base.GetComponent<Workable>().SetWorkTime(float.PositiveInfinity);
		this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.GeneShuffle, this, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, false, true, Assets.GetAnim("anim_interacts_neuralvacillator_kanim"), false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
	}

	private void CancelChore()
	{
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel("User cancelled");
		this.chore = null;
	}

	private void CompleteChore()
	{
		this.chore.Cleanup();
		this.chore = null;
	}

	public void RequestRecharge(bool request)
	{
		this.RechargeRequested = request;
		this.RefreshRechargeChore();
	}

	private void RefreshRechargeChore()
	{
		this.delivery.Pause(!this.RechargeRequested, "No recharge requested");
	}

	public void RefreshSideScreen()
	{
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			DetailsScreen.Instance.Refresh(base.gameObject);
		}
	}

	public void SetAssignable(bool set_it)
	{
		this.assignable.SetCanBeAssigned(set_it);
		this.RefreshSideScreen();
	}

	[MyCmpReq]
	public Assignable assignable;

	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public ManualDeliveryKG delivery;

	[MyCmpReq]
	public Storage storage;

	[Serialize]
	public bool IsConsumed;

	[Serialize]
	public bool RechargeRequested;

	private Chore chore;

	private GeneShuffler.GeneShufflerSM.Instance geneShufflerSMI;

	private Notification notification;

	private static Tag RechargeTag = new Tag("GeneShufflerRecharge");

	private static readonly EventSystem.IntraObjectHandler<GeneShuffler> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<GeneShuffler>(delegate(GeneShuffler component, object data)
	{
		component.OnStorageChange(data);
	});

	private bool storage_recursion_guard;

	public class GeneShufflerSM : GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.PlayAnim("on").Enter(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				smi.master.SetAssignable(true);
			}).Exit(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				smi.master.SetAssignable(false);
			})
				.WorkableStartTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pre)
				.ParamTransition<bool>(this.isCharged, this.consumed, GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.IsFalse);
			this.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working.loop);
			this.working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ScheduleGoTo(5f, this.working.complete);
			this.working.complete.ToggleStatusItem(Db.Get().BuildingStatusItems.GeneShuffleCompleted, null).Enter(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				smi.master.RefreshSideScreen();
			}).WorkableStopTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pst);
			this.working.pst.EventTransition(GameHashes.AnimQueueComplete, this.consumed, null);
			this.consumed.PlayAnim("off", KAnim.PlayMode.Once).ParamTransition<bool>(this.isCharged, this.recharging, GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.IsTrue);
			this.recharging.PlayAnim("recharging", KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
		}

		public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State idle;

		public GeneShuffler.GeneShufflerSM.WorkingStates working;

		public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State consumed;

		public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State recharging;

		public StateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.BoolParameter isCharged;

		public class WorkingStates : GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State
		{
			public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State pre;

			public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State loop;

			public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State complete;

			public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State pst;
		}

		public new class Instance : GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.GameInstance
		{
			public Instance(GeneShuffler master)
				: base(master)
			{
			}
		}
	}
}
