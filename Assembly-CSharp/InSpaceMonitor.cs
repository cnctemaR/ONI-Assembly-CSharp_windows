using System;

public class InSpaceMonitor : GameStateMachine<InSpaceMonitor, InSpaceMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.root.Enter(delegate(InSpaceMonitor.Instance smi)
		{
			if (smi.IsInSpace())
			{
				smi.GoTo(this.inSpace);
			}
		});
		this.idle.EventTransition(GameHashes.MinionMigration, (InSpaceMonitor.Instance smi) => Game.Instance, this.inSpace, (InSpaceMonitor.Instance smi) => smi.IsInSpace());
		this.inSpace.EventTransition(GameHashes.MinionMigration, (InSpaceMonitor.Instance smi) => Game.Instance, this.idle, (InSpaceMonitor.Instance smi) => !smi.IsInSpace()).ToggleEffect("SpaceBuzz");
	}

	public GameStateMachine<InSpaceMonitor, InSpaceMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<InSpaceMonitor, InSpaceMonitor.Instance, IStateMachineTarget, object>.State inSpace;

	public new class Instance : GameStateMachine<InSpaceMonitor, InSpaceMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsInSpace()
		{
			WorldContainer myWorld = this.GetMyWorld();
			if (!myWorld)
			{
				return false;
			}
			int parentWorldId = myWorld.ParentWorldId;
			int id = myWorld.id;
			return myWorld.GetComponent<Clustercraft>() && parentWorldId == id;
		}
	}
}
