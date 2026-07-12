using System;

public class FetchableMonitor : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.unfetchable;
		base.serializable = StateMachine.SerializeType.Never;
		this.fetchable.Enter("RegisterFetchable", delegate(FetchableMonitor.Instance smi)
		{
			smi.RegisterFetchable();
		}).Exit("UnregisterFetchable", delegate(FetchableMonitor.Instance smi)
		{
			smi.UnregisterFetchable();
		}).EventTransition(GameHashes.ReachableChanged, this.unfetchable, GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)))
			.EventTransition(GameHashes.AssigneeChanged, this.unfetchable, GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)))
			.EventTransition(GameHashes.EntombedChanged, this.unfetchable, GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)))
			.EventTransition(GameHashes.TagsChanged, this.unfetchable, GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)))
			.EventHandler(GameHashes.OnStore, new GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.UpdateStorage))
			.EventHandler(GameHashes.StoragePriorityChanged, new GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.UpdateStorage))
			.EventHandler(GameHashes.TagsChanged, new GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.UpdateTags))
			.ParamTransition<bool>(this.forceUnfetchable, this.unfetchable, (FetchableMonitor.Instance smi, bool p) => !smi.IsFetchable());
		this.unfetchable.EventTransition(GameHashes.ReachableChanged, this.fetchable, new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)).EventTransition(GameHashes.AssigneeChanged, this.fetchable, new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable)).EventTransition(GameHashes.EntombedChanged, this.fetchable, new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable))
			.EventTransition(GameHashes.TagsChanged, this.fetchable, new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(this.IsFetchable))
			.ParamTransition<bool>(this.forceUnfetchable, this.fetchable, new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.Parameter<bool>.Callback(this.IsFetchable));
	}

	private bool IsFetchable(FetchableMonitor.Instance smi, bool param)
	{
		return this.IsFetchable(smi);
	}

	private bool IsFetchable(FetchableMonitor.Instance smi)
	{
		return smi.IsFetchable();
	}

	private void UpdateStorage(FetchableMonitor.Instance smi, object data)
	{
		Game.Instance.fetchManager.UpdateStorage(smi.pickupable.PrefabID(), smi.fetchable, data as Storage);
	}

	private void UpdateTags(FetchableMonitor.Instance smi, object data)
	{
		Game.Instance.fetchManager.UpdateTags(smi.pickupable.PrefabID(), smi.fetchable);
	}

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.State fetchable;

	public GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.State unfetchable;

	public StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.BoolParameter forceUnfetchable = new StateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.BoolParameter(false);

	public new class Instance : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(Pickupable pickupable)
			: base(pickupable)
		{
			this.pickupable = pickupable;
			this.equippable = base.GetComponent<Equippable>();
		}

		public void RegisterFetchable()
		{
			this.fetchable = Game.Instance.fetchManager.Add(this.pickupable);
			Game.Instance.Trigger(-1588644844, base.gameObject);
		}

		public void UnregisterFetchable()
		{
			Game.Instance.fetchManager.Remove(base.smi.pickupable.KPrefabID.PrefabID(), this.fetchable);
			Game.Instance.Trigger(-1491270284, base.gameObject);
		}

		public void SetForceUnfetchable(bool is_unfetchable)
		{
			base.sm.forceUnfetchable.Set(is_unfetchable, base.smi, false);
		}

		public bool IsFetchable()
		{
			return !base.sm.forceUnfetchable.Get(this) && !this.pickupable.IsEntombed && this.pickupable.IsReachable() && (!(this.equippable != null) || !this.equippable.isEquipped) && !this.pickupable.KPrefabID.HasTag(GameTags.StoredPrivate) && !this.pickupable.KPrefabID.HasTag(GameTags.Creatures.ReservedByCreature);
		}

		public Pickupable pickupable;

		private Equippable equippable;

		public HandleVector<int>.Handle fetchable;
	}
}
