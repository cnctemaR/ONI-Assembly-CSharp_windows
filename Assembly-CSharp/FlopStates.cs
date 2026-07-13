using System;
using STRINGS;
using UnityEngine;

public class FlopStates : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.flop_pre;
		GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.FLOPPING.NAME;
		string text2 = CREATURES.STATUSITEMS.FLOPPING.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Exclamation;
		NotificationType notificationType = NotificationType.Bad;
		bool flag = false;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main);
		this.flop_pre.Enter(new StateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.State.Callback(FlopStates.ChooseDirection)).Transition(this.flop_cycle, new StateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.Transition.ConditionCallback(FlopStates.ShouldFlop), UpdateRate.SIM_200ms).Transition(this.pst, GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.Not(new StateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.Transition.ConditionCallback(FlopStates.ShouldFlop)), UpdateRate.SIM_200ms);
		this.flop_cycle.PlayAnim("flop_loop", KAnim.PlayMode.Once).Transition(this.pst, new StateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.Transition.ConditionCallback(FlopStates.IsSubstantialLiquid), UpdateRate.SIM_200ms).Update("Flop", new Action<FlopStates.Instance, float>(FlopStates.FlopForward), UpdateRate.SIM_33ms, false)
			.OnAnimQueueComplete(this.flop_pre);
		this.pst.QueueAnim("flop_loop", true, null).BehaviourComplete(GameTags.Creatures.Flopping, false);
	}

	public static bool ShouldFlop(FlopStates.Instance smi)
	{
		int num = Grid.CellBelow(Grid.PosToCell(smi.transform.GetPosition()));
		return Grid.IsValidCell(num) && Grid.Solid[num];
	}

	public static void ChooseDirection(FlopStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.transform.GetPosition());
		if (FlopStates.SearchForLiquid(num, 1))
		{
			smi.currentDir = 1f;
			return;
		}
		if (FlopStates.SearchForLiquid(num, -1))
		{
			smi.currentDir = -1f;
			return;
		}
		if (global::UnityEngine.Random.value > 0.5f)
		{
			smi.currentDir = 1f;
			return;
		}
		smi.currentDir = -1f;
	}

	private static bool SearchForLiquid(int cell, int delta_x)
	{
		while (Grid.IsValidCell(cell))
		{
			if (Grid.IsSubstantialLiquid(cell, 0.35f))
			{
				return true;
			}
			if (Grid.Solid[cell])
			{
				return false;
			}
			if (Grid.CritterImpassable[cell])
			{
				return false;
			}
			int num = Grid.CellBelow(cell);
			if (Grid.IsValidCell(num) && Grid.Solid[num])
			{
				cell += delta_x;
			}
			else
			{
				cell = num;
			}
		}
		return false;
	}

	public static void FlopForward(FlopStates.Instance smi, float dt)
	{
		if (smi.HasTag(GameTags.Creatures.StunnedForCapture))
		{
			return;
		}
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		int currentFrame = component.currentFrame;
		if (component.IsVisible() && (currentFrame < 23 || currentFrame > 36))
		{
			return;
		}
		Vector3 position = smi.transform.GetPosition();
		Vector3 vector = position;
		vector.x = position.x + smi.currentDir * dt * 1f;
		int num = Grid.PosToCell(vector);
		if (FlopStates.CanFlopForward(smi, num))
		{
			smi.transform.SetPosition(vector);
			return;
		}
		smi.currentDir = -smi.currentDir;
	}

	private static bool CanFlopForward(FlopStates.Instance smi, int new_cell)
	{
		if (!Grid.IsValidCell(new_cell) || Grid.Solid[new_cell] || Grid.CritterImpassable[new_cell])
		{
			return false;
		}
		CellOffset[] occupiedCellsOffsets = smi.GetComponent<OccupyArea>().OccupiedCellsOffsets;
		for (int i = 0; i < occupiedCellsOffsets.Length; i++)
		{
			int num = Grid.OffsetCell(new_cell, occupiedCellsOffsets[i]);
			if (!Grid.IsValidCell(num) || Grid.Solid[num] || Grid.CritterImpassable[num])
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsSubstantialLiquid(FlopStates.Instance smi)
	{
		return Grid.IsSubstantialLiquid(Grid.PosToCell(smi.transform.GetPosition()), 0.35f);
	}

	private GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.State flop_pre;

	private GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.State flop_cycle;

	private GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>.GameInstance
	{
		public Instance(Chore<FlopStates.Instance> chore, FlopStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Flopping);
		}

		public float currentDir = 1f;
	}
}
