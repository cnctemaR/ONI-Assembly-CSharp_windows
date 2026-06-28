using System;
using System.Collections.Generic;
using UnityEngine;

public class PrioritizedChore : Chore<PrioritizedChore.StatesInstance>
{
	public PrioritizedChore(ChoreType chore_type, IStateMachineTarget target, Chore chore)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), true, null, null, null, int.MaxValue, false, true)
	{
		this.chore = chore;
		this.smi = new PrioritizedChore.StatesInstance(this, target.gameObject);
	}

	public override void CollectChores(ChoreConsumer consumer, List<Chore.Precondition.Context> contexts, bool is_attempting_override)
	{
		if (this.chore.overrideTarget == consumer)
		{
			Chore.Precondition.Context context = new Chore.Precondition.Context(this.chore, consumer, false, null);
			context.masterPriority = base.masterPriority;
			context.SetPriority(this);
			context.RunPreconditions();
			contexts.Add(context);
		}
	}

	private Chore chore;

	public class StatesInstance : GameStateMachine<PrioritizedChore.States, PrioritizedChore.StatesInstance, PrioritizedChore>.GameInstance
	{
		public StatesInstance(PrioritizedChore master, GameObject worker)
			: base(master)
		{
			base.sm.worker.Set(worker, base.smi);
		}
	}

	public class States : GameStateMachine<PrioritizedChore.States, PrioritizedChore.StatesInstance, PrioritizedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.running;
			base.Target(this.worker);
			this.running.DoNothing();
		}

		public StateMachine<PrioritizedChore.States, PrioritizedChore.StatesInstance, PrioritizedChore>.TargetParameter worker;

		public GameStateMachine<PrioritizedChore.States, PrioritizedChore.StatesInstance, PrioritizedChore>.State running;
	}
}
