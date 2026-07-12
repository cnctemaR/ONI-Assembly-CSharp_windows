using System;
using STRINGS;
using UnityEngine;

public class BuzzStates : GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.root.Exit("StopNavigator", delegate(BuzzStates.Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false, true);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ToggleTag(GameTags.Idle);
		this.idle.Enter(new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(this.PlayIdle)).ToggleScheduleCallback("DoBuzz", (BuzzStates.Instance smi) => (float)global::UnityEngine.Random.Range(3, 10), delegate(BuzzStates.Instance smi)
		{
			this.numMoves.Set(global::UnityEngine.Random.Range(4, 6), smi, false);
			smi.GoTo(this.buzz.move);
		});
		this.buzz.ParamTransition<int>(this.numMoves, this.idle, (BuzzStates.Instance smi, int p) => p <= 0);
		this.buzz.move.Enter(new StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State.Callback(this.MoveToNewCell)).EventTransition(GameHashes.DestinationReached, this.buzz.pause, null).EventTransition(GameHashes.NavigationFailed, this.buzz.pause, null);
		this.buzz.pause.Enter(delegate(BuzzStates.Instance smi)
		{
			this.numMoves.Set(this.numMoves.Get(smi) - 1, smi, false);
			smi.GoTo(this.buzz.move);
		});
	}

	public void MoveToNewCell(BuzzStates.Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		BuzzStates.MoveCellQuery moveCellQuery = new BuzzStates.MoveCellQuery(component.CurrentNavType);
		moveCellQuery.allowLiquid = smi.gameObject.HasTag(GameTags.Amphibious);
		component.RunQuery(moveCellQuery);
		component.GoTo(moveCellQuery.GetResultCell(), null);
	}

	public void PlayIdle(BuzzStates.Instance smi)
	{
		KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
		Navigator component2 = smi.GetComponent<Navigator>();
		NavType navType = component2.CurrentNavType;
		if (smi.GetComponent<Facing>().GetFacing())
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
					component.Play(invalid, KAnim.PlayMode.Once, 1f, 0f);
				}
				component.Queue(hashedString, KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
		}
		HashedString idleAnim = component2.NavGrid.GetIdleAnim(navType);
		component.Play(idleAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private StateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.IntParameter numMoves;

	private BuzzStates.BuzzingStates buzz;

	public GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State idle;

	public GameStateMachine<BuzzStates, BuzzStates.Instance, IStateMachineTarget, BuzzStates.Def>.State move;

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
		}
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
	}
}
