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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.geneShufflerSMI = new GeneShuffler.GeneShufflerSM.Instance(this);
		this.geneShufflerSMI.StartSM();
		this.showProgressBar = false;
		if (!this.IsConsumed)
		{
			if (this.assignable.assignee != null)
			{
				if (!this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.consumed))
				{
					this.ActivateChore(null);
				}
				else
				{
					this.geneShufflerSMI.GoTo(this.geneShufflerSMI.sm.idle);
				}
			}
		}
		else
		{
			this.geneShufflerSMI.GoTo(this.geneShufflerSMI.sm.consumed);
		}
	}

	private void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != null)
		{
			if (this.geneShufflerSMI != null && !this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.consumed))
			{
				this.ActivateChore(null);
			}
		}
		else if (this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.idle))
		{
			this.CancelChore(null);
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.notification = new Notification(MISC.NOTIFICATIONS.GENESHUFFLER.NAME, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.GENESHUFFLER.TOOLTIP + notificationList.ReduceMessages(false), null, false, 0f, null, null);
		this.notifier.Add(this.notification, string.Empty);
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, true);
		}
		this.assignable.SetCanBeAssigned(false);
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
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, true);
		}
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
		}
		else
		{
			InfoDialogScreen infoDialogScreen2 = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text3 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_FAILURE, worker.GetProperName());
			infoDialogScreen2.SetHeader(UI.GENESHUFFLERMESSAGE.HEADER).AddPlainText(text3);
		}
	}

	public void ActivateChore(object param = null)
	{
		if (this.chore != null)
		{
			this.CancelChore(null);
		}
		base.GetComponent<Workable>().SetWorkTime(float.PositiveInfinity);
		ChoreType geneShuffle = Db.Get().ChoreTypes.GeneShuffle;
		KAnimFile anim = Assets.GetAnim("anim_interacts_neuralvacillator_kanim");
		this.chore = new WorkChore<Workable>(geneShuffle, this, null, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, false, true, anim, false, true, true, PriorityScreen.PriorityClass.emergency, 0, false);
		this.chore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, this.assignable);
		this.chore.AddPrecondition(ChorePreconditions.instance.IsOperational, this.assignable.gameObject.GetComponent<Operational>());
	}

	public void CancelChore(object param = null)
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

	[MyCmpReq]
	public Assignable assignable;

	[MyCmpAdd]
	public Notifier notifier;

	private Notification notification;

	private Chore chore;

	private GeneShuffler.GeneShufflerSM.Instance geneShufflerSMI;

	[Serialize]
	public bool IsConsumed;

	public class GeneShufflerSM : GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = false;
			default_state = this.idle;
			this.idle.PlayAnim("idle").WorkableStartTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pre);
			this.working.pre.PlayAnim("working_pre").EventTransition(GameHashes.AnimQueueComplete, this.working.loop, null);
			this.working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ScheduleGoTo(5f, this.working.complete);
			this.working.complete.ToggleStatusItem(Db.Get().BuildingStatusItems.GeneShuffleCompleted, null).Enter(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				KSelectable component = smi.master.GetComponent<KSelectable>();
				if (component.IsSelected)
				{
					DetailsScreen.Instance.Refresh(smi.master.gameObject);
				}
			}).WorkableStopTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pst);
			this.working.pst.EventTransition(GameHashes.AnimQueueComplete, this.consumed, null);
			this.consumed.PlayAnim("off", KAnim.PlayMode.Once).Enter(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				smi.master.IsConsumed = true;
			});
		}

		public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State idle;

		public GeneShuffler.GeneShufflerSM.WorkingStates working;

		public GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler, object>.State consumed;

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
