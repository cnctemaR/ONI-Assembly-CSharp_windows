using System;
using STRINGS;

public class QuarantineMonitor : GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.ParamTransition<bool>(this.isQuarantined, this.quarantined, (QuarantineMonitor.Instance smi, bool p) => smi.IsQuarantined());
		this.quarantined.ParamTransition<bool>(this.isQuarantined, this.satisfied, (QuarantineMonitor.Instance smi, bool p) => !smi.IsQuarantined()).EventTransition(GameHashes.AssignablesChanged, this.quarantined.bedunassigned, (QuarantineMonitor.Instance smi) => !smi.HasQuarantineArea()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Quarantined, null)
			.DefaultState(this.quarantined.bedunassigned);
		this.quarantined.bedunassigned.ToggleStatusItem(Db.Get().DuplicantStatusItems.QuarantineAreaUnassigned, null).EventTransition(GameHashes.AssignablesChanged, this.quarantined.outside, (QuarantineMonitor.Instance smi) => smi.HasQuarantineArea());
		this.quarantined.bedunreachable.ToggleStatusItem(Db.Get().DuplicantStatusItems.QuarantineAreaUnreachable, null);
		this.quarantined.outside.ToggleUrge(Db.Get().Urges.MoveToQuarantine).ToggleChore((QuarantineMonitor.Instance smi) => new MoveToQuarantineChore(smi.master, smi.GetQuarantineArea()), this.quarantined.inside, false);
		this.quarantined.inside.ToggleStateMachine((QuarantineMonitor.Instance smi) => new QuarantineFeedableMonitor.Instance(smi.master)).ToggleStateMachine((QuarantineMonitor.Instance smi) => new PatientMonitor.Instance(smi.master)).EventTransition(GameHashes.CellChanged, this.quarantined.outside, (QuarantineMonitor.Instance smi) => !smi.IsInsideQuarantine())
			.Enter("EnableNavMask", delegate(QuarantineMonitor.Instance smi)
			{
				smi.EnableNavMask();
			})
			.Exit("DisableNavMask", delegate(QuarantineMonitor.Instance smi)
			{
				smi.DisableNavMask();
			});
	}

	private StateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.BoolParameter isQuarantined = new StateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.BoolParameter();

	public GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public QuarantineMonitor.QuarantinedState quarantined;

	public class QuarantinedState : GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State bedunassigned;

		public GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State bedunreachable;

		public GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State outside;

		public GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.State inside;
	}

	public new class Instance : GameStateMachine<QuarantineMonitor, QuarantineMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		}

		public KMonoBehaviour GetQuarantineArea()
		{
			return Db.Get().OwnableSlots.Bed.Get(base.master.gameObject).assignable;
		}

		public bool HasQuarantineArea()
		{
			return this.GetQuarantineArea() != null;
		}

		public bool IsInsideQuarantine()
		{
			return true;
		}

		public bool IsQuarantined()
		{
			return base.sm.isQuarantined.Get(base.smi);
		}

		public void EnableNavMask()
		{
		}

		public void DisableNavMask()
		{
		}

		private void OnClickQuarantine()
		{
			base.sm.isQuarantined.Set(true, base.smi);
			base.GetComponent<UserMenu>().Refresh();
		}

		private void OnClickCancelQuarantine()
		{
			base.sm.isQuarantined.Set(false, base.smi);
			base.GetComponent<UserMenu>().Refresh();
		}

		private void OnRefreshUserMenu(object data)
		{
			string text;
			string text2;
			global::System.Action action;
			if (this.IsQuarantined())
			{
				text = UI.USERMENUACTIONS.QUARANTINE.NAME_OFF;
				text2 = UI.USERMENUACTIONS.QUARANTINE.TOOLTIP_OFF;
				action = new global::System.Action(this.OnClickCancelQuarantine);
			}
			else
			{
				text = UI.USERMENUACTIONS.QUARANTINE.NAME;
				text2 = UI.USERMENUACTIONS.QUARANTINE.TOOLTIP;
				if (!this.HasBedRoom())
				{
					text2 = UI.USERMENUACTIONS.QUARANTINE.TOOLTIP_DISABLED;
				}
				action = new global::System.Action(this.OnClickQuarantine);
			}
			UserMenu component = base.GetComponent<UserMenu>();
			UserMenu userMenu = component;
			string text3 = text2;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_quarantine", text, action, global::Action.NumActions, null, null, null, text3, true), 1f);
		}

		private bool HasBedRoom()
		{
			Assignable assignable = base.smi.master.GetComponent<Ownables>().GetAssignable(Db.Get().OwnableSlots.Bed);
			return assignable != null;
		}

		public override void StopSM(string reason)
		{
			base.master.Unsubscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
			base.StopSM(reason);
		}

		private NavMask navMask;
	}
}
