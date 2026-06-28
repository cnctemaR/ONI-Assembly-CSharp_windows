using System;
using STRINGS;
using UnityEngine;

public class RedAlertManager : GameStateMachine<RedAlertManager, RedAlertManager.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = true;
		this.off.ParamTransition<bool>(this.isOn, this.on, (RedAlertManager.Instance smi, bool p) => p);
		this.on.Enter("EnterEvent", delegate(RedAlertManager.Instance smi)
		{
			Game.Instance.Trigger(1585324898, null);
		}).Exit("ExitEvent", delegate(RedAlertManager.Instance smi)
		{
			Game.Instance.Trigger(-1393151672, null);
		}).Enter("EnableVignette", delegate(RedAlertManager.Instance smi)
		{
			Vignette.Instance.SetColor(new Color(1f, 0f, 0f, 0.3f));
		})
			.Exit("DisableVignette", delegate(RedAlertManager.Instance smi)
			{
				Vignette.Instance.Reset();
			})
			.ToggleNotification((RedAlertManager.Instance smi) => smi.notification)
			.ParamTransition<bool>(this.isOn, this.off, (RedAlertManager.Instance smi, bool p) => !p);
	}

	public GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget>.State off;

	public GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget>.State on;

	public StateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget>.BoolParameter isOn = new StateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget>.BoolParameter();

	public new class Instance : GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			RedAlertManager.Instance.instance = this;
		}

		public static RedAlertManager.Instance Get()
		{
			return RedAlertManager.Instance.instance;
		}

		public bool IsOn()
		{
			return base.sm.isOn.Get(base.smi);
		}

		public void Toggle(bool on)
		{
			base.sm.isOn.Set(on, base.smi);
		}

		private static RedAlertManager.Instance instance;

		public Notification notification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, null, null, null, false, 0f, null, null, null);
	}
}
