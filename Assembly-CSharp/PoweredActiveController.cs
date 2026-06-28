using System;

public class PoweredActiveController : GameStateMachine<PoweredActiveController, PoweredActiveController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.on, (PoweredActiveController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.off, (PoweredActiveController.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.working.pre, (PoweredActiveController.Instance smi) => smi.GetComponent<Operational>().IsActive);
		this.working.Enter(delegate(PoweredActiveController.Instance smi)
		{
			if (smi.ShowWorkingStatus)
			{
				smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Working, null);
			}
		}).Exit(delegate(PoweredActiveController.Instance smi)
		{
			if (smi.ShowWorkingStatus)
			{
				smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Working, false);
			}
		});
		this.working.pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working.loop);
		this.working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.OperationalChanged, this.working.pst, (PoweredActiveController.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.working.pst, (PoweredActiveController.Instance smi) => !smi.GetComponent<Operational>().IsActive);
		this.working.pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.on);
	}

	public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State on;

	public PoweredActiveController.WorkingStates working;

	public class WorkingStates : GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State pre;

		public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State loop;

		public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.State pst;
	}

	public new class Instance : GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool ShowWorkingStatus;
	}
}
