using System;
using UnityEngine;

public class CreatureFallMonitor : GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded;
		this.grounded.Enter("SnapToGround", delegate(CreatureFallMonitor.Instance smi)
		{
			smi.SnapToGround();
		}).Transition(this.falling, (CreatureFallMonitor.Instance smi) => !Grid.Solid[Grid.PosToCell(smi.transform.GetPosition())] && !Grid.Solid[Grid.CellBelow(Grid.PosToCell(smi.transform.GetPosition()))], UpdateRate.SIM_200ms);
		this.falling.EventTransition(GameHashes.Landed, this.grounded, null).ToggleTag(GameTags.Creatures.Falling);
	}

	public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State grounded;

	public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State falling;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureFallMonitor.Def def)
			: base(master, def)
		{
		}

		public void SnapToGround()
		{
			Vector3 position = base.smi.transform.GetPosition();
			Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Creatures);
			vector.z = position.z;
			base.smi.transform.SetPosition(vector);
		}

		public string anim = "fall";
	}
}
