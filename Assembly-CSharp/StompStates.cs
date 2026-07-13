using System;
using STRINGS;
using UnityEngine;

public class StompStates : GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Never;
		default_state = this.approach;
		this.root.Enter(new StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State.Callback(StompStates.RefreshTarget));
		this.approach.InitializeStates(this.stomper, this.target, (StompStates.Instance smi) => smi.TargetOffsets, this.stomp, this.failed, null).ToggleMainStatusItem(new Func<StompStates.Instance, StatusItem>(StompStates.GetGoingToStompStatusItem), null).OnTargetLost(this.target, this.failed)
			.Target(this.target)
			.EventTransition(GameHashes.Harvest, this.failed, null)
			.EventTransition(GameHashes.Uprooted, this.failed, null)
			.EventTransition(GameHashes.QueueDestroyObject, this.failed, null);
		this.stomp.DefaultState(this.stomp.pre).ToggleMainStatusItem(new Func<StompStates.Instance, StatusItem>(StompStates.GetStompingStatusItem), null);
		this.stomp.pre.Enter(new StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State.Callback(StompStates.ResetStompLoopTimer)).PlayAnim("stomping_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.stomp.loop);
		this.stomp.loop.ParamTransition<float>(this.stompingLoopTimer, this.stomp.pst, GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.IsLTZero).PlayAnim("stomping_loop", KAnim.PlayMode.Loop).Update(new Action<StompStates.Instance, float>(StompStates.StompUpdate), UpdateRate.SIM_200ms, false);
		this.stomp.pst.PlayAnim("stomping_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete);
		this.complete.BehaviourComplete(GameTags.Creatures.WantsToStomp, false);
		this.failed.Enter(new StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State.Callback(StompStates.ReportFailure)).EnterGoTo(null);
	}

	private static StatusItem GetGoingToStompStatusItem(StompStates.Instance smi)
	{
		return StompStates.GetStatusItem(smi, CREATURES.STATUSITEMS.GOING_TO_STOMP.NAME, CREATURES.STATUSITEMS.GOING_TO_STOMP.TOOLTIP);
	}

	private static StatusItem GetStompingStatusItem(StompStates.Instance smi)
	{
		return StompStates.GetStatusItem(smi, CREATURES.STATUSITEMS.STOMPING.NAME, CREATURES.STATUSITEMS.STOMPING.TOOLTIP);
	}

	private static StatusItem GetStatusItem(StompStates.Instance smi, string name, string tooltip)
	{
		return new StatusItem(smi.GetCurrentState().longName, name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, true, null);
	}

	private static void ResetStompLoopTimer(StompStates.Instance smi)
	{
		smi.sm.stompingLoopTimer.Set(0f, smi, false);
	}

	private static void StompUpdate(StompStates.Instance smi, float dt)
	{
		if (smi.StompLoopTimer <= 1.8333334f)
		{
			smi.sm.stompingLoopTimer.Set(smi.StompLoopTimer + dt, smi, false);
			return;
		}
		if (smi.HarvestAnyOneIntersectingPlant())
		{
			StompStates.ResetStompLoopTimer(smi);
			return;
		}
		smi.sm.stompingLoopTimer.Set(-1f, smi, false);
	}

	private static void RefreshTarget(StompStates.Instance smi)
	{
		StompMonitor.Instance smi2 = smi.GetSMI<StompMonitor.Instance>();
		smi.SetTarget(smi2.Target);
	}

	private static void ReportFailure(StompStates.Instance smi)
	{
		StompMonitor.Instance smi2 = smi.GetSMI<StompMonitor.Instance>();
		if (smi2 != null)
		{
			smi2.sm.StompStateFailed.Trigger(smi2);
		}
	}

	public const string PRE_STOMP_ANIM_NAME = "stomping_pre";

	public const string LOOP_STOMP_ANIM_NAME = "stomping_loop";

	public const string PST_STOMP_ANIM_NAME = "stomping_pst";

	private const int STOMP_LOOP_ANIM_FRAME_COUNT = 55;

	private const float STOMP_LOOP_ANIM_DURATION = 1.8333334f;

	public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.ApproachSubState<IApproachable> approach;

	public StompStates.StompState stomp;

	public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State complete;

	public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State failed;

	public StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.FloatParameter stompingLoopTimer;

	public StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.TargetParameter stomper;

	public StateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.TargetParameter target;

	public class Def : StateMachine.BaseDef
	{
	}

	public class StompState : GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State
	{
		public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State pre;

		public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State loop;

		public GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.State pst;
	}

	public new class Instance : GameStateMachine<StompStates, StompStates.Instance, IStateMachineTarget, StompStates.Def>.GameInstance
	{
		public float StompLoopTimer
		{
			get
			{
				return base.sm.stompingLoopTimer.Get(this);
			}
		}

		public GameObject CurrentTarget
		{
			get
			{
				return base.sm.target.Get(this);
			}
		}

		public Instance(Chore<StompStates.Instance> chore, StompStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToStomp);
			this.occupyArea = base.GetComponent<OccupyArea>();
			base.sm.stomper.Set(base.smi.gameObject, base.smi, false);
		}

		public void SetTarget(GameObject target)
		{
			base.smi.sm.target.Set(target, base.smi, false);
			if (this.CurrentTarget == null)
			{
				this.TargetOffsets = new CellOffset[]
				{
					new CellOffset(0, 0)
				};
				return;
			}
			ListPool<CellOffset, StompStates.Instance>.PooledList pooledList = ListPool<CellOffset, StompStates.Instance>.Allocate();
			StompMonitor.Def.GetObjectCellsOffsetsWithExtraBottomPadding(this.CurrentTarget, pooledList);
			this.TargetOffsets = pooledList.ToArray();
			pooledList.Recycle();
		}

		public bool HarvestAnyOneIntersectingPlant()
		{
			int num = Grid.PosToCell(base.gameObject);
			bool flag = false;
			for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, this.occupyArea.OccupiedCellsOffsets[i]);
				if (Grid.IsValidCell(num2))
				{
					GameObject gameObject = Grid.Objects[num2, 5];
					gameObject = ((gameObject != null) ? gameObject : Grid.Objects[num2, 1]);
					if (!(gameObject == null))
					{
						Harvestable component = gameObject.GetComponent<Harvestable>();
						if (!(component == null) && component.CanBeHarvested)
						{
							component.Trigger(2127324410, true);
							component.Harvest();
							flag = true;
							break;
						}
					}
				}
			}
			return flag;
		}

		public CellOffset[] TargetOffsets;

		private OccupyArea occupyArea;
	}
}
