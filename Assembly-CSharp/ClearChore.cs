using System;
using System.Collections.Generic;

public class ClearChore : Chore<ClearChore.StatesInstance>
{
	public ClearChore(ChoreType chore_type, Pickupable clearable, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(chore_type, clearable, chore_provider, run_until_complete, on_complete, on_begin, on_end, int.MaxValue, false, true, 0)
	{
		this.smi = new ClearChore.StatesInstance(this);
		this.smi.sm.clearable.Set(clearable, this.smi);
		base.SetPrioritizable(clearable.GetComponent<Prioritizable>());
	}

	public override void CollectChores(ChoreConsumer consumer, List<Chore.Precondition.Context> contexts, bool is_attempting_override)
	{
		Pickupable pickupable = this.smi.sm.clearable.Get<Pickupable>(this.smi);
		int fetchCount = GlobalChoreProvider.Instance.fetchCount;
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		for (int i = 0; i < GlobalChoreProvider.Instance.fetchCount; i++)
		{
			GlobalChoreProvider.Fetch fetch = GlobalChoreProvider.Instance.fetches[i];
			bool flag = pickupable.KPrefabID.HasAnyTags(fetch.tags);
			if (flag)
			{
				context.Set(fetch.chore, consumer, is_attempting_override, pickupable);
				context.RunPreconditions();
				if (context.IsSuccess())
				{
					context.masterPriority = base.masterPriority;
					context.SetPriority(this);
					contexts.Add(context);
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
