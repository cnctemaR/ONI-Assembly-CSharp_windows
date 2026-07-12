using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class WarpPortal : Workable
{
	public bool ReadyToWarp
	{
		get
		{
			return this.warpPortalSMI.IsInsideState(this.warpPortalSMI.sm.occupied.waiting);
		}
	}

	public bool IsWorking
	{
		get
		{
			return this.warpPortalSMI.IsInsideState(this.warpPortalSMI.sm.occupied);
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
		this.warpPortalSMI = new WarpPortal.WarpPortalSM.Instance(this);
		this.warpPortalSMI.sm.isCharged.Set(!this.IsConsumed, this.warpPortalSMI, false);
		this.warpPortalSMI.StartSM();
		this.selectEventHandle = Game.Instance.Subscribe(-1503271301, new Action<object>(this.OnObjectSelected));
	}

	private void OnObjectSelected(object data)
	{
		if (data != null && (GameObject)data == base.gameObject && Components.LiveMinionIdentities.Count > 0)
		{
			this.Discover();
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(this.selectEventHandle);
		base.OnCleanUp();
	}

	private void Discover()
	{
		if (this.discovered)
		{
			return;
		}
		ClusterManager.Instance.GetWorld(this.GetTargetWorldID()).SetDiscovered(true);
		SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.WarpWorldReveal, -1).smi as SimpleEvent.StatesInstance;
		statesInstance.minions = new GameObject[] { Components.LiveMinionIdentities[0].gameObject };
		statesInstance.callback = delegate
		{
			ManagementMenu.Instance.OpenClusterMap();
			ClusterMapScreen.Instance.SetTargetFocusPosition(ClusterManager.Instance.GetWorld(this.GetTargetWorldID()).GetMyWorldLocation(), 0.5f);
		};
		statesInstance.ShowEventPopup();
		this.discovered = true;
	}

	public void StartWarpSequence()
	{
		this.warpPortalSMI.GoTo(this.warpPortalSMI.sm.occupied.warping);
	}

	public void CancelAssignment()
	{
		this.CancelChore();
		this.assignable.Unassign();
		this.warpPortalSMI.GoTo(this.warpPortalSMI.sm.idle);
	}

	private int GetTargetWorldID()
	{
		SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag(WarpReceiverConfig.ID);
		foreach (WarpReceiver warpReceiver in global::UnityEngine.Object.FindObjectsOfType<WarpReceiver>())
		{
			if (warpReceiver.GetMyWorldId() != this.GetMyWorldId())
			{
				return warpReceiver.GetMyWorldId();
			}
		}
		global::Debug.LogError("No receiver world found for warp portal sender");
		return -1;
	}

	private void Warp()
	{
		if (base.worker == null || base.worker.HasTag(GameTags.Dying) || base.worker.HasTag(GameTags.Dead))
		{
			return;
		}
		WarpReceiver warpReceiver = null;
		foreach (WarpReceiver warpReceiver2 in global::UnityEngine.Object.FindObjectsOfType<WarpReceiver>())
		{
			if (warpReceiver2.GetMyWorldId() != this.GetMyWorldId())
			{
				warpReceiver = warpReceiver2;
				break;
			}
		}
		if (warpReceiver == null)
		{
			SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag(WarpReceiverConfig.ID);
			warpReceiver = global::UnityEngine.Object.FindObjectOfType<WarpReceiver>();
		}
		if (warpReceiver != null)
		{
			this.delayWarpRoutine = base.StartCoroutine(this.DelayedWarp(warpReceiver));
		}
		else
		{
			global::Debug.LogWarning("No warp receiver found - maybe POI stomping or failure to spawn?");
		}
		if (SelectTool.Instance.selected == base.GetComponent<KSelectable>())
		{
			SelectTool.Instance.Select(null, true);
		}
	}

	public IEnumerator DelayedWarp(WarpReceiver receiver)
	{
		yield return SequenceUtil.WaitForEndOfFrame;
		int myWorldId = receiver.GetMyWorldId();
		CameraController.Instance.ActiveWorldStarWipe(myWorldId, Grid.CellToPos(Grid.PosToCell(receiver)), 10f, null);
		Worker worker = base.worker;
		worker.StopWork();
		receiver.ReceiveWarpedDuplicant(worker);
		ClusterManager.Instance.MigrateMinion(worker.GetComponent<MinionIdentity>(), myWorldId);
		this.delayWarpRoutine = null;
		yield break;
	}

	public void SetAssignable(bool set_it)
	{
		this.assignable.SetCanBeAssigned(set_it);
		this.RefreshSideScreen();
	}

	private void Assign(IAssignableIdentity new_assignee)
	{
		this.CancelChore();
		if (new_assignee != null)
		{
			this.ActivateChore();
		}
	}

	private void ActivateChore()
	{
		global::Debug.Assert(this.chore == null);
		this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.Migrate, this, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, false, true, Assets.GetAnim("anim_interacts_warp_portal_sender_kanim"), false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		base.SetWorkTime(float.PositiveInfinity);
		this.workLayer = Grid.SceneLayer.Building;
		this.workAnims = new HashedString[] { "sending_pre", "sending_loop" };
		this.workingPstComplete = new HashedString[] { "sending_pst" };
		this.workingPstFailed = new HashedString[] { "idle_loop" };
		this.showProgressBar = false;
	}

	private void CancelChore()
	{
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel("User cancelled");
		this.chore = null;
		if (this.delayWarpRoutine != null)
		{
			base.StopCoroutine(this.delayWarpRoutine);
			this.delayWarpRoutine = null;
		}
	}

	private void CompleteChore()
	{
		this.IsConsumed = true;
		this.chore.Cleanup();
		this.chore = null;
	}

	public void RefreshSideScreen()
	{
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			DetailsScreen.Instance.Refresh(base.gameObject);
		}
	}

	[MyCmpReq]
	public Assignable assignable;

	[MyCmpAdd]
	public Notifier notifier;

	private Chore chore;

	private WarpPortal.WarpPortalSM.Instance warpPortalSMI;

	private Notification notification;

	public const float RECHARGE_TIME = 3000f;

	[Serialize]
	public bool IsConsumed;

	[Serialize]
	public float rechargeProgress;

	[Serialize]
	private bool discovered;

	private int selectEventHandle = -1;

	private Coroutine delayWarpRoutine;

	private static readonly HashedString[] printing_anim = new HashedString[] { "printing_pre", "printing_loop", "printing_pst" };

	public class WarpPortalSM : GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			this.root.Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				if (smi.master.rechargeProgress != 0f)
				{
					smi.GoTo(this.recharging);
				}
			}).DefaultState(this.idle);
			this.idle.PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				smi.master.IsConsumed = false;
				smi.sm.isCharged.Set(true, smi, false);
				smi.master.SetAssignable(true);
			}).Exit(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				smi.master.SetAssignable(false);
			})
				.WorkableStartTransition((WarpPortal.WarpPortalSM.Instance smi) => smi.master, this.become_occupied)
				.ParamTransition<bool>(this.isCharged, this.recharging, GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.IsFalse);
			this.become_occupied.Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				this.worker.Set(smi.master.worker, smi);
				smi.GoTo(this.occupied.get_on);
			});
			this.occupied.OnTargetLost(this.worker, this.idle).Target(this.worker).TagTransition(GameTags.Dying, this.idle, false)
				.Target(this.masterTarget)
				.Exit(delegate(WarpPortal.WarpPortalSM.Instance smi)
				{
					this.worker.Set(null, smi);
				});
			this.occupied.get_on.PlayAnim("sending_pre").OnAnimQueueComplete(this.occupied.waiting);
			this.occupied.waiting.PlayAnim("sending_loop", KAnim.PlayMode.Loop).ToggleNotification((WarpPortal.WarpPortalSM.Instance smi) => smi.CreateDupeWaitingNotification()).Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				smi.master.RefreshSideScreen();
			})
				.Exit(delegate(WarpPortal.WarpPortalSM.Instance smi)
				{
					smi.master.RefreshSideScreen();
				});
			this.occupied.warping.PlayAnim("sending_pst").OnAnimQueueComplete(this.do_warp);
			this.do_warp.Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				smi.master.Warp();
			}).GoTo(this.recharging);
			this.recharging.Enter(delegate(WarpPortal.WarpPortalSM.Instance smi)
			{
				smi.master.SetAssignable(false);
				smi.master.IsConsumed = true;
				this.isCharged.Set(false, smi, false);
			}).PlayAnim("recharge", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().BuildingStatusItems.WarpPortalCharging, (WarpPortal.WarpPortalSM.Instance smi) => smi.master)
				.Update(delegate(WarpPortal.WarpPortalSM.Instance smi, float dt)
				{
					smi.master.rechargeProgress += dt;
					if (smi.master.rechargeProgress > 3000f)
					{
						this.isCharged.Set(true, smi, false);
						smi.master.rechargeProgress = 0f;
						smi.GoTo(this.idle);
					}
				}, UpdateRate.SIM_200ms, false);
		}

		public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State idle;

		public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State become_occupied;

		public WarpPortal.WarpPortalSM.OccupiedStates occupied;

		public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State do_warp;

		public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State recharging;

		public StateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.BoolParameter isCharged;

		private StateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.TargetParameter worker;

		public class OccupiedStates : GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State
		{
			public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State get_on;

			public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State waiting;

			public GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.State warping;
		}

		public new class Instance : GameStateMachine<WarpPortal.WarpPortalSM, WarpPortal.WarpPortalSM.Instance, WarpPortal, object>.GameInstance
		{
			public Instance(WarpPortal master)
				: base(master)
			{
			}

			public Notification CreateDupeWaitingNotification()
			{
				if (base.master.worker != null)
				{
					return new Notification(MISC.NOTIFICATIONS.WARP_PORTAL_DUPE_READY.NAME.Replace("{dupe}", base.master.worker.name), NotificationType.Neutral, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.WARP_PORTAL_DUPE_READY.TOOLTIP.Replace("{dupe}", base.master.worker.name), null, false, 0f, null, null, base.master.transform, true, false);
				}
				return null;
			}
		}
	}
}
