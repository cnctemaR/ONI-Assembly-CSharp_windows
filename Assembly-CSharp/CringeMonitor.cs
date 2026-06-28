using System;

public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.Cringe, new GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget>.GameEvent.Callback(this.TriggerCringe));
		this.cringe.ToggleChore((CringeMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_cringe", CringeMonitor.CringeAnims, new Func<StatusItem>(smi.GetStatusItem)), this.idle, false).ScheduleGoTo(3f, this.idle);
	}

	private void TriggerCringe(CringeMonitor.Instance smi, object data)
	{
		smi.SetCringeSourceData(data);
		smi.GoTo(this.cringe);
	}

	private static readonly string[] CringeAnims = new string[] { "cringe_pre", "cringe_loop", "cringe_pst" };

	public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget>.State idle;

	public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget>.State cringe;

	public new class Instance : GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetCringeSourceData(object data)
		{
			string text = (string)data;
			this.statusItem = new StatusItem("CringeSource", text, string.Empty, null, false, StatusItem.IconType.Exclamation, NotificationType.BadMinor, SimViewMode.None, SimViewMode.None);
		}

		public StatusItem GetStatusItem()
		{
			return this.statusItem;
		}

		private StatusItem statusItem;
	}
}
