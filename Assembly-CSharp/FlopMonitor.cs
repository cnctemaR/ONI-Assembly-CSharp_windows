using System;
using UnityEngine;

public class FlopMonitor : GameStateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.Flopping, (FlopMonitor.Instance smi) => smi.ShouldBeginFlopping(), null);
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FlopMonitor.Def def)
			: base(master, def)
		{
		}

		public bool ShouldBeginFlopping()
		{
			Vector3 position = base.transform.GetPosition();
			position.y += CreatureFallMonitor.FLOOR_DISTANCE;
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Grid.Solid[Grid.PosToCell(position)] && !Grid.IsSubstantialLiquid(num, 0.35f) && !Grid.IsLiquid(Grid.CellAbove(num));
		}
	}
}
