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
			.EventTransition(GameHashes.EntombedChanged, this.unfetchable, (FetchableMonitor.Instance smi) => !smi.IsFetchable())
			.EventTransition(GameHashes.TagsChanged, this.unfetchable, (FetchableMonitor.Instance smi) => !smi.IsFetchable())
			.ParamTransition<bool>(this.forceUnfetchable, this.unfetchable, (FetchableMonitor.Instance smi, bool p) => !smi.IsFetchable());
		this.unfetchable.EventTransition(GameHashes.ReachableChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable()).EventTransition(GameHashes.AssigneeChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable()).EventTransition(GameHashes.EntombedChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable())
			.EventTransition(GameHashes.TagsChanged, this.fetchable, (FetchableMonitor.Instance smi) => smi.IsFetchable())
			.ParamTransition<bool>(this.forceUnfetchable, this.fetchable, (FetchableMonitor.Instance smi, bool p) => smi.IsFetchable());
	}

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.State fetchable;

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.State unfetchable;

	public StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.BoolParameter forceUnfetchable = new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.BoolParameter(false);

	public new class Instance : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.GameInstance
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

		public void SetForceUnfetchable(bool is_unfetchable)
		{
			base.sm.forceUnfetchable.Set(is_unfetchable, base.smi);
		}

		public bool IsFetchable()
		{
			return !base.sm.forceUnfetchable.Get(this) && !this.pickupable.IsEntombed && this.pickupable.IsReachable() && (!(this.equippable != null) || !this.equippable.isEquipped);
		}

		private Pickupable pickupable;

		private Equippable equippable;
	}
}
