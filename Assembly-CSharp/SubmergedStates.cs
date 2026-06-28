using System;
using STRINGS;
using UnityEngine;

internal class SubmergedStates : GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle_pre;
		GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.IDLE.NAME;
		string text2 = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.idle_pre.Enter("PlayIdleAnim", delegate(SubmergedStates.Instance smi)
		{
			smi.PlayIdleAnim(false);
		}).OnAnimQueueComplete(this.idle);
		this.idle.Enter("PlayIdleAnim", delegate(SubmergedStates.Instance smi)
		{
			smi.PlayIdleAnim(true);
		}).Transition(this.behaviourcomplete, (SubmergedStates.Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_1000ms).Transition(this.movetosurface, (SubmergedStates.Instance smi) => smi.FindTargetCell(), UpdateRate.SIM_1000ms);
		this.movetosurface.MoveTo((SubmergedStates.Instance smi) => smi.targetCell, this.idle_pre, this.idle_pre, false);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Submerged, false);
	}

	public GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.State idle;

	public GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.State idle_pre;

	public GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.State movetosurface;

	public GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>.GameInstance
	{
		public Instance(Chore<SubmergedStates.Instance> chore, SubmergedStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Submerged);
		}

		public bool FindTargetCell()
		{
			int num = Grid.PosToCell(base.gameObject);
			this.targetCell = GameUtil.FloodFillFind(new Func<int, bool>(this.IsAboveWater), num, 8, true, false);
			if (this.targetCell == -1)
			{
				CellOffset[] array = new CellOffset[]
				{
					new CellOffset(0, 0),
					new CellOffset(-1, 0),
					new CellOffset(1, 0),
					new CellOffset(-1, -1),
					new CellOffset(1, -1)
				};
				this.targetCell = Grid.OffsetCell(num, array[global::UnityEngine.Random.Range(0, array.Length)]);
				int num2 = Grid.CellAbove(this.targetCell);
				while (Grid.IsSubstantialLiquid(num2, 0.35f))
				{
					this.targetCell = num2;
					num2 = Grid.CellAbove(this.targetCell);
				}
			}
			return this.targetCell != PathProber.InvalidCell;
		}

		public bool IsAboveWater(int cell)
		{
			return !Grid.IsSubstantialLiquid(cell, 0.35f);
		}

		public bool IsSubmerged()
		{
			return !this.IsAboveWater(Grid.PosToCell(base.transform.GetPosition()));
		}

		public void PlayIdleAnim(bool loop)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			KAnim.PlayMode playMode = KAnim.PlayMode.Once;
			if (loop)
			{
				playMode = KAnim.PlayMode.Loop;
			}
			if (this.IsSubmerged())
			{
				component.Play("swim_idle_loop", playMode, 1f, 0f);
			}
			else
			{
				component.Play("idle_loop", playMode, 1f, 0f);
			}
		}

		public int targetCell = PathProber.InvalidCell;
	}
}
