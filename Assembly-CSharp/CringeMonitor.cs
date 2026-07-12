using System;

public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.Cringe, new GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.TriggerCringe));
		this.cringe.ToggleReactable((CringeMonitor.Instance smi) => smi.GetReactable()).ToggleStatusItem((CringeMonitor.Instance smi) => smi.GetStatusItem(), null).ScheduleGoTo(3f, this.idle);
	}

	private void TriggerCringe(CringeMonitor.Instance smi, object data)
	{
		if (smi.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
		{
			return;
		}
		smi.SetCringeSourceData(data);
		smi.GoTo(this.cringe);
	}

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
			this.statusItem = new StatusItem("CringeSource", text, null, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
		}

		public Reactable GetReactable()
		{
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, "Cringe", Db.Get().ChoreTypes.EmoteHighPriority, 0f, 0f, float.PositiveInfinity, 0f);
			selfEmoteReactable.SetEmote(Db.Get().Emotes.Minion.Cringe);
			selfEmoteReactable.preventChoreInterruption = true;
			return selfEmoteReactable;
		}

		public StatusItem GetStatusItem()
		{
			return this.statusItem;
		}

		private StatusItem statusItem;
	}
}
