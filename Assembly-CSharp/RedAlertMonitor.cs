using System;

public class RedAlertMonitor : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = true;
		this.off.EventTransition(GameHashes.EnteredRedAlert, (RedAlertMonitor.Instance smi) => Game.Instance, this.on, (RedAlertMonitor.Instance smi) => RedAlertManager.Instance.Get().IsOn());
		this.on.EventTransition(GameHashes.ExitedRedAlert, (RedAlertMonitor.Instance smi) => Game.Instance, this.off, (RedAlertMonitor.Instance smi) => !RedAlertManager.Instance.Get().IsOn()).Enter("EnableRedAlert", delegate(RedAlertMonitor.Instance smi)
		{
			smi.EnableRedAlert();
		}).ToggleEffect("RedAlert")
			.ToggleExpression(Db.Get().Expressions.RedAlert, null);
	}

	public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget>.State off;

	public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget>.State on;

	public new class Instance : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void EnableRedAlert()
		{
			base.GetComponent<ChoreDriver>().StopChore();
		}
	}
}
