using System;
using STRINGS;
using UnityEngine;

public class RescueIncapacitatedChore : Chore<RescueIncapacitatedChore.StatesInstance>
{
	public RescueIncapacitatedChore(IStateMachineTarget master, GameObject incapacitatedDuplicant)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, null, false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new RescueIncapacitatedChore.StatesInstance(this);
		base.runUntilComplete = true;
		base.AddPrecondition(ChorePreconditions.instance.NotChoreCreator, incapacitatedDuplicant.gameObject);
		base.AddPrecondition(RescueIncapacitatedChore.CanReachIncapacitated, incapacitatedDuplicant);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.rescuer.Set(context.consumerState.gameObject, base.smi, false);
		base.smi.sm.rescueTarget.Set(this.gameObject, base.smi, false);
		base.smi.sm.deliverTarget.Set(this.gameObject.GetSMI<BeIncapacitatedChore.StatesInstance>().master.GetChosenClinic(), base.smi, false);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		this.DropIncapacitatedDuplicant();
		base.End(reason);
	}

	private void DropIncapacitatedDuplicant()
	{
		if (base.smi.sm.rescuer.Get(base.smi) != null && base.smi.sm.rescueTarget.Get(base.smi) != null)
		{
			base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>().Drop(base.smi.sm.rescueTarget.Get(base.smi), true);
		}
	}

	public static Chore.Precondition CanReachIncapacitated = new Chore.Precondition
	{
		id = "CanReachIncapacitated",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject == null)
			{
				return false;
			}
			int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(gameObject.transform.GetPosition()));
			if (-1 != navigationCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		}
	};

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
			this.approachIncapacitated.InitializeStates(this.rescuer, this.rescueTarget, this.holding.pickup, this.failure, Grid.DefaultOffset, null).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				DeathMonitor.Instance smi2 = this.rescueTarget.GetSMI<DeathMonitor.Instance>(smi);
				if (smi2 == null || smi2.IsDead())
				{
					smi.StopSM("target died");
				}
			});
			this.holding.Target(this.rescuer).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.sm.rescueTarget.Get(smi).Subscribe(1623392196, delegate(object d)
				{
					smi.GoTo(this.holding.ditch);
				});
				if (this.rescuer.Get(smi).gameObject.HasTag(GameTags.Minion))
				{
					KAnimFile anim = Assets.GetAnim("anim_incapacitated_carrier_kanim");
					smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
					smi.master.GetComponent<KAnimControllerBase>().AddAnimOverrides(anim, 0f);
				}
			}).Exit(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				if (this.rescuer.Get(smi).gameObject.HasTag(GameTags.Minion))
				{
					KAnimFile anim2 = Assets.GetAnim("anim_incapacitated_carrier_kanim");
					smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim2);
				}
			});
			this.holding.pickup.Target(this.rescuer).PlayAnim("pickup").Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				this.rescueTarget.Get(smi).gameObject.GetComponent<KBatchedAnimController>().Play("pickup", KAnim.PlayMode.Once, 1f, 0f);
			})
				.Exit(delegate(RescueIncapacitatedChore.StatesInstance smi)
				{
					this.rescuer.Get(smi).GetComponent<Storage>().Store(this.rescueTarget.Get(smi), false, false, true, false);
					this.rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = this.rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
					if (component != null)
					{
						component.symbol = new HashedString("snapTo_pivot");
						component.offset = new Vector3(0f, 0f, 1f);
					}
				})
				.EventTransition(GameHashes.AnimQueueComplete, this.holding.delivering, null);
			this.holding.delivering.InitializeStates(this.rescuer, this.deliverTarget, this.holding.deposit, this.holding.ditch, null, null).Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				DeathMonitor.Instance smi3 = this.rescueTarget.GetSMI<DeathMonitor.Instance>(smi);
				if (smi3 == null || smi3.IsDead())
				{
					smi.StopSM("target died");
				}
			}).Update(delegate(RescueIncapacitatedChore.StatesInstance smi, float dt)
			{
				if (this.deliverTarget.Get(smi) == null)
				{
					smi.GoTo(this.holding.ditch);
				}
			}, UpdateRate.SIM_200ms, false);
			this.holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
				smi.SetStatus(StateMachine.Status.Success);
				smi.StopSM("complete");
			});
			this.holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, this.failure).Exit(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
			});
			this.failure.Enter(delegate(RescueIncapacitatedChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
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

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.ApproachSubState<IApproachable> delivering;

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State deposit;

			public GameStateMachine<RescueIncapacitatedChore.States, RescueIncapacitatedChore.StatesInstance, RescueIncapacitatedChore, object>.State ditch;
		}
	}
}
