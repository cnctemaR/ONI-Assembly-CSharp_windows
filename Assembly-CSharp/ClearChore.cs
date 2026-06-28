using System;
using System.Collections.Generic;

public class ClearChore : Chore<ClearChore.StatesInstance>
{
	public ClearChore(ChoreType chore_type, Pickupable clearable, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(chore_type, clearable, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, 0, false, true, 0, null)
	{
		this.smi = new ClearChore.StatesInstance(this);
		this.smi.sm.clearable.Set(clearable, this.smi);
		base.SetPrioritizable(clearable.GetComponent<Prioritizable>());
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		Pickupable pickupable = this.smi.sm.clearable.Get<Pickupable>(this.smi);
		int fetchCount = GlobalChoreProvider.Instance.fetchCount;
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		context.personalPriority = consumer_state.consumer.GetPersonalPriority(base.choreType);
		TagBits tagBits = pickupable.KPrefabID.GetTagBits();
		for (int i = 0; i < fetchCount; i++)
		{
			GlobalChoreProvider.Fetch fetch = GlobalChoreProvider.Instance.fetches[i];
			bool flag = tagBits.HasAny(fetch.tagBits);
			if (flag)
			{
				context.Set(fetch.chore, consumer_state, is_attempting_override, pickupable);
				context.choreTypeForPermission = base.choreType;
				context.RunPreconditions();
				if (context.IsSuccess())
				{
					context.masterPriority = this.masterPriority;
					context.SetPriority(this);
					succeeded_contexts.Add(context);
					break;
				}
			}
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
		if (this.gameObject != null)
		{
			this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.NoClearLocationsAvailable, false);
		}
	}

	public class StatesInstance : GameStateMachine<ClearChore.States, ClearChore.StatesInstance, ClearChore, object>.GameInstance
	{
		public StatesInstance(ClearChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<ClearChore.States, ClearChore.StatesInstance, ClearChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
		}

		public StateMachine<ClearChore.States, ClearChore.StatesInstance, ClearChore, object>.TargetParameter clearable;
	}
}
