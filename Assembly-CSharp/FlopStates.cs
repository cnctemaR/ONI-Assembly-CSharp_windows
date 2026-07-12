using System;
using STRINGS;
using UnityEngine;

public class FlopStates : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.flop_pre;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.FLOPPING.NAME, CREATURES.STATUSITEMS.FLOPPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
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
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.CritterImpassable[num])
		{
			smi.transform.SetPosition(vector);
			return;
		}
		smi.currentDir = -smi.currentDir;
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
