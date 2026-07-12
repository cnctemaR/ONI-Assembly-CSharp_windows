using System;

public class RedAlertMonitor : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.off.EventTransition(GameHashes.EnteredRedAlert, (RedAlertMonitor.Instance smi) => Game.Instance, this.on, delegate(RedAlertMonitor.Instance smi)
		{
			WorldContainer myWorld = smi.master.gameObject.GetMyWorld();
			return !(myWorld == null) && myWorld.AlertManager.IsRedAlert();
		});
		this.on.EventTransition(GameHashes.ExitedRedAlert, (RedAlertMonitor.Instance smi) => Game.Instance, this.off, delegate(RedAlertMonitor.Instance smi)
		{
			WorldContainer myWorld2 = smi.master.gameObject.GetMyWorld();
			return !(myWorld2 == null) && !myWorld2.AlertManager.IsRedAlert();
		}).Enter("EnableRedAlert", delegate(RedAlertMonitor.Instance smi)
		{
			smi.EnableRedAlert();
		}).ToggleEffect("RedAlert")
			.ToggleExpression(Db.Get().Expressions.RedAlert, null);
	}

	public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.State on;

	public new class Instance : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void EnableRedAlert()
		{
			ChoreDriver component = base.GetComponent<ChoreDriver>();
			if (component != null)
			{
				Chore currentChore = component.GetCurrentChore();
				if (currentChore != null)
				{
					bool flag = false;
					for (int i = 0; i < currentChore.GetPreconditions().Count; i++)
					{
						if (currentChore.GetPreconditions()[i].id == ChorePreconditions.instance.IsNotRedAlert.id)
						{
							flag = true;
						}
					}
					if (flag)
					{
						component.StopChore();
					}
				}
			}
		}
	}
}
