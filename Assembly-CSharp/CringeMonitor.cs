using System;

public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.Cringe, new GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.TriggerCringe));
		this.cringe.ToggleChore((CringeMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_cringe_kanim", CringeMonitor.CringeAnims, new Func<StatusItem>(smi.GetStatusItem)), this.idle).ScheduleGoTo(3f, this.idle);
	}

	private void TriggerCringe(CringeMonitor.Instance smi, object data)
	{
		smi.SetCringeSourceData(data);
		smi.GoTo(this.cringe);
	}

	private static readonly HashedString[] CringeAnims = new HashedString[] { "cringe_pre", "cringe_loop", "cringe_pst" };

	public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.State cringe;

	public new class Instance : GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetCringeSourceData(object data)
		{
			string text = (string)data;
			this.statusItem = new StatusItem("CringeSource", text, null, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, 14334);
		}

		public StatusItem GetStatusItem()
		{
			return this.statusItem;
		}

		private StatusItem statusItem;
	}
}
