using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class RoboDancerChore : Chore<RoboDancerChore.StatesInstance>, IWorkerPrioritizable
{
	public RoboDancerChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.JoyReaction, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		this.showAvailabilityInHoverText = false;
		base.smi = new RoboDancerChore.StatesInstance(this, target.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		this.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	public bool GetWorkerPriority(WorkerBase worker, out int priority)
	{
		priority = this.basePriority;
		return true;
	}

	private int basePriority = RELAXATION.PRIORITY.TIER1;

	public class States : GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.goToStand;
			base.Target(this.roboDancer);
			this.idle.EventTransition(GameHashes.ScheduleBlocksTick, this.goToStand, (RoboDancerChore.StatesInstance smi) => !smi.IsRecTime());
			this.goToStand.MoveTo((RoboDancerChore.StatesInstance smi) => smi.GetTargetCell(), this.dancing, this.idle, false);
			this.dancing.ToggleEffect("Dancing").ToggleAnims("anim_bionic_joy_kanim", 0f).DefaultState(this.dancing.pre)
				.Update(delegate(RoboDancerChore.StatesInstance smi, float dt)
				{
					RoboDancer.Instance smi2 = this.roboDancer.Get(smi).GetSMI<RoboDancer.Instance>();
					RoboDancer sm = smi2.sm;
					sm.hasAudience.Set(smi.HasAudience(), smi2, false);
					sm.timeSpentDancing.Set(sm.timeSpentDancing.Get(smi2) + dt, smi2, false);
				}, UpdateRate.SIM_33ms, false)
				.Exit(delegate(RoboDancerChore.StatesInstance smi)
				{
					smi.ClearAudienceWorkables();
				});
			this.dancing.pre.QueueAnim("robotdance_pre", false, null).OnAnimQueueComplete(this.dancing.variation_1).Enter(delegate(RoboDancerChore.StatesInstance smi)
			{
				smi.ClearAudienceWorkables();
				smi.CreateAudienceWorkables();
			});
			this.dancing.variation_1.QueueAnim("robotdance_loop", false, null).OnAnimQueueComplete(this.dancing.variation_2);
			this.dancing.variation_2.QueueAnim("robotdance_2_loop", false, null).OnAnimQueueComplete(this.dancing.pst);
			this.dancing.pst.QueueAnim("robotdance_pst", false, null).OnAnimQueueComplete(this.dancing.pre);
		}

		public StateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.TargetParameter roboDancer;

		public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State idle;

		public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State goToStand;

		public RoboDancerChore.States.DancingStates dancing;

		public class DancingStates : GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State
		{
			public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State pre;

			public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State variation_1;

			public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State variation_2;

			public GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.State pst;
		}
	}

	public class StatesInstance : GameStateMachine<RoboDancerChore.States, RoboDancerChore.StatesInstance, RoboDancerChore, object>.GameInstance
	{
		public StatesInstance(RoboDancerChore master, GameObject roboDancer)
		{
			Chore.Precondition precondition = default(Chore.Precondition);
			precondition.id = "IsNotRoboHyped";
			precondition.description = "__ Duplicant hasn't watched the dance yet";
			precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return !(context.consumerState.consumer == null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect(WatchRoboDancerWorkable.TRACKING_EFFECT);
			};
			this.IsNotRoboHyped = precondition;
			base..ctor(master);
			this.roboDancer = roboDancer;
			base.sm.roboDancer.Set(roboDancer, base.smi, false);
		}

		public bool IsRecTime()
		{
			return base.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public int GetTargetCell()
		{
			Navigator component = base.GetComponent<Navigator>();
			float num = float.MaxValue;
			SocialGatheringPoint socialGatheringPoint = null;
			foreach (SocialGatheringPoint socialGatheringPoint2 in Components.SocialGatheringPoints.GetItems((int)Grid.WorldIdx[Grid.PosToCell(this)]))
			{
				float num2 = (float)component.GetNavigationCost(Grid.PosToCell(socialGatheringPoint2));
				if (num2 != -1f && num2 < num)
				{
					num = num2;
					socialGatheringPoint = socialGatheringPoint2;
				}
			}
			if (socialGatheringPoint != null)
			{
				return Grid.PosToCell(socialGatheringPoint);
			}
			return Grid.PosToCell(base.master.gameObject);
		}

		public bool HasAudience()
		{
			if (base.smi.watchWorkables == null)
			{
				return false;
			}
			WatchRoboDancerWorkable[] array = base.smi.watchWorkables;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].worker)
				{
					return true;
				}
			}
			return false;
		}

		public void CreateAudienceWorkables()
		{
			int num = Grid.PosToCell(base.gameObject);
			Vector3Int[] array = new Vector3Int[]
			{
				Vector3Int.left * 3,
				Vector3Int.left * 2,
				Vector3Int.left,
				Vector3Int.right,
				Vector3Int.right * 2,
				Vector3Int.right * 3
			};
			int num2 = 0;
			for (int i = 0; i < this.audienceWorkables.Length; i++)
			{
				int num3 = Grid.OffsetCell(num, array[i].x, array[i].y);
				if (Grid.IsValidCellInWorld(num3, (int)Grid.WorldIdx[num]))
				{
					GameObject gameObject = ChoreHelpers.CreateLocator("WatchRoboDancerWorkable", Grid.CellToPos(num3));
					this.audienceWorkables[i] = gameObject;
					KSelectable kselectable = gameObject.AddOrGet<KSelectable>();
					kselectable.SetName("WatchRoboDancerWorkable");
					kselectable.IsSelectable = false;
					WatchRoboDancerWorkable watchRoboDancerWorkable = gameObject.AddOrGet<WatchRoboDancerWorkable>();
					watchRoboDancerWorkable.owner = this.roboDancer;
					WorkChore<WatchRoboDancerWorkable> workChore = new WorkChore<WatchRoboDancerWorkable>(Db.Get().ChoreTypes.JoyReaction, watchRoboDancerWorkable, null, true, null, null, null, true, Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, true, PriorityScreen.PriorityClass.high, 5, false, true);
					workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
					workChore.AddPrecondition(this.IsNotRoboHyped, workChore);
					num2++;
				}
			}
			this.watchWorkables = new WatchRoboDancerWorkable[num2];
			for (int j = 0; j < num2; j++)
			{
				this.watchWorkables[j] = this.audienceWorkables[j].GetComponent<WatchRoboDancerWorkable>();
			}
		}

		public void ClearAudienceWorkables()
		{
			for (int i = 0; i < this.audienceWorkables.Length; i++)
			{
				if (!(this.audienceWorkables[i] == null))
				{
					WorkerBase worker = this.audienceWorkables[i].GetComponent<WatchRoboDancerWorkable>().worker;
					if (worker != null)
					{
						this.audienceWorkables[i].GetComponent<WatchRoboDancerWorkable>().CompleteWork(worker);
					}
					ChoreHelpers.DestroyLocator(this.audienceWorkables[i]);
				}
			}
			this.watchWorkables = null;
		}

		private GameObject roboDancer;

		private GameObject[] audienceWorkables = new GameObject[4];

		private WatchRoboDancerWorkable[] watchWorkables;

		private Chore.Precondition IsNotRoboHyped;
	}
}
