using System;

public class TaskAvailabilityMonitor : GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.EventTransition(GameHashes.NewDay, (TaskAvailabilityMonitor.Instance smi) => GameClock.Instance, this.unavailable, (TaskAvailabilityMonitor.Instance smi) => GameClock.Instance.GetCycle() > 0);
		this.unavailable.Enter("RefreshStatusItem", delegate(TaskAvailabilityMonitor.Instance smi)
		{
			smi.RefreshStatusItem();
		}).EventHandler(GameHashes.ScheduleChanged, delegate(TaskAvailabilityMonitor.Instance smi)
		{
			smi.RefreshStatusItem();
		});
	}

	public GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State unavailable;

	public new class Instance : GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void RefreshStatusItem()
		{
			KSelectable component = base.GetComponent<KSelectable>();
			WorldContainer myWorld = base.gameObject.GetMyWorld();
			if (myWorld != null && myWorld.IsModuleInterior && myWorld.ParentWorldId == myWorld.id)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.IdleInRockets, null);
				return;
			}
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Idle, null);
		}
	}
}
