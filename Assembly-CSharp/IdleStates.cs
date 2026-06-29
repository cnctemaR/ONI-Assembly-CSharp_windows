using System;
using STRINGS;
using UnityEngine;

internal class IdleStates : GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State state = this.root.Exit("StopNavigator", delegate(IdleStates.Instance smi)
		{
			smi.GetComponent<Navigator>().Stop(false);
		});
		string text = CREATURES.STATUSITEMS.IDLE.NAME;
		string text2 = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main).ToggleTag(GameTags.Idle);
		this.loop.Enter(delegate(IdleStates.Instance smi)
		{
			smi.PlayIdle();
		}).ToggleScheduleCallback("IdleMove", (IdleStates.Instance smi) => (float)global::UnityEngine.Random.Range(3, 10), delegate(IdleStates.Instance smi)
		{
			smi.GoTo(this.move);
		});
		this.move.Enter("MoveToNewCell", delegate(IdleStates.Instance smi)
		{
			smi.MoveToNewCell();
		}).EventTransition(GameHashes.DestinationReached, this.loop, null).EventTransition(GameHashes.NavigationFailed, this.loop, null);
	}

	private GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State loop;

	private GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.State move;

	public class Def : StateMachine.BaseDef
	{
		public Func<IdleStates.Instance, HashedString> customIdleAnim;
	}

	public new class Instance : GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>.GameInstance
	{
		public Instance(Chore<IdleStates.Instance> chore, IdleStates.Def def)
			: base(chore, def)
		{
		}

		public void MoveToNewCell()
		{
			Navigator component = base.GetComponent<Navigator>();
			IdleStates.MoveCellQuery moveCellQuery = new IdleStates.MoveCellQuery(component.CurrentNavType);
			component.RunQuery(moveCellQuery);
			component.GoTo(moveCellQuery.GetResultCell(), null);
		}

		public void PlayIdle()
		{
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			Navigator component2 = base.GetComponent<Navigator>();
			NavType navType = component2.CurrentNavType;
			Facing component3 = base.GetComponent<Facing>();
			if (component3.GetFacing())
			{
				navType = NavGrid.MirrorNavType(navType);
			}
			HashedString hashedString = HashedString.Invalid;
			if (base.def.customIdleAnim != null)
			{
				hashedString = base.def.customIdleAnim(this);
			}
			if (hashedString == HashedString.Invalid)
			{
				hashedString = component2.NavGrid.GetIdleAnim(navType);
			}
			component.Play(hashedString, KAnim.PlayMode.Loop, 1f, 0f);
		}
	}

	public class MoveCellQuery : PathFinderQuery
	{
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
			if (Grid.IsSubstantialLiquid(cell, 0.35f) == (this.navType == NavType.Swim))
			{
				this.targetCell = cell;
				return --this.maxIterations <= 0;
			}
			return false;
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
