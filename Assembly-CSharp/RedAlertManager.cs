using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RedAlertManager : GameStateMachine<RedAlertManager, RedAlertManager.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = true;
		this.off.ParamTransition<bool>(this.isOn, this.on, GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.IsTrue);
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
			.Enter("Sounds", delegate(RedAlertManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON", false));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("RedAlert_LP", false), null)
			.ToggleNotification((RedAlertManager.Instance smi) => smi.notification)
			.ParamTransition<bool>(this.isOn, this.off, GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.IsFalse);
		this.on_pst.Enter("Sounds", delegate(RedAlertManager.Instance smi)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF", false));
		});
	}

	public GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.State on;

	public GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.State on_pst;

	public StateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.BoolParameter isOn = new StateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.BoolParameter();

	public new class Instance : GameStateMachine<RedAlertManager, RedAlertManager.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			RedAlertManager.Instance.instance = this;
		}

		public static void DestroyInstance()
		{
			RedAlertManager.Instance.instance = null;
		}

		public static RedAlertManager.Instance Get()
		{
			return RedAlertManager.Instance.instance;
		}

		public bool IsOn()
		{
			return base.sm.isOn.Get(base.smi);
		}

		public bool IsToggledOn()
		{
			return this.isToggled;
		}

		public void Toggle(bool on)
		{
			this.isToggled = on;
			this.Refresh();
		}

		public void HasEmergencyChore(bool on)
		{
			this.hasEmergencyChore = on;
			this.Refresh();
		}

		private void Refresh()
		{
			base.sm.isOn.Set(this.isToggled || this.hasEmergencyChore, base.smi);
		}

		private static RedAlertManager.Instance instance;

		private bool isToggled;

		private bool hasEmergencyChore;

		public Notification notification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, false, 0f, null, null, null);
	}
}
