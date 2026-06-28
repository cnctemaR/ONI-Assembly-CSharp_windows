using System;
using UnityEngine;

public class RescueIncapacitatedChore : Chore<RescueIncapacitatedChore.StatesInstance>
{
	public RescueIncapacitatedChore(IStateMachineTarget master, GameObject incapacitatedDuplicant)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, null, false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new RescueIncapacitatedChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.NotChoreCreator, incapacitatedDuplicant.gameObject);
		base.AddPrecondition(ChorePreconditions.CanMoveTo, incapacitatedDuplicant.GetComponent<Workable>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.rescuer.Set(context.consumer.gameObject, this.smi);
		this.smi.sm.rescueTarget.Set(this.gameObject, this.smi);
		this.smi.sm.deliverTarget.Set(this.gameObject.GetSMI<BeIncapacitatedChore.StatesInstance>().master.GetChosenClinic(), this.smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		this.DropIncapacitatedDuplicant();
		base.End(reason);
	}

	private void DropIncapacitatedDuplicant()
	{
		if (this.smi.sm.rescuer.Get(this.smi) != null && this.smi.sm.rescueTarget.Get(this.smi) != null)
		{
			this.smi.sm.rescuer.Get(this.smi).GetComponent<Storage>().Drop(this.smi.sm.rescueTarget.Get(this.smi));
		}
	}

	public class StatesInstance : GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(RescueIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approachIncapacitated;
			this.approachIncapacitated.InitializeStates(this.rescuer, this.rescueTarget, this.holding.pickup, this.failure, null, null).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				Health health = this.rescueTarget.Get<Health>(smi);
				if (health == null || health.IsDead())
				{
					smi.StopSM("target died");
				}
			});
			this.holding.Target(this.rescuer).ToggleAnims("anim_incapacitated_carrier_kanim", 0f).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.sm.rescueTarget.Get(smi).Subscribe(1623392196, delegate(object d)
				{
					smi.GoTo(this.holding.ditch);
				});
			});
			this.holding.pickup.Target(this.rescuer).PlayAnim("pickup", KAnim.PlayMode.Once, null).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				this.rescueTarget.Get(smi).gameObject.GetComponent<KBatchedAnimController>().Play("pickup", KAnim.PlayMode.Once, 1f, 0f);
			})
				.Exit(delegate(RescueIncapacitatedChore.StatesInstance smi)
				{
					this.rescuer.Get(smi).GetComponent<Storage>().Store(this.rescueTarget.Get(smi), false, false);
					this.rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = this.rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.ignoreRotation = true;
					component.symbol = new HashedString("snapTo_pivot");
					component.offset = new Vector3(0f, 0f, 1f);
				})
				.EventTransition(GameHashes.AnimQueueComplete, this.holding.delivering, null);
			this.holding.delivering.InitializeStates(this.rescuer, this.deliverTarget, this.holding.deposit, this.holding.ditch, null, null).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				Health health2 = this.rescueTarget.Get<Health>(smi);
				if (health2 == null || health2.IsDead())
				{
					smi.StopSM("target died");
				}
			});
			this.holding.deposit.PlayAnim("place", KAnim.PlayMode.Once, null).EventHandler(GameHashes.AnimQueueComplete, delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
				smi.StopSM("complete");
			});
			this.holding.ditch.PlayAnim("place", KAnim.PlayMode.Once, null).ScheduleGoTo(0.5f, this.failure).Exit(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
			});
			this.failure.Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.StopSM("failed");
			});
		}

		public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.ApproachSubState<Chattable> approachIncapacitated;

		public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State failure;

		public RescueIncapacitatedChore.States.HoldingIncapacitated holding;

		public StateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.TargetParameter rescueTarget;

		public StateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.TargetParameter deliverTarget;

		public StateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.TargetParameter rescuer;

		public class HoldingIncapacitated : GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State
		{
			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State pickup;

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.ApproachSubState<Approachable> delivering;

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State deposit;

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State ditch;
		}
	}
}
