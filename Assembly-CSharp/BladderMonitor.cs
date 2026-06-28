using System;
using Klei.AI;

public class BladderMonitor : GameStateMachine<BladderMonitor, BladderMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.needstopee, (BladderMonitor.Instance smi) => smi.NeedsToPee());
		this.needstopee.ToggleUrge(Db.Get().Urges.Pee).EventTransition(GameHashes.BeginChore, this.needstopee.peeing, (BladderMonitor.Instance smi) => smi.IsPeeing()).DefaultState(this.needstopee.holdingitin)
			.ToggleThought(Db.Get().Thoughts.FullBladder, null)
			.ToggleExpression(Db.Get().Expressions.FullBladder, null)
			.ToggleStateMachine((BladderMonitor.Instance smi) => new ToiletMonitor.Instance(smi.master))
			.ToggleStateMachine((BladderMonitor.Instance smi) => new PeeChoreMonitor.Instance(smi.master));
		this.needstopee.holdingitin.ToggleEffect("FullBladder").DefaultState(this.needstopee.holdingitin.nobathrooms).ToggleSchedulePeriodic("check bathrooms", 1f, delegate(BladderMonitor.Instance smi)
		{
			smi.CheckBathrooms();
		})
			.OnSignal(this.noBathrooms, this.needstopee.holdingitin.nobathrooms)
			.OnSignal(this.hasBathrooms, this.needstopee.holdingitin.hasbathrooms);
		this.needstopee.urgent.ToggleChore(new Func<BladderMonitor.Instance, Chore>(this.CreatePeeChore), this.satisfied);
		this.needstopee.peeing.EventTransition(GameHashes.EndChore, this.satisfied, (BladderMonitor.Instance smi) => !smi.IsPeeing());
		this.needstopee.holdingitin.nobathrooms.ToggleStatusItem(Db.Get().DuplicantStatusItems.NoToilets, null);
	}

	private Chore CreatePeeChore(BladderMonitor.Instance smi)
	{
		return new PeeChore(smi.master);
	}

	public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public BladderMonitor.NeedsToPeeState needstopee;

	public StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Signal noBathrooms;

	public StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Signal hasBathrooms;

	public class BathroomsState : GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State nobathrooms;

		public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State hasbathrooms;
	}

	public class NeedsToPeeState : GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State
	{
		public BladderMonitor.BathroomsState holdingitin;

		public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State urgent;

		public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State peeing;
	}

	public new class Instance : GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.bladder = Db.Get().Amounts.Bladder.Lookup(master.gameObject);
			this.choreDriver = base.GetComponent<ChoreDriver>();
		}

		public bool NeedsToPee()
		{
			return this.bladder.value >= 100f;
		}

		public bool IsPeeing()
		{
			return this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Pee);
		}

		public void CheckBathrooms()
		{
			if (Components.Toilets.Count > 0)
			{
				base.sm.hasBathrooms.Trigger(this);
			}
			else
			{
				base.sm.noBathrooms.Trigger(this);
			}
		}

		private AmountInstance bladder;

		private ChoreDriver choreDriver;
	}
}
