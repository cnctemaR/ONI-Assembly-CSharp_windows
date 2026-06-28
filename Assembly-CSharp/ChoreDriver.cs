using System;

public class ChoreDriver : StateMachineComponent<ChoreDriver.StatesInstance>
{
	public Chore GetCurrentChore()
	{
		return base.smi.GetCurrentChore();
	}

	public bool HasChore()
	{
		return base.smi.GetCurrentChore() != null;
	}

	public void StopChore()
	{
		base.smi.sm.stop.Trigger(base.smi);
	}

	public void SetChore(Chore.Precondition.Context context)
	{
		if (base.smi.GetCurrentChore() != context.chore)
		{
			this.StopChore();
			this.context = context;
			base.smi.sm.nextChore.Set(context.chore, base.smi);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpAdd]
	private User user;

	private Chore.Precondition.Context context;

	public class StatesInstance : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.GameInstance
	{
		public StatesInstance(ChoreDriver master)
			: base(master)
		{
			ChoreConsumer component = base.GetComponent<ChoreConsumer>();
			component.choreRulesChanged = (global::System.Action)Delegate.Combine(component.choreRulesChanged, new global::System.Action(this.OnChoreRulesChanged));
		}

		public void BeginChore()
		{
			Chore nextChore = this.GetNextChore();
			Chore chore = base.smi.sm.currentChore.Set(nextChore, base.smi);
			base.smi.sm.nextChore.Set(null, base.smi);
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, new Action<Chore>(this.OnChoreExit));
			chore.Begin(base.master.context);
			base.Trigger(-1988963660, chore);
		}

		public void EndChore(string reason)
		{
			if (this.GetCurrentChore() != null)
			{
				Chore currentChore = this.GetCurrentChore();
				base.smi.sm.currentChore.Set(null, base.smi);
				Chore chore = currentChore;
				chore.onExit = (Action<Chore>)Delegate.Remove(chore.onExit, new Action<Chore>(this.OnChoreExit));
				currentChore.Fail(reason);
				base.Trigger(1745615042, currentChore);
			}
		}

		private void OnChoreExit(Chore chore)
		{
			base.smi.sm.stop.Trigger(base.smi);
		}

		public Chore GetNextChore()
		{
			return base.smi.sm.nextChore.Get(base.smi);
		}

		public Chore GetCurrentChore()
		{
			return base.smi.sm.currentChore.Get(base.smi);
		}

		private void OnChoreRulesChanged()
		{
			Chore currentChore = this.GetCurrentChore();
			if (currentChore != null)
			{
				ChoreConsumer component = base.GetComponent<ChoreConsumer>();
				if (!component.IsPermittedOrEnabled(currentChore))
				{
					this.EndChore("Permissions changed");
				}
			}
		}
	}

	public class States : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.nochore;
			this.saveHistory = true;
			this.nochore.ParamTransition<Chore>(this.nextChore, this.haschore, (ChoreDriver.StatesInstance smi, Chore next_chore) => next_chore != null);
			this.haschore.Enter("BeginChore", delegate(ChoreDriver.StatesInstance smi)
			{
				smi.BeginChore();
			}).Exit("EndChore", delegate(ChoreDriver.StatesInstance smi)
			{
				smi.EndChore("ChoreDriver.SignalStop");
			}).OnSignal(this.stop, this.nochore);
		}

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> currentChore;

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> nextChore;

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.Signal stop;

		public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State nochore;

		public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State haschore;
	}
}
