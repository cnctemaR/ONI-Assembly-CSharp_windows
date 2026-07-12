using System;
using System.Collections.Generic;
using STRINGS;

public class AlertStateManager : GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.off.ParamTransition<bool>(this.isOn, this.on, GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.IsTrue);
		this.on.Exit("VignetteOff", delegate(AlertStateManager.Instance smi)
		{
			Vignette.Instance.Reset();
		}).ParamTransition<bool>(this.isRedAlert, this.on.red, (AlertStateManager.Instance smi, bool p) => this.isRedAlert.Get(smi)).ParamTransition<bool>(this.isRedAlert, this.on.yellow, (AlertStateManager.Instance smi, bool p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))
			.ParamTransition<bool>(this.isYellowAlert, this.on.yellow, (AlertStateManager.Instance smi, bool p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))
			.ParamTransition<bool>(this.isOn, this.off, GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.IsFalse);
		this.on.red.Enter("EnterEvent", delegate(AlertStateManager.Instance smi)
		{
			Game.Instance.Trigger(1585324898, null);
		}).Exit("ExitEvent", delegate(AlertStateManager.Instance smi)
		{
			Game.Instance.Trigger(-1393151672, null);
		}).Enter("SoundsOnRedAlert", delegate(AlertStateManager.Instance smi)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON", false));
		})
			.Exit("SoundsOffRedAlert", delegate(AlertStateManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF", false));
			})
			.ToggleNotification((AlertStateManager.Instance smi) => smi.redAlertNotification);
		this.on.yellow.Enter("EnterEvent", delegate(AlertStateManager.Instance smi)
		{
			Game.Instance.Trigger(-741654735, null);
		}).Exit("ExitEvent", delegate(AlertStateManager.Instance smi)
		{
			Game.Instance.Trigger(-2062778933, null);
		}).Enter("SoundsOnYellowAlert", delegate(AlertStateManager.Instance smi)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_ON", false));
		})
			.Exit("SoundsOffRedAlert", delegate(AlertStateManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_OFF", false));
			});
	}

	public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State off;

	public AlertStateManager.OnStates on;

	public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isRedAlert = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();

	public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isYellowAlert = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();

	public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isOn = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();

	public class Def : StateMachine.BaseDef
	{
	}

	public class OnStates : GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State
	{
		public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State yellow;

		public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State red;
	}

	public new class Instance : GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AlertStateManager.Def def)
			: base(master, def)
		{
		}

		public void UpdateState(float dt)
		{
			if (this.IsRedAlert())
			{
				base.smi.GoTo(base.sm.on.red);
				return;
			}
			if (this.IsYellowAlert())
			{
				base.smi.GoTo(base.sm.on.yellow);
				return;
			}
			if (!this.IsOn())
			{
				base.smi.GoTo(base.sm.off);
			}
		}

		public bool IsOn()
		{
			return base.sm.isYellowAlert.Get(base.smi) || base.sm.isRedAlert.Get(base.smi);
		}

		public bool IsRedAlert()
		{
			return base.sm.isRedAlert.Get(base.smi);
		}

		public bool IsYellowAlert()
		{
			return base.sm.isYellowAlert.Get(base.smi);
		}

		public bool IsRedAlertToggledOn()
		{
			return this.isToggled;
		}

		public void ToggleRedAlert(bool on)
		{
			this.isToggled = on;
			this.Refresh();
		}

		public void SetHasTopPriorityChore(bool on)
		{
			this.hasTopPriorityChore = on;
			this.Refresh();
		}

		private void Refresh()
		{
			base.sm.isYellowAlert.Set(this.hasTopPriorityChore, base.smi, false);
			base.sm.isRedAlert.Set(this.isToggled, base.smi, false);
			base.sm.isOn.Set(this.hasTopPriorityChore || this.isToggled, base.smi, false);
		}

		private bool isToggled;

		private bool hasTopPriorityChore;

		public Notification redAlertNotification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, false, 0f, null, null, null, true, false, false);
	}
}
