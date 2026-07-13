using System;
using UnityEngine;

public class BuzzStates : GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.root.Exit("StopNavigator", new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(BuzzStates.StopNavigator)).ToggleMainStatusItem(IdleStates.IdleStatus, null).ToggleTag(GameTags.Idle);
		this.idle.Enter(new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(BuzzStates.PlayIdle)).ToggleScheduleCallback("DoBuzz", new Func<BuzzStates.Instance, float>(BuzzStates.GetIdleTime), new Action<BuzzStates.Instance>(BuzzStates.GoBuzz));
		this.buzz.ParamTransition<int>(this.numMoves, this.idle, GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.IsLTEZero_int);
		this.buzz.move.Enter(new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(BuzzStates.MoveToNewCell)).EventTransition(GameHashes.DestinationReached, this.buzz.pause, null).EventTransition(GameHashes.NavigationFailed, this.buzz.pause, null);
		this.buzz.pause.Enter(new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(BuzzStates.BuzzPause));
	}

	private static float GetIdleTime(BuzzStates.Instance smi)
	{
		return (float)global::UnityEngine.Random.Range(3, 10);
	}

	private static void GoBuzz(BuzzStates.Instance smi)
	{
		smi.sm.numMoves.Set(global::UnityEngine.Random.Range(4, 6), smi, false);
		smi.GoTo(smi.sm.buzz.move);
	}

	private static void BuzzPause(BuzzStates.Instance smi)
	{
		smi.sm.numMoves.Set(smi.sm.numMoves.Get(smi) - 1, smi, false);
		smi.GoTo(smi.sm.buzz.move);
	}

	private static void StopNavigator(BuzzStates.Instance smi)
	{
		smi.navigator.Stop(false, true);
	}

	private static void MoveToNewCell(BuzzStates.Instance smi)
	{
		BuzzStates.MoveCellQuery.Instance.Reset(smi.navigator.CurrentNavType, smi.kpid.HasTag(GameTags.Amphibious));
		smi.navigator.RunQuery(BuzzStates.MoveCellQuery.Instance);
		smi.navigator.GoTo(BuzzStates.MoveCellQuery.Instance.GetResultCell(), null);
	}

	private static void PlayIdle(BuzzStates.Instance smi)
	{
		NavType navType = smi.navigator.CurrentNavType;
		if (smi.facing.GetFacing())
		{
			navType = NavGrid.MirrorNavType(navType);
		}
		if (smi.def.customIdleAnim != null)
		{
			HashedString invalid = HashedString.Invalid;
			HashedString hashedString = smi.def.customIdleAnim(smi, ref invalid);
			if (hashedString != HashedString.Invalid)
			{
				if (invalid != HashedString.Invalid)
				{
					smi.kac.Play(invalid, KAnim.PlayMode.Once, 1f, 0f);
				}
				smi.kac.Queue(hashedString, KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
		}
		HashedString idleAnim = smi.navigator.NavGrid.GetIdleAnim(navType);
		smi.kac.Play(idleAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.IntParameter numMoves;

	private BuzzStates.BuzzingStates buzz;

	public GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State idle;

	public class Def : StateMachine.BaseDef
	{
		public BuzzStates.Def.IdleAnimCallback customIdleAnim;

		public delegate HashedString IdleAnimCallback(BuzzStates.Instance smi, ref HashedString pre_anim);
	}

	public new class Instance : GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.GameInstance
	{
		public Instance(Chore<BuzzStates.Instance> chore, BuzzStates.Def def)
			: base(chore, def)
		{
			this.navigator = base.GetComponent<Navigator>();
			this.kac = base.GetComponent<KBatchedAnimController>();
			this.kpid = base.GetComponent<KPrefabID>();
			this.facing = base.GetComponent<Facing>();
		}

		public Navigator navigator;

		public KBatchedAnimController kac;

		public KPrefabID kpid;

		public Facing facing;
	}

	public class BuzzingStates : GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State
	{
		public GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State move;

		public GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State pause;
	}

	public class MoveCellQuery : PathFinderQuery
	{
		public bool allowLiquid { get; set; }

		public MoveCellQuery(NavType navType)
		{
			this.navType = navType;
			this.maxIterations = global::UnityEngine.Random.Range(5, 25);
		}

		public void Reset(NavType navType, bool allowLiquid)
		{
			this.navType = navType;
			this.maxIterations = global::UnityEngine.Random.Range(5, 25);
			this.targetCell = Grid.InvalidCell;
			this.allowLiquid = allowLiquid;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			bool flag = this.navType != NavType.Swim;
			bool flag2 = this.navType == NavType.Swim || this.allowLiquid;
			bool flag3 = Grid.IsSubstantialLiquid(cell, 0.35f);
			if (flag3 && !flag2)
			{
				return false;
			}
			if (!flag3 && !flag)
			{
				return false;
			}
			this.targetCell = cell;
			int num = this.maxIterations - 1;
			this.maxIterations = num;
			return num <= 0;
		}

		public override int GetResultCell()
		{
			return this.targetCell;
		}

		private NavType navType;

		private int targetCell = Grid.InvalidCell;

		private int maxIterations;

		public static BuzzStates.MoveCellQuery Instance = new BuzzStates.MoveCellQuery(NavType.Floor);
	}
}
