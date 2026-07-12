using System;
using STRINGS;

public class RanchedStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.ranch;
		this.root.Exit("AbandonedRanchStation", delegate(RanchedStates.Instance smi)
		{
			RanchStation.Instance ranchStation = smi.GetRanchStation();
			if (ranchStation == null)
			{
				return;
			}
			ranchStation.Abandon(smi.Monitor);
		});
		this.ranch.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.SubscribeToRancherStateChanges)).EventHandler(GameHashes.RanchStationNoLongerAvailable, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.OnRanchStationNotAvailable)).BehaviourComplete(GameTags.Creatures.WantsToGetRanched, true)
			.Update(delegate(RanchedStates.Instance smi, float deltaSeconds)
			{
				RanchStation.Instance ranchStation2 = smi.GetRanchStation();
				if (ranchStation2.IsNullOrDestroyed())
				{
					smi.StopSM("No more target ranch station.");
					return;
				}
				Option<CavityInfo> option = Option.Maybe<CavityInfo>(Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi)));
				Option<CavityInfo> cavityInfo = ranchStation2.GetCavityInfo();
				if (option.IsNone() || cavityInfo.IsNone())
				{
					smi.StopSM("No longer in any cavity.");
					return;
				}
				if (option.Unwrap() != cavityInfo.Unwrap())
				{
					smi.StopSM("Critter is in a different cavity");
					return;
				}
			}, UpdateRate.SIM_200ms, false)
			.Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.UnsubscribeFromRancherStateChanges))
			.Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride));
		this.ranch.Cheer.ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter("FaceRancher", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetRanchStation().transform.GetPosition());
		}).PlayAnim("excited_loop")
			.OnAnimQueueComplete(this.ranch.Cheer.Pst);
		this.ranch.Cheer.Pst.ScheduleGoTo(0.2f, this.ranch.Move);
		this.ranch.Move.DefaultState(this.ranch.Move.MoveToRanch).Enter("Speedup", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed * 1.25f;
		}).ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main)
			.Exit("RestoreSpeed", delegate(RanchedStates.Instance smi)
			{
				smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed;
			});
		this.ranch.Move.MoveToRanch.EnterTransition(this.ranch.Move.WaitInLine, GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Not(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn))).MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRanchNavTarget), this.ranch.Move.WaitInLine, null, false);
		this.ranch.Move.WaitInLine.EnterTransition(this.ranch.Ranching, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn)).Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.MoveToWaitPosition)).EventHandler(GameHashes.DestinationReached, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.Wait));
		this.ranch.Ranching.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.GetOnTable)).Enter("SetCreatureAtRanchingStation", delegate(RanchedStates.Instance smi)
		{
			smi.GetRanchStation().MessageCreatureArrived(smi);
			smi.AnimController.SetSceneLayer(Grid.SceneLayer.BuildingUse);
		}).EventTransition(GameHashes.RanchingComplete, this.ranch.Wavegoodbye, null)
			.ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_RANCHED.NAME, CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.ranch.Wavegoodbye.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride)).OnAnimQueueComplete(this.ranch.Runaway).ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.ranch.Runaway.MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRunawayCell), null, null, false).ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
	}

	private static void ClearLayerOverride(RanchedStates.Instance smi)
	{
		smi.AnimController.SetSceneLayer(Grid.SceneLayer.Creatures);
	}

	private static RanchStation.Instance GetRanchStation(RanchedStates.Instance smi)
	{
		return smi.GetRanchStation();
	}

	private static GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State GetInitialRanchState(RanchedStates.Instance smi)
	{
		if (!RanchedStates.IsCrittersTurn(smi))
		{
			return smi.sm.ranch.Move.WaitInLine;
		}
		return smi.sm.ranch.Cheer;
	}

	private static void OnRanchStationNotAvailable(RanchedStates.Instance smi)
	{
		smi.GoTo(null);
	}

	private static void GetOnTable(RanchedStates.Instance smi)
	{
		Navigator navigator = smi.Get<Navigator>();
		if (navigator.IsValidNavType(NavType.Floor))
		{
			navigator.SetCurrentNavType(NavType.Floor);
		}
		smi.Get<Facing>().SetFacing(false);
	}

	private static bool IsCrittersTurn(RanchedStates.Instance smi)
	{
		RanchStation.Instance ranchStation = RanchedStates.GetRanchStation(smi);
		return ranchStation != null && ranchStation.IsRancherReady && ranchStation.TryGetRanched(smi);
	}

	private static int GetRanchNavTarget(RanchedStates.Instance smi)
	{
		RanchStation.Instance ranchStation = RanchedStates.GetRanchStation(smi);
		return smi.ModifyNavTargetForCritter(ranchStation.GetRanchNavTarget());
	}

	private static void MoveToWaitPosition(RanchedStates.Instance smi)
	{
		smi.EnterQueue();
	}

	private static void Wait(RanchedStates.Instance smi)
	{
		RanchStation.Instance targetRanchStation = smi.Monitor.TargetRanchStation;
		smi.Monitor.NavComponent.IsFacingLeft = targetRanchStation.transform.position.x - smi.transform.position.x < 0f;
		smi.AnimController.Queue(smi.def.StartWaitingAnim, KAnim.PlayMode.Once, 1f, 0f);
		smi.AnimController.Play(smi.def.WaitingAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private static void SubscribeToRancherStateChanges(RanchedStates.Instance smi)
	{
		RanchStation.Instance ranchStation = smi.GetRanchStation();
		if (ranchStation == null)
		{
			return;
		}
		RanchStation.Instance instance = ranchStation;
		instance.RancherStateChanged = (Action<RanchStation.Instance>)Delegate.Combine(instance.RancherStateChanged, new Action<RanchStation.Instance>(smi.OnRancherStateChanged));
		if (ranchStation.IsRancherReady)
		{
			smi.OnRancherStateChanged(ranchStation);
		}
	}

	private static void UnsubscribeFromRancherStateChanges(RanchedStates.Instance smi)
	{
		RanchStation.Instance ranchStation = smi.GetRanchStation();
		if (ranchStation == null)
		{
			return;
		}
		RanchStation.Instance instance = ranchStation;
		instance.RancherStateChanged = (Action<RanchStation.Instance>)Delegate.Remove(instance.RancherStateChanged, new Action<RanchStation.Instance>(smi.OnRancherStateChanged));
	}

	private static int GetRunawayCell(RanchedStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.transform.GetPosition());
		int num2 = Grid.OffsetCell(num, 2, 0);
		if (Grid.Solid[num2])
		{
			num2 = Grid.OffsetCell(num, -2, 0);
		}
		return num2;
	}

	private RanchedStates.RanchStates ranch;

	public class Def : StateMachine.BaseDef
	{
		public bool IsQueueAnim(HashedString anim)
		{
			return (this.StartWaitingAnim == anim) | (this.WaitingAnim == anim) | (this.EndWaitingAnim == anim);
		}

		public HashedString StartWaitingAnim = "queue_pre";

		public HashedString WaitingAnim = "queue_loop";

		public HashedString EndWaitingAnim = "queue_pst";

		public int WaitCellOffset = 1;
	}

	public new class Instance : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.GameInstance
	{
		public RanchableMonitor.Instance Monitor
		{
			get
			{
				if (this.ranchMonitor == null)
				{
					this.ranchMonitor = this.GetSMI<RanchableMonitor.Instance>();
				}
				return this.ranchMonitor;
			}
		}

		public KBatchedAnimController AnimController
		{
			get
			{
				return this.animController;
			}
		}

		public Instance(Chore<RanchedStates.Instance> chore, RanchedStates.Def def)
			: base(chore, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.OriginalSpeed = this.Monitor.NavComponent.defaultSpeed;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetRanched);
		}

		public override void StartSM()
		{
			base.StartSM();
			this.animController.onAnimComplete += this.OnAnimComplete;
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			this.animController.onAnimComplete -= this.OnAnimComplete;
		}

		public RanchStation.Instance GetRanchStation()
		{
			if (this.Monitor != null)
			{
				return this.Monitor.TargetRanchStation;
			}
			return null;
		}

		public void EnterQueue()
		{
			this.InitializeWaitCell();
			this.Monitor.NavComponent.GoTo(this.waitCell, null);
		}

		public void ExitQueue()
		{
			if (!RanchedStates.IsCrittersTurn(this))
			{
				return;
			}
			if (this.animController.HasAnimation(base.def.EndWaitingAnim) && this.animController.currentAnim != base.def.EndWaitingAnim && base.def.IsQueueAnim(this.animController.currentAnim))
			{
				this.animController.Play(base.def.EndWaitingAnim, KAnim.PlayMode.Once, 1f, 0f);
				return;
			}
			this.GoTo(base.sm.ranch.Move.MoveToRanch);
		}

		public void AbandonRanchStation()
		{
			if (this.Monitor.TargetRanchStation == null || this.status == StateMachine.Status.Failed)
			{
				return;
			}
			this.StopSM("Abandoned Ranch");
		}

		public int ModifyNavTargetForCritter(int navCell)
		{
			if (base.smi.HasTag(GameTags.Creatures.Flyer))
			{
				return Grid.CellAbove(navCell);
			}
			return navCell;
		}

		private void InitializeWaitCell()
		{
			if (this.Monitor == null)
			{
				return;
			}
			int num = 0;
			Extents stationExtents = this.Monitor.TargetRanchStation.StationExtents;
			int num2 = this.ModifyNavTargetForCritter(Grid.XYToCell(stationExtents.x, stationExtents.y));
			int num3 = 0;
			int num4;
			if (Grid.Raycast(num2, new Vector2I(-1, 0), out num4, base.def.WaitCellOffset, ~(Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable)))
			{
				num3 = 1 + base.def.WaitCellOffset - num4;
				num = this.ModifyNavTargetForCritter(Grid.XYToCell(stationExtents.x + 1, stationExtents.y));
			}
			int num5 = 0;
			int num6;
			if (num3 != 0 && Grid.Raycast(num, new Vector2I(1, 0), out num6, base.def.WaitCellOffset, ~(Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable)))
			{
				num5 = base.def.WaitCellOffset - num6;
			}
			int num7 = (base.def.WaitCellOffset - num3) * -1;
			if (num3 == base.def.WaitCellOffset)
			{
				num7 = 1 + base.def.WaitCellOffset - num5;
			}
			CellOffset cellOffset = new CellOffset(num7, 0);
			this.waitCell = Grid.OffsetCell(num2, cellOffset);
		}

		public void OnRancherStateChanged(RanchStation.Instance ranch)
		{
			RanchedStates.IRanchStatesCallbacks ranchStatesCallbacks = base.smi.GetCurrentState() as RanchedStates.IRanchStatesCallbacks;
			if (ranchStatesCallbacks == null)
			{
				return;
			}
			ranchStatesCallbacks.OnRancherStateChanged(this);
		}

		private void OnAnimComplete(HashedString completedAnim)
		{
			RanchedStates.IRanchStatesCallbacks ranchStatesCallbacks = base.smi.GetCurrentState() as RanchedStates.IRanchStatesCallbacks;
			if (ranchStatesCallbacks == null)
			{
				return;
			}
			ranchStatesCallbacks.OnAnimComplete(this, completedAnim);
		}

		public float OriginalSpeed;

		private int waitCell;

		private KBatchedAnimController animController;

		private RanchableMonitor.Instance ranchMonitor;
	}

	public class RanchStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State, RanchedStates.IRanchStatesCallbacks
	{
		public void OnRancherStateChanged(RanchedStates.Instance smi)
		{
			if (RanchedStates.IsCrittersTurn(smi))
			{
				smi.GoTo(smi.sm.ranch.Cheer);
				return;
			}
			smi.GoTo(smi.sm.ranch.Move.WaitInLine);
		}

		public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
		{
		}

		public RanchedStates.CheerStates Cheer;

		public RanchedStates.MoveStates Move;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Ranching;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Wavegoodbye;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Runaway;
	}

	public class CheerStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
	{
		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Cheer;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Pst;
	}

	public class MoveStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
	{
		public RanchedStates.MoveState MoveToRanch;

		public RanchedStates.WaitState WaitInLine;
	}

	public interface IRanchStatesCallbacks
	{
		void OnRancherStateChanged(RanchedStates.Instance smi);

		void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim);
	}

	public class MoveState : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State, RanchedStates.IRanchStatesCallbacks
	{
		public void OnRancherStateChanged(RanchedStates.Instance smi)
		{
			smi.GoTo(this.sm.ranch.Move.WaitInLine);
		}

		public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
		{
		}
	}

	public class WaitState : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State, RanchedStates.IRanchStatesCallbacks
	{
		public void OnRancherStateChanged(RanchedStates.Instance smi)
		{
			smi.ExitQueue();
		}

		public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
		{
			if (completedAnim == smi.def.EndWaitingAnim)
			{
				smi.ExitQueue();
			}
		}
	}
}
