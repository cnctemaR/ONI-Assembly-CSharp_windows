using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class VignetteManager : GameStateMachine<VignetteManager, VignetteManager.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.ParamTransition<bool>(this.isOn, this.on, GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.IsTrue);
		this.on.Exit("VignetteOff", delegate(VignetteManager.Instance smi)
		{
			Vignette.Instance.Reset();
		}).ParamTransition<bool>(this.isRedAlert, this.on.red, (VignetteManager.Instance smi, bool p) => this.isRedAlert.Get(smi)).ParamTransition<bool>(this.isRedAlert, this.on.yellow, (VignetteManager.Instance smi, bool p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))
			.ParamTransition<bool>(this.isYellowAlert, this.on.yellow, (VignetteManager.Instance smi, bool p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))
			.ParamTransition<bool>(this.isOn, this.off, GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.IsFalse);
		this.on.red.Enter("EnterEvent", delegate(VignetteManager.Instance smi)
		{
			Game.Instance.Trigger(1585324898, null);
		}).Exit("ExitEvent", delegate(VignetteManager.Instance smi)
		{
			Game.Instance.Trigger(-1393151672, null);
		}).Enter("EnableVignette", delegate(VignetteManager.Instance smi)
		{
			Vignette.Instance.SetColor(new Color(1f, 0f, 0f, 0.3f));
		})
			.Enter("SoundsOnRedAlert", delegate(VignetteManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON", false));
			})
			.Exit("SoundsOffRedAlert", delegate(VignetteManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF", false));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("RedAlert_LP", false), null, true, false, true)
			.ToggleNotification((VignetteManager.Instance smi) => smi.redAlertNotification);
		this.on.yellow.Enter("EnterEvent", delegate(VignetteManager.Instance smi)
		{
			Game.Instance.Trigger(-741654735, null);
		}).Exit("ExitEvent", delegate(VignetteManager.Instance smi)
		{
			Game.Instance.Trigger(-2062778933, null);
		}).Enter("EnableVignette", delegate(VignetteManager.Instance smi)
		{
			Vignette.Instance.SetColor(new Color(1f, 1f, 0f, 0.3f));
		})
			.Enter("SoundsOnYellowAlert", delegate(VignetteManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_ON", false));
			})
			.Exit("SoundsOffRedAlert", delegate(VignetteManager.Instance smi)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_OFF", false));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("YellowAlert_LP", false), null, true, false, true);
	}

	public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State off;

	public VignetteManager.OnStates on;

	public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isRedAlert = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();

	public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isYellowAlert = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();

	public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isOn = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();

	public class OnStates : GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State yellow;

		public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State red;
	}

	public new class Instance : GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			VignetteManager.Instance.instance = this;
		}

		public static void DestroyInstance()
		{
			VignetteManager.Instance.instance = null;
		}

		public static VignetteManager.Instance Get()
		{
			return VignetteManager.Instance.instance;
		}

		public void UpdateState(float dt)
		{
			if (this.IsRedAlert())
			{
				base.smi.GoTo(base.sm.on.red);
			}
			else if (this.IsYellowAlert())
			{
				base.smi.GoTo(base.sm.on.yellow);
			}
			else if (!this.IsOn())
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

		public void HasTopPriorityChore(bool on)
		{
			this.hasTopPriorityChore = on;
			this.Refresh();
		}

		private void Refresh()
		{
			base.sm.isYellowAlert.Set(this.hasTopPriorityChore, base.smi);
			base.sm.isRedAlert.Set(this.isToggled, base.smi);
			base.sm.isOn.Set(this.hasTopPriorityChore || this.isToggled, base.smi);
		}

		private static VignetteManager.Instance instance;

		private bool isToggled;

		private bool hasTopPriorityChore;

		public Notification redAlertNotification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, false, 0f, null, null, null);
	}
}
