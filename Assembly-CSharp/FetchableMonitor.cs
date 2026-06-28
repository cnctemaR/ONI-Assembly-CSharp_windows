using System;

public class FetchableMonitor : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.unfetchable;
		base.serializable = false;
		this.fetchable.Enter("RegisterFetchable", delegate(FetchableMonitor.Instance smi)
		{
			smi.RegisterFetchable();
		}).Exit("UnregisterFetchable", delegate(FetchableMonitor.Instance smi)
		{
			smi.UnregisterFetchable();
		}).EventTransition(GameHashes.ReachableChanged, this.unfetchable, (FetchableMonitor.Instance smi) => !smi.IsFetchable())
			.EventTransition(GameHashes.AssigneeChanged, this.unfetchable, (FetchableMonitor.Instance smi) => !smi.IsFetchable())
			.EventTransition(GameHashes.EntombedChanged, this.unfetchable, (FetchableMonitor.Instance smi) => !smi.IsFetchable());
		this.unfetchable.EventTransition(GameHashes.ReachableChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable()).EventTransition(GameHashes.AssigneeChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable()).EventTransition(GameHashes.EntombedChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable());
	}

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget>.State fetchable;

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget>.State unfetchable;

	public new class Instance : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.pickupable = base.GetComponent<Pickupable>();
			this.equippable = base.GetComponent<Equippable>();
		}

		public void RegisterFetchable()
		{
			Game.Instance.Trigger(-1588644844, base.gameObject);
			FetchManager.Instance.Add(this.pickupable);
		}

		public void UnregisterFetchable()
		{
			Game.Instance.Trigger(-1491270284, base.gameObject);
			FetchManager.Instance.Remove(this.pickupable);
		}

		public bool IsFetchable()
		{
			return this.pickupable != null && !this.pickupable.IsEntombed && this.pickupable.IsReachable() && (this.equippable == null || !this.equippable.IsEquipped());
		}

		private Pickupable pickupable;

		private Equippable equippable;
	}
}
