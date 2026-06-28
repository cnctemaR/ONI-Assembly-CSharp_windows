using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeneShuffler : Ownable
{
	public Assignable Assignable
	{
		get
		{
			return this.assignable;
		}
	}

	public bool WorkComplete
	{
		get
		{
			return this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.working.complete);
		}
	}

	public bool IsConsumed
	{
		get
		{
			return this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.consumed);
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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.geneShufflerSMI = new GeneShuffler.GeneShufflerSM.Instance(this);
		this.geneShufflerSMI.StartSM();
		this.showProgressBar = false;
	}

	protected override void SetAssignables(Assignables assignables)
	{
		base.SetAssignables(assignables);
		if (!this.geneShufflerSMI.IsInsideState(this.geneShufflerSMI.sm.consumed))
		{
			this.ActivateChore(null);
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.notification = new Notification(MISC.NOTIFICATIONS.GENESHUFFLER.NAME, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.GENESHUFFLER.TOOLTIP + notificationList.ReduceMessages(false), null, false, 0f, null, null, null);
		this.notifier.Add(this.notification, string.Empty);
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, true);
		}
		base.SetCanBeAssigned(false);
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
		CameraController.Instance.CameraGoTo(this.transform.position, 1f, false);
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
			LoreDialogScreen loreDialogScreen = (LoreDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.LoreDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text2 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_SUCCESS, worker.GetProperName(), trait.Name, trait.GetTooltip());
			loreDialogScreen.PopupLoreDialog(text2, UI.GENESHUFFLERMESSAGE.HEADER, null);
		}
		else
		{
			LoreDialogScreen loreDialogScreen2 = (LoreDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.LoreDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			string text3 = string.Format(UI.GENESHUFFLERMESSAGE.BODY_FAILURE, worker.GetProperName());
			loreDialogScreen2.PopupLoreDialog(text3, UI.GENESHUFFLERMESSAGE.HEADER, null);
		}
	}

	public void ActivateChore(object param = null)
	{
		if (this.chore != null)
		{
			this.CancelChore(null);
		}
		base.GetComponent<Workable>().SetWorkTime(float.PositiveInfinity);
		Action<Chore> action = delegate(Chore o)
		{
			this.CompleteChore();
		};
		KAnimFile anim = Assets.GetAnim("anim_interacts_neuralvacillator_kanim");
		this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.GeneShuffle, this, null, true, action, null, null, true, null, true, default(Tag), anim, false, true, true);
		this.chore.AddPrecondition(ChorePreconditions.IsAssignedtoMe, this.assignable);
		this.chore.AddPrecondition(ChorePreconditions.IsOperational, this.assignable.gameObject);
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

	[MyCmpGet]
	public Assignable assignable;

	[MyCmpGet]
	public Notifier notifier;

	private Notification notification;

	private Chore chore;

	private GeneShuffler.GeneShufflerSM.Instance geneShufflerSMI;

	public class GeneShufflerSM : GameStateMachine<GeneShuffler.GeneShufflerSM, GeneShuffler.GeneShufflerSM.Instance, GeneShuffler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.idle;
			this.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).WorkableStartTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pre);
			this.working.pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).EventTransition(GameHashes.AnimQueueComplete, this.working.loop, null);
			this.working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).ScheduleGoTo(5f, this.working.complete);
			this.working.complete.ToggleStatusItem(Db.Get().BuildingStatusItems.GeneShuffleCompleted, null).Enter(delegate(GeneShuffler.GeneShufflerSM.Instance smi)
			{
				if (smi.master.selectable.IsSelected)
				{
					DetailsScreen.Instance.Refresh(smi.master.gameObject);
				}
			}).WorkableStopTransition((GeneShuffler.GeneShufflerSM.Instance smi) => smi.master, this.working.pst);
			this.working.pst.EventTransition(GameHashes.AnimQueueComplete, this.consumed, null);
			this.consumed.PlayAnim("off", KAnim.PlayMode.Once, null);
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
