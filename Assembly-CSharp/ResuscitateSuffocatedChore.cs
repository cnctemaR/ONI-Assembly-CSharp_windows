using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ResuscitateSuffocatedChore : Chore<ResuscitateSuffocatedChore.StatesInstance>
{
	public ResuscitateSuffocatedChore(IStateMachineTarget master, GameObject incapacitatedDuplicant)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, null, false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new ResuscitateSuffocatedChore.StatesInstance(this);
		this.runUntilComplete = true;
		this.AddPrecondition(ChorePreconditions.instance.NotChoreCreator, incapacitatedDuplicant.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
		this.AddPrecondition(ResuscitateSuffocatedChore.CanReachIncapacitated, incapacitatedDuplicant);
		this.AddPrecondition(ResuscitateSuffocatedChore.CanReachOxygenatedArea, incapacitatedDuplicant);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.rescuer.Set(context.consumerState.gameObject, base.smi, false);
		base.smi.sm.rescueTarget.Set(this.gameObject, base.smi, false);
		base.smi.resucerController = context.consumerState.gameObject.GetComponent<KBatchedAnimController>();
		base.smi.rescueeController = this.gameObject.GetComponent<KBatchedAnimController>();
		Vector3 vector = Grid.CellToPosCBC(ResuscitateSuffocatedChore.StatesInstance.FindClosestOxygenCellToIncapacitated(context.consumerState.navigator, Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Move);
		GameObject gameObject = ChoreHelpers.CreateLocator("OxygenCell", vector);
		base.smi.sm.deliverTarget.Set(gameObject, base.smi, false);
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
			Storage component = base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>();
			GameObject gameObject = base.smi.sm.rescueTarget.Get(base.smi);
			if (component.items.Contains(gameObject))
			{
				base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>().Drop(gameObject, true);
			}
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

	public static Chore.Precondition CanReachOxygenatedArea = new Chore.Precondition
	{
		id = "CanReachOxygenatedArea",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.chore.InProgress())
			{
				return true;
			}
			GameObject gameObject2 = (GameObject)data;
			if (gameObject2 == null)
			{
				return false;
			}
			Navigator navigator = context.consumerState.navigator;
			int num = ResuscitateSuffocatedChore.StatesInstance.FindClosestOxygenCellToIncapacitated(navigator, Grid.PosToCell(gameObject2));
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			int navigationCost2 = navigator.GetNavigationCost(num);
			if (-1 != navigationCost2)
			{
				context.cost += navigationCost2;
				return true;
			}
			return false;
		}
	};

	public class StatesInstance : GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.GameInstance
	{
		public StatesInstance(ResuscitateSuffocatedChore master)
			: base(master)
		{
		}

		public static int FindClosestOxygenCellToIncapacitated(Navigator navigator, int start_cell)
		{
			if (navigator == null)
			{
				return Grid.InvalidCell;
			}
			OxygenBreather component = navigator.GetComponent<OxygenBreather>();
			if (component == null)
			{
				global::Debug.Assert(false, "How is a non- oxygen breathing attempting to resuscitate?");
				return Grid.InvalidCell;
			}
			SafeResuscitateCellQuery safeResuscitateCellQuery = ResuscitateSuffocatedChore.StatesInstance.ResuscitateCellQuery.Reset(component);
			PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(start_cell, NavType.Floor, PathFinder.PotentialPath.Flags.None);
			PathFinder.Run(navigator.NavGrid, navigator.GetCurrentAbilities(), potentialPath, safeResuscitateCellQuery);
			return safeResuscitateCellQuery.GetResultCell();
		}

		public KBatchedAnimController resucerController;

		public KBatchedAnimController rescueeController;

		public static SafeResuscitateCellQuery ResuscitateCellQuery = new SafeResuscitateCellQuery();
	}

	public class States : GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approachSuffocated;
			this.root.Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				smi.sm.rescueTarget.Get(smi).Subscribe(1623392196, delegate(object d)
				{
					smi.GoTo(this.holding.ditch);
				});
			}).Exit(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				ResuscitateSuffocatedChore.States.SyncControllers(smi, false);
			});
			this.approachSuffocated.InitializeStates(this.rescuer, this.rescueTarget, this.holding.pickup, this.failure, Grid.DefaultOffset, null);
			this.holding.pickup.Target(this.rescuer).Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				this.rescuer.Get(smi).GetComponent<Storage>().Store(this.rescueTarget.Get(smi), false, false, true, false);
				this.rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
				KBatchedAnimTracker component = this.rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
				if (component != null)
				{
					component.symbol = new HashedString("snapTo_pivot");
					component.offset = new Vector3(0f, 0f, 1f);
				}
				ResuscitateSuffocatedChore.States.PlayResuscitateAnim(smi, "pickup", false);
			}).EventTransition(GameHashes.AnimQueueComplete, this.holding.delivering, null);
			this.holding.delivering.InitializeStates(this.rescuer, this.deliverTarget, this.resuscitate.pre, this.holding.ditch, null, null).Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				smi.rescueeController.Play("carry_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}).Update(delegate(ResuscitateSuffocatedChore.StatesInstance smi, float dt)
			{
				if (this.deliverTarget.Get(smi) == null)
				{
					smi.GoTo(this.holding.ditch);
				}
			}, UpdateRate.SIM_200ms, false);
			this.resuscitate.Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				GameObject gameObject = this.rescuer.Get(smi).gameObject;
				if (!gameObject.IsNullOrDestroyed())
				{
					KAnimFile anim = Assets.GetAnim("anim_resuscitate_kanim");
					gameObject.GetComponent<KAnimControllerBase>().AddAnimOverrides(anim, 0f);
					KAnimFile anim2 = Assets.GetAnim("anim_drowning_kanim");
					KAnimControllerBase component2 = smi.master.GetComponent<KAnimControllerBase>();
					component2.AddAnimOverrides(anim2, 0f);
					component2.gameObject.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
				}
			}).Exit(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				GameObject gameObject2 = this.rescuer.Get(smi).gameObject;
				if (!gameObject2.IsNullOrDestroyed())
				{
					KAnimFile anim3 = Assets.GetAnim("anim_resuscitate_kanim");
					gameObject2.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim3);
					KAnimFile anim4 = Assets.GetAnim("anim_drowning_kanim");
					smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim4);
				}
			});
			this.resuscitate.pre.Target(this.rescuer).Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				ResuscitateSuffocatedChore.States.SyncControllers(smi, true);
				ResuscitateSuffocatedChore.States.PlayResuscitateAnim(smi, "resuscitate_pre", true);
			}).OnAnimQueueComplete(this.resuscitate.loop);
			this.resuscitate.loop.Target(this.rescuer).Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				ResuscitateSuffocatedChore.States.PlayResuscitateAnim(smi, "resuscitate_loop", true);
			}).OnAnimQueueComplete(this.resuscitate.pst);
			this.resuscitate.pst.Target(this.rescuer).Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				ResuscitateSuffocatedChore.States.PlayResuscitateAnim(smi, "resuscitate_pst", true);
			}).OnAnimQueueComplete(this.success);
			this.holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, this.failure).Exit(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
			});
			this.failure.ReturnFailure();
			this.success.Enter(delegate(ResuscitateSuffocatedChore.StatesInstance smi)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Breath.Lookup(smi.gameObject);
				amountInstance.SetValue(amountInstance.GetMax());
				smi.Trigger(-1256572400, null);
				smi.GetSMI<IncapacitationMonitor.Instance>().ApplyRecoverEffect();
			}).ReturnSuccess();
		}

		public static void PlayResuscitateAnim(ResuscitateSuffocatedChore.StatesInstance smi, string anim_name, bool synced = false)
		{
			smi.resucerController.Play(anim_name, KAnim.PlayMode.Once, 1f, 0f);
			if (!synced)
			{
				smi.rescueeController.Play(anim_name, KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public static void SyncControllers(ResuscitateSuffocatedChore.StatesInstance smi, bool sync)
		{
			KAnimSynchronizer synchronizer = smi.resucerController.GetSynchronizer();
			if (synchronizer != null)
			{
				if (sync)
				{
					synchronizer.Add(smi.rescueeController, null);
					return;
				}
				if (smi.rescueeController.HasTag(GameTags.Dead))
				{
					synchronizer.RemoveWithoutIdleAnim(smi.rescueeController);
					return;
				}
				synchronizer.Remove(smi.rescueeController);
			}
		}

		public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.ApproachSubState<IApproachable> approachSuffocated;

		public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State failure;

		public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State success;

		public ResuscitateSuffocatedChore.States.HoldingSuffocated holding;

		public ResuscitateSuffocatedChore.States.Resuscitate resuscitate;

		public StateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.TargetParameter rescueTarget;

		public StateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.TargetParameter deliverTarget;

		public StateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.TargetParameter rescuer;

		public class HoldingSuffocated : GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State
		{
			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State pickup;

			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.ApproachSubState<IApproachable> delivering;

			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State ditch;
		}

		public class Resuscitate : GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State
		{
			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State pre;

			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State loop;

			public GameStateMachine<ResuscitateSuffocatedChore.States, ResuscitateSuffocatedChore.StatesInstance, ResuscitateSuffocatedChore, object>.State pst;
		}
	}
}
