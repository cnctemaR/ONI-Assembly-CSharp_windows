using System;
using System.Collections.Generic;

public class PartyCake : GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.creating.ready;
		this.creating.ready.PlayAnim("base").GoTo(this.creating.tier1);
		this.creating.tier1.InitializeStates(this.masterTarget, "tier_1", this.creating.tier2);
		this.creating.tier2.InitializeStates(this.masterTarget, "tier_2", this.creating.tier3);
		this.creating.tier3.InitializeStates(this.masterTarget, "tier_3", this.ready_to_party);
		this.ready_to_party.PlayAnim("unlit");
		this.party.PlayAnim("lit");
		this.complete.PlayAnim("finished");
	}

	private static Chore CreateFetchChore(PartyCake.StatesInstance smi)
	{
		return new FetchChore(Db.Get().ChoreTypes.FarmFetch, smi.GetComponent<Storage>(), 10f, new HashSet<Tag> { "MushBar".ToTag() }, FetchChore.MatchCriteria.MatchID, Tag.Invalid, null, null, true, null, null, null, Operational.State.Functional, 0);
	}

	private static Chore CreateWorkChore(PartyCake.StatesInstance smi)
	{
		Workable component = smi.master.GetComponent<PartyCakeWorkable>();
		return new WorkChore<PartyCakeWorkable>(Db.Get().ChoreTypes.Cook, component, null, true, null, null, null, false, Db.Get().ScheduleBlockTypes.Work, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
	}

	public PartyCake.CreatingStates creating;

	public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State ready_to_party;

	public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State party;

	public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State complete;

	public class Def : StateMachine.BaseDef
	{
	}

	public class CreatingStates : GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State
	{
		public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State ready;

		public PartyCake.CreatingStates.Tier tier1;

		public PartyCake.CreatingStates.Tier tier2;

		public PartyCake.CreatingStates.Tier tier3;

		public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State finish;

		public class Tier : GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State
		{
			public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State InitializeStates(StateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.TargetParameter targ, string animPrefix, GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State success)
			{
				base.root.Target(targ).DefaultState(this.fetch);
				this.fetch.PlayAnim(animPrefix + "_ready").ToggleChore(new Func<PartyCake.StatesInstance, Chore>(PartyCake.CreateFetchChore), this.work);
				this.work.Enter(delegate(PartyCake.StatesInstance smi)
				{
					KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
					component.Play(animPrefix + "_working", KAnim.PlayMode.Once, 1f, 0f);
					component.SetPositionPercent(0f);
				}).ToggleChore(new Func<PartyCake.StatesInstance, Chore>(PartyCake.CreateWorkChore), success, this.work);
				return this;
			}

			public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State fetch;

			public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State work;
		}
	}

	public class StatesInstance : GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget smi, PartyCake.Def def)
			: base(smi, def)
		{
		}
	}
}
