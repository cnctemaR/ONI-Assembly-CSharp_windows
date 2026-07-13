using System;
using STRINGS;

public class RanchedStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.ranch;
		this.root.Exit("AbandonedRanchStation", delegate(RanchedStates.Instance smi)
		{
			if (smi.Monitor.TargetRanchStation != null)
			{
				if (smi.Monitor.TargetRanchStation.IsCritterInQueue(smi.Monitor))
				{
					Debug.LogWarning("Why are we exiting RanchedStates while in the queue?");
					smi.Monitor.TargetRanchStation.Abandon(smi.Monitor);
				}
				smi.Monitor.TargetRanchStation = null;
			}
			smi.sm.ranchTarget.Set(null, smi);
		});
		this.ranch.EnterTransition(this.ranch.Cheer, (RanchedStates.Instance smi) => RanchedStates.IsCrittersTurn(smi)).EventHandler(GameHashes.RanchStationNoLongerAvailable, delegate(RanchedStates.Instance smi)
		{
			smi.GoTo(null);
		}).BehaviourComplete(GameTags.Creatures.WantsToGetRanched, true)
			.Update(delegate(RanchedStates.Instance smi, float deltaSeconds)
			{
				RanchStation.Instance ranchStation = smi.GetRanchStation();
				if (ranchStation.IsNullOrDestroyed())
				{
					smi.StopSM("No more target ranch station.");
					return;
				}
				Option<CavityInfo> option = Option.Maybe<CavityInfo>(Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi)));
				Option<CavityInfo> cavityInfo = ranchStation.GetCavityInfo();
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
			.EventHandler(GameHashes.RancherReadyAtRanchStation, delegate(RanchedStates.Instance smi)
			{
				smi.UpdateWaitingState();
			})
			.Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride));
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State cheer = this.ranch.Cheer;
		string text = CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME;
		string text2 = CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		cheer.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, statusItemCategory).Enter("FaceRancher", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetRanchStation().transform.GetPosition());
		}).PlayAnim("excited_loop")
			.OnAnimQueueComplete(this.ranch.Cheer.Pst)
			.ScheduleGoTo((RanchedStates.Instance smi) => smi.cheerAnimLength, this.ranch.Move);
		this.ranch.Cheer.Pst.ScheduleGoTo(0.2f, this.ranch.Move);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state = this.ranch.Move.DefaultState(this.ranch.Move.MoveToRanch).Enter("Speedup", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed * 1.25f;
		});
		string text4 = CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME;
		string text5 = CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP;
		string text6 = "";
		StatusItem.IconType iconType2 = StatusItem.IconType.Info;
		NotificationType notificationType2 = NotificationType.Neutral;
		bool flag2 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text4, text5, text6, iconType2, notificationType2, flag2, default(HashedString), 129022, null, null, statusItemCategory).Exit("RestoreSpeed", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed;
		});
		this.ranch.Move.MoveToRanch.EnterTransition(this.ranch.Wait.WaitInLine, GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Not(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn))).MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRanchNavTarget), this.ranch.Wait.WaitInLine, null, false).Target(this.ranchTarget)
			.EventTransition(GameHashes.CreatureArrivedAtRanchStation, this.ranch.Wait.WaitInLine, (RanchedStates.Instance smi) => !RanchedStates.IsCrittersTurn(smi));
		this.ranch.Wait.WaitInLine.EnterTransition(this.ranch.Ranching, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn)).Enter(delegate(RanchedStates.Instance smi)
		{
			smi.EnterQueue();
		}).EventTransition(GameHashes.DestinationReached, this.ranch.Wait.Waiting, null);
		this.ranch.Wait.Waiting.Face(this.ranchTarget, 0f).PlayAnim((RanchedStates.Instance smi) => smi.def.StartWaitingAnim, KAnim.PlayMode.Once).QueueAnim((RanchedStates.Instance smi) => smi.def.WaitingAnim, true, null);
		this.ranch.Wait.DoneWaiting.PlayAnim((RanchedStates.Instance smi) => smi.def.EndWaitingAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.ranch.Move.MoveToRanch);
		this.ranch.Ranching.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.GetOnTable)).Enter("SetCreatureAtRanchingStation", delegate(RanchedStates.Instance smi)
		{
			smi.GetRanchStation().MessageCreatureArrived(smi);
			smi.AnimController.SetSceneLayer(Grid.SceneLayer.BuildingUse);
		}).EventTransition(GameHashes.RanchingComplete, this.ranch.Wavegoodbye, null)
			.ToggleMainStatusItem(delegate(RanchedStates.Instance smi)
			{
				RanchStation.Instance ranchStation2 = RanchedStates.GetRanchStation(smi);
				if (ranchStation2 != null)
				{
					return ranchStation2.def.CreatureRanchingStatusItem;
				}
				return Db.Get().CreatureStatusItems.GettingRanched;
			}, null);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state2 = this.ranch.Wavegoodbye.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride)).OnAnimQueueComplete(this.ranch.Runaway);
		string text7 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		string text8 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		string text9 = "";
		StatusItem.IconType iconType3 = StatusItem.IconType.Info;
		NotificationType notificationType3 = NotificationType.Neutral;
		bool flag3 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text7, text8, text9, iconType3, notificationType3, flag3, default(HashedString), 129022, null, null, statusItemCategory);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state3 = this.ranch.Runaway.MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRunawayCell), null, null, false);
		string text10 = CREATURES.STATUSITEMS.IDLE.NAME;
		string text11 = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		string text12 = "";
		StatusItem.IconType iconType4 = StatusItem.IconType.Info;
		NotificationType notificationType4 = NotificationType.Neutral;
		bool flag4 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state3.ToggleStatusItem(text10, text11, text12, iconType4, notificationType4, flag4, default(HashedString), 129022, null, null, statusItemCategory);
	}

	private static void ClearLayerOverride(RanchedStates.Instance smi)
	{
		smi.AnimController.SetSceneLayer(Grid.SceneLayer.Creatures);
	}

	private static RanchStation.Instance GetRanchStation(RanchedStates.Instance smi)
	{
		return smi.GetRanchStation();
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
		int num = smi.ModifyNavTargetForCritter(ranchStation.GetRanchNavTarget());
		if (smi.HasTag(GameTags.LargeCreature))
		{
			ref Vector2I ptr = Grid.PosToXY(smi.gameObject.transform.position);
			Vector2I vector2I = Grid.CellToXY(num);
			if (ptr.x > vector2I.x)
			{
				num = Grid.CellLeft(num);
			}
		}
		return num;
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

	private StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.TargetParameter ranchTarget;

	public class Def : StateMachine.BaseDef
	{
		public string StartWaitingAnim = "queue_pre";

		public string WaitingAnim = "queue_loop";

		public string EndWaitingAnim = "queue_pst";

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
			KAnim.Anim anim = this.animController.GetAnim(new HashedString("excited_loop"));
			this.cheerAnimLength = ((anim != null) ? (anim.totalTime + 0.2f) : 1.2f);
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
			if (this.GetRanchStation() != null)
			{
				this.InitializeWaitCell();
				this.Monitor.NavComponent.GoTo(this.waitCell, null);
			}
		}

		public void AbandonRanchStation()
		{
			if (this.Monitor.TargetRanchStation == null || this.status == StateMachine.Status.Failed)
			{
				return;
			}
			this.StopSM("Abandoned Ranch");
		}

		public void SetRanchStation(RanchStation.Instance ranch_station)
		{
			if (this.Monitor.TargetRanchStation != null && this.Monitor.TargetRanchStation != ranch_station)
			{
				this.Monitor.TargetRanchStation.Abandon(base.smi.Monitor);
			}
			base.smi.sm.ranchTarget.Set(ranch_station.gameObject, base.smi, false);
			this.Monitor.TargetRanchStation = ranch_station;
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
			if (this.GetRanchStation() == null)
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

		public void UpdateWaitingState()
		{
			if (!RanchedStates.IsCrittersTurn(base.smi))
			{
				base.smi.GoTo(base.smi.sm.ranch.Wait.WaitInLine);
				return;
			}
			if (base.smi.IsInsideState(base.sm.ranch.Wait.Waiting))
			{
				base.smi.GoTo(base.smi.sm.ranch.Wait.DoneWaiting);
				return;
			}
			base.smi.GoTo(base.smi.sm.ranch.Cheer);
		}

		public float OriginalSpeed;

		private int waitCell;

		private KBatchedAnimController animController;

		private RanchableMonitor.Instance ranchMonitor;

		public float cheerAnimLength;
	}

	public class RanchStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
	{
		public RanchedStates.CheerStates Cheer;

		public RanchedStates.MoveStates Move;

		public RanchedStates.WaitStates Wait;

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
		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State MoveToRanch;
	}

	public class WaitStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
	{
		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State WaitInLine;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Waiting;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State DoneWaiting;
	}
}
