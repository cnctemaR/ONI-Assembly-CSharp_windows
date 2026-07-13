using System;
using STRINGS;
using UnityEngine;

public class IdleStates : GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.root.Exit("StopNavigator", new StateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State.Callback(IdleStates.StopNavigator)).ToggleMainStatusItem(IdleStates.IdleStatus, null).ToggleTag(GameTags.Idle);
		this.loop.Enter(new StateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State.Callback(IdleStates.PlayIdle)).ToggleScheduleCallback("IdleMove", new Func<IdleStates.Instance, float>(IdleStates.GetIdleTime), new Action<IdleStates.Instance>(IdleStates.GoMove));
		this.move.Enter(new StateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State.Callback(IdleStates.MoveToNewCell)).EventTransition(GameHashes.DestinationReached, this.loop, null).EventTransition(GameHashes.NavigationFailed, this.loop, null);
	}

	private static float GetIdleTime(IdleStates.Instance smi)
	{
		return (float)global::UnityEngine.Random.Range(3, 10);
	}

	private static void GoMove(IdleStates.Instance smi)
	{
		smi.GoTo(smi.sm.move);
	}

	private static void StopNavigator(IdleStates.Instance smi)
	{
		smi.navigator.Stop(false, true);
	}

	private static void MoveToNewCell(IdleStates.Instance smi)
	{
		if (smi.kpid.HasTag(GameTags.StationaryIdling))
		{
			smi.GoTo(smi.sm.loop);
			return;
		}
		IdleStates.MoveCellQuery instance = IdleStates.MoveCellQuery.Instance;
		instance.Reset(smi.navigator.CurrentNavType);
		instance.allowLiquid = smi.kpid.HasTag(GameTags.Amphibious);
		instance.submerged = smi.kpid.HasTag(GameTags.Creatures.Submerged);
		int num = Grid.PosToCell(smi.navigator);
		if (smi.navigator.CurrentNavType == NavType.Hover && CellSelectionObject.IsExposedToSpace(num))
		{
			int num2 = 0;
			int num3 = num;
			for (int i = 0; i < 10; i++)
			{
				num3 = Grid.CellBelow(num3);
				if (!Grid.IsValidCell(num3) || Grid.IsSolidCell(num3) || !CellSelectionObject.IsExposedToSpace(num3))
				{
					break;
				}
				num2++;
			}
			instance.lowerCellBias = num2 == 10;
		}
		smi.navigator.RunQuery(instance);
		if (smi.navigator.CanReach(instance.GetResultCell()))
		{
			smi.navigator.GoTo(instance.GetResultCell(), null);
			return;
		}
		smi.GoTo(smi.sm.loop);
	}

	private static void PlayIdle(IdleStates.Instance smi)
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

	private GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State loop;

	private GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State move;

	public static StatusItem IdleStatus = new StatusItem("IdleStatus", CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Messages, false, OverlayModes.None.ID, 129022, true, null);

	public class Def : StateMachine.BaseDef
	{
		public IdleStates.Def.IdleAnimCallback customIdleAnim;

		public PriorityScreen.PriorityClass priorityClass;

		public delegate HashedString IdleAnimCallback(IdleStates.Instance smi, ref HashedString pre_anim);
	}

	public new class Instance : GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.GameInstance
	{
		public Instance(Chore<IdleStates.Instance> chore, IdleStates.Def def)
			: base(chore, def)
		{
			this.navigator = base.GetComponent<Navigator>();
			this.kpid = base.GetComponent<KPrefabID>();
			this.kac = base.GetComponent<KBatchedAnimController>();
			this.facing = base.GetComponent<Facing>();
			chore.masterPriority.priority_class = def.priorityClass;
		}

		public Navigator navigator;

		public KPrefabID kpid;

		public KBatchedAnimController kac;

		public Facing facing;
	}

	public class MoveCellQuery : PathFinderQuery
	{
		public bool allowLiquid { get; set; }

		public bool submerged { get; set; }

		public bool lowerCellBias { get; set; }

		public MoveCellQuery(NavType navType)
		{
			this.Reset(navType);
		}

		public void Reset(NavType navType)
		{
			this.navType = navType;
			this.maxIterations = global::UnityEngine.Random.Range(5, 25);
			this.targetCell = Grid.InvalidCell;
			this.allowLiquid = false;
			this.submerged = false;
			this.lowerCellBias = false;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (Grid.ObjectLayers[9].ContainsKey(cell))
			{
				return false;
			}
			bool flag = this.submerged || Grid.IsNavigatableLiquid(cell);
			bool flag2 = this.navType != NavType.Swim;
			bool flag3 = this.navType == NavType.Swim || this.allowLiquid;
			if (flag && !flag3)
			{
				return false;
			}
			if (!flag && !flag2)
			{
				return false;
			}
			if (this.targetCell == Grid.InvalidCell || !this.lowerCellBias)
			{
				this.targetCell = cell;
			}
			else
			{
				int num = Grid.CellRow(this.targetCell);
				if (Grid.CellRow(cell) < num)
				{
					this.targetCell = cell;
				}
			}
			int num2 = this.maxIterations - 1;
			this.maxIterations = num2;
			return num2 <= 0;
		}

		public override int GetResultCell()
		{
			return this.targetCell;
		}

		private NavType navType;

		private int targetCell = Grid.InvalidCell;

		private int maxIterations;

		public static IdleStates.MoveCellQuery Instance = new IdleStates.MoveCellQuery(NavType.Floor);
	}
}
