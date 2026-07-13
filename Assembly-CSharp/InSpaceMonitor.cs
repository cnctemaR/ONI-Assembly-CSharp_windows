using System;
using Klei.AI;

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
		this.idle.Transition(this.inSpace, (InSpaceMonitor.Instance smi) => smi.IsInSpace(), UpdateRate.SIM_1000ms).Enter(delegate(InSpaceMonitor.Instance smi)
		{
			Effects component = smi.master.gameObject.GetComponent<Effects>();
			if (component != null && component.HasEffect("SpaceBuzz"))
			{
				component.Remove("SpaceBuzz");
			}
		});
		this.inSpace.Transition(this.idle, (InSpaceMonitor.Instance smi) => !smi.IsInSpace(), UpdateRate.SIM_1000ms).ToggleEffect("SpaceBuzz");
	}

	private const string SPACE_EFFECT_NAME = "SpaceBuzz";

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
