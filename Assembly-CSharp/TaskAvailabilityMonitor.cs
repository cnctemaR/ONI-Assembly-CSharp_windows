using System;

public class TaskAvailabilityMonitor : GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.EventTransition(GameHashes.NewDay, (TaskAvailabilityMonitor.Instance smi) => GameClock.Instance, this.unavailable, (TaskAvailabilityMonitor.Instance smi) => GameClock.Instance.GetDay() > 0);
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
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Idle, null);
		}
	}
}
