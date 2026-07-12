using System;
using STRINGS;
using UnityEngine;

public class RescueSweepBotChore : Chore<RescueSweepBotChore.StatesInstance>
{
	public RescueSweepBotChore(IStateMachineTarget master, GameObject sweepBot, GameObject baseStation)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "CanReachBaseStation";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			return !(kmonoBehaviour == null) && context.consumerState.consumer.navigator.CanReach(Grid.PosToCell(kmonoBehaviour));
		};
		this.CanReachBaseStation = precondition;
		base..ctor(Db.Get().ChoreTypes.RescueIncapacitated, master, null, false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime);
		base.smi = new RescueSweepBotChore.StatesInstance(this);
		base.runUntilComplete = true;
		base.AddPrecondition(RescueSweepBotChore.CanReachIncapacitated, sweepBot.GetComponent<Storage>());
		base.AddPrecondition(this.CanReachBaseStation, baseStation.GetComponent<Storage>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.rescuer.Set(context.consumerState.gameObject, base.smi, false);
		base.smi.sm.rescueTarget.Set(this.gameObject, base.smi, false);
		base.smi.sm.deliverTarget.Set(this.gameObject.GetSMI<SweepBotTrappedStates.Instance>().sm.GetSweepLocker(this.gameObject.GetSMI<SweepBotTrappedStates.Instance>()).gameObject, base.smi, false);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		this.DropSweepBot();
		base.End(reason);
	}

	private void DropSweepBot()
	{
		if (base.smi.sm.rescuer.Get(base.smi) != null && base.smi.sm.rescueTarget.Get(base.smi) != null)
		{
			base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>().Drop(base.smi.sm.rescueTarget.Get(base.smi), true);
		}
	}

	public Chore.Precondition CanReachBaseStation;

	public static Chore.Precondition CanReachIncapacitated = new Chore.Precondition
	{
		id = "CanReachIncapacitated",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			if (kmonoBehaviour == null)
			{
				return false;
			}
			int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(kmonoBehaviour.transform.GetPosition()));
			if (-1 != navigationCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		}
	};

	public class StatesInstance : GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.GameInstance
	{
		public StatesInstance(RescueSweepBotChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approachSweepBot;
			this.approachSweepBot.InitializeStates(this.rescuer, this.rescueTarget, this.holding.pickup, this.failure, Grid.DefaultOffset, null);
			this.holding.Target(this.rescuer).Enter(delegate(RescueSweepBotChore.StatesInstance smi)
			{
				KAnimFile anim = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
				this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().AddAnimOverrides(anim, 0f);
			}).Exit(delegate(RescueSweepBotChore.StatesInstance smi)
			{
				KAnimFile anim2 = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim2);
			});
			this.holding.pickup.Target(this.rescuer).PlayAnim("pickup").Enter(delegate(RescueSweepBotChore.StatesInstance smi)
			{
			})
				.Exit(delegate(RescueSweepBotChore.StatesInstance smi)
				{
					this.rescuer.Get(smi).GetComponent<Storage>().Store(this.rescueTarget.Get(smi), false, false, true, false);
					this.rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = this.rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.symbol = new HashedString("snapTo_pivot");
					component.offset = new Vector3(0f, 0f, 1f);
				})
				.EventTransition(GameHashes.AnimQueueComplete, this.holding.delivering, null);
			this.holding.delivering.InitializeStates(this.rescuer, this.deliverTarget, this.holding.deposit, this.holding.ditch, null, null).Update(delegate(RescueSweepBotChore.StatesInstance smi, float dt)
			{
				if (this.deliverTarget.Get(smi) == null)
				{
					smi.GoTo(this.holding.ditch);
				}
			}, UpdateRate.SIM_200ms, false);
			this.holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, delegate(RescueSweepBotChore.StatesInstance smi)
			{
				smi.master.DropSweepBot();
				smi.SetStatus(StateMachine.Status.Success);
				smi.StopSM("complete");
			});
			this.holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, this.failure).Exit(delegate(RescueSweepBotChore.StatesInstance smi)
			{
				smi.master.DropSweepBot();
			});
			this.failure.Enter(delegate(RescueSweepBotChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
				smi.StopSM("failed");
			});
		}

		public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.ApproachSubState<Storage> approachSweepBot;

		public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State failure;

		public RescueSweepBotChore.States.HoldingSweepBot holding;

		public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter rescueTarget;

		public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter deliverTarget;

		public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter rescuer;

		public class HoldingSweepBot : GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State
		{
			public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State pickup;

			public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.ApproachSubState<IApproachable> delivering;

			public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State deposit;

			public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State ditch;
		}
	}
}
