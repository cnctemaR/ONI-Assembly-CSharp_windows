using System;
using Klei;
using KSerialization;
using UnityEngine;

public class RemoteWorkerSM : StateMachineComponent<RemoteWorkerSM.StatesInstance>
{
	public bool Docked
	{
		get
		{
			return this.docked;
		}
		set
		{
			this.docked = value;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetNextChore(Chore.Precondition.Context next)
	{
		if (this.nextChore != null)
		{
			this.nextChore.Value.chore.Reserve(null);
		}
		this.nextChore = new Chore.Precondition.Context?(next);
		next.chore.Reserve(this.driver);
	}

	public void StartNextChore()
	{
		if (this.nextChore != null)
		{
			this.driver.SetChore(this.nextChore.Value);
			this.nextChore = null;
		}
	}

	public bool HasChoreQueued()
	{
		return this.nextChore != null;
	}

	public RemoteWorkerDock HomeDepot
	{
		get
		{
			Ref<RemoteWorkerDock> @ref = this.homeDepot;
			if (@ref == null)
			{
				return null;
			}
			return @ref.Get();
		}
		set
		{
			this.homeDepot = new Ref<RemoteWorkerDock>(value);
		}
	}

	public ChoreConsumerState ConsumerState
	{
		get
		{
			return this.consumer.consumerState;
		}
	}

	public bool ActivelyControlled { get; set; }

	public bool ActivelyWorking { get; set; }

	public bool Available { get; set; }

	public bool RequiresMaintnence
	{
		get
		{
			return this.power.IsLowPower;
		}
	}

	public void TickResources(float dt)
	{
		this.power.ApplyDeltaEnergy(-0.1f * dt);
		float num;
		SimUtil.DiseaseInfo diseaseInfo;
		float num2;
		this.storage.ConsumeAndGetDisease(GameTags.LubricatingOil, 0.033333335f * dt, out num, out diseaseInfo, out num2);
		if (num > 0f)
		{
			this.storage.AddElement(SimHashes.LiquidGunk, num, num2, diseaseInfo.idx, diseaseInfo.count, true, true);
		}
	}

	public GameObject FindStation()
	{
		if (Components.ComplexFabricators.Count == 0)
		{
			return null;
		}
		return Components.ComplexFabricators[0].gameObject;
	}

	public bool HasHomeDepot()
	{
		return !this.HomeDepot.IsNullOrDestroyed();
	}

	[MyCmpAdd]
	private RemoteWorkerCapacitor power;

	[MyCmpAdd]
	private RemoteWorkerGunkMonitor gunk;

	[MyCmpAdd]
	private RemoteWorkerOilMonitor oil;

	[MyCmpAdd]
	private ChoreDriver driver;

	[MyCmpGet]
	private ChoreConsumer consumer;

	[MyCmpGet]
	private Storage storage;

	public bool playNewWorker;

	[Serialize]
	private bool docked = true;

	private Chore.Precondition.Context? nextChore;

	private const string LostAnim_pre = "sos_pre";

	private const string LostAnim_loop = "sos_loop";

	private const string LostAnim_pst = "sos_pst";

	private const string DeathAnim = "explode";

	[Serialize]
	private Ref<RemoteWorkerDock> homeDepot;

	public class StatesInstance : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.GameInstance
	{
		public StatesInstance(RemoteWorkerSM master)
			: base(master)
		{
			base.sm.homedock.Set(base.smi.master.HomeDepot, base.smi);
		}
	}

	public class States : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.uncontrolled;
			this.controlled.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.Available = false;
			}).EnterTransition(this.controlled.exit_dock, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsInsideDock)).EnterTransition(this.controlled.working, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsInsideDock)))
				.Transition(this.uncontrolled, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasRemoteOperator)), UpdateRate.SIM_200ms)
				.Transition(this.incapacitated.lost, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.CanReachDepot)), UpdateRate.SIM_200ms)
				.Transition(this.incapacitated.die, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasHomeDepot)), UpdateRate.SIM_200ms)
				.Update(new Action<RemoteWorkerSM.StatesInstance, float>(RemoteWorkerSM.States.TickResources), UpdateRate.SIM_200ms, false);
			this.controlled.exit_dock.ToggleWork<RemoteWorkerDock.ExitableDock>(this.homedock, this.controlled.working, this.controlled.working, (RemoteWorkerSM.StatesInstance _) => true);
			this.controlled.working.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.ActivelyWorking = true;
			}).Exit(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.ActivelyWorking = false;
			}).DefaultState(this.controlled.working.find_work);
			this.controlled.working.find_work.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				if (RemoteWorkerSM.States.HasChore(smi))
				{
					smi.GoTo(this.controlled.working.do_work);
					return;
				}
				RemoteWorkerSM.States.SetNextChore(smi);
				smi.GoTo(RemoteWorkerSM.States.HasChore(smi) ? this.controlled.working.do_work : this.controlled.no_work);
			});
			this.controlled.working.do_work.Exit(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State.Callback(RemoteWorkerSM.States.ClearChore)).Transition(this.controlled.working.find_work, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasChore)), UpdateRate.SIM_200ms);
			this.controlled.no_work.Transition(this.controlled.working.do_work, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasChore), UpdateRate.SIM_200ms).Transition(this.controlled.working.find_work, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasChoreQueued), UpdateRate.SIM_200ms);
			this.uncontrolled.EnterTransition(this.uncontrolled.working.new_worker, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsNewWorker)).EnterTransition(this.uncontrolled.idle, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.And(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsInsideDock), GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsNewWorker)))).EnterTransition(this.uncontrolled.approach_dock, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.And(GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsInsideDock)), GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.IsNewWorker))))
				.Transition(this.controlled.working.find_work, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasRemoteOperator), UpdateRate.SIM_200ms)
				.Transition(this.incapacitated.lost, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.CanReachDepot)), UpdateRate.SIM_200ms)
				.Transition(this.incapacitated.die, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasHomeDepot)), UpdateRate.SIM_200ms);
			this.uncontrolled.approach_dock.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.Available = true;
			}).MoveTo<IApproachable>(this.homedock, this.uncontrolled.working.enter, this.incapacitated.lost, null, null);
			this.uncontrolled.working.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.Available = false;
			});
			this.uncontrolled.working.new_worker.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.playNewWorker = false;
			}).ToggleWork<RemoteWorkerDock.NewWorker>(this.homedock, this.uncontrolled.working.recharge, this.uncontrolled.working.recharge, (RemoteWorkerSM.StatesInstance _) => true);
			this.uncontrolled.working.enter.ToggleWork<RemoteWorkerDock.EnterableDock>(this.homedock, this.uncontrolled.working.recharge, this.uncontrolled.idle, (RemoteWorkerSM.StatesInstance _) => true);
			this.uncontrolled.working.recharge.ToggleWork<RemoteWorkerDock.WorkerRecharger>(this.homedock, this.uncontrolled.working.recharge_pst, this.uncontrolled.idle, (RemoteWorkerSM.StatesInstance _) => true);
			this.uncontrolled.working.recharge_pst.OnAnimQueueComplete(this.uncontrolled.working.drain_gunk).ScheduleGoTo(1f, this.uncontrolled.working.drain_gunk);
			this.uncontrolled.working.drain_gunk.ToggleWork<RemoteWorkerDock.WorkerGunkRemover>(this.homedock, this.uncontrolled.working.drain_gunk_pst, this.uncontrolled.idle, (RemoteWorkerSM.StatesInstance _) => true);
			this.uncontrolled.working.drain_gunk_pst.OnAnimQueueComplete(this.uncontrolled.working.fill_oil).ScheduleGoTo(1f, this.uncontrolled.working.fill_oil);
			this.uncontrolled.working.fill_oil.ToggleWork<RemoteWorkerDock.WorkerOilRefiller>(this.homedock, this.uncontrolled.working.fill_oil_pst, this.uncontrolled.idle, (RemoteWorkerSM.StatesInstance _) => true);
			this.uncontrolled.working.fill_oil_pst.OnAnimQueueComplete(this.uncontrolled.idle).ScheduleGoTo(1f, this.uncontrolled.idle);
			this.uncontrolled.idle.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.master.Available = true;
			}).PlayAnim(RemoteWorkerConfig.IDLE_IN_DOCK_ANIM, KAnim.PlayMode.Loop).Transition(this.uncontrolled.working.recharge, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.And(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.RequiresMaintnence), new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.DockIsOperational)), UpdateRate.SIM_1000ms);
			this.incapacitated.lost.Enter(delegate(RemoteWorkerSM.StatesInstance smi)
			{
				smi.Play("sos_pre", KAnim.PlayMode.Once);
				smi.Queue("sos_loop", KAnim.PlayMode.Loop);
				RemoteWorkerSM.States.ClearChore(smi);
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.UnreachableDock, null).Transition(this.incapacitated.lost_recovery, new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.CanReachDepot), UpdateRate.SIM_200ms)
				.Transition(this.incapacitated.die, GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Not(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.Transition.ConditionCallback(RemoteWorkerSM.States.HasHomeDepot)), UpdateRate.SIM_200ms);
			this.incapacitated.lost_recovery.PlayAnim("sos_pst").OnAnimQueueComplete(this.controlled);
			this.incapacitated.die.Enter(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State.Callback(RemoteWorkerSM.States.ClearChore)).PlayAnim("explode").OnAnimQueueComplete(this.incapacitated.explode)
				.ToggleStatusItem(Db.Get().DuplicantStatusItems.NoHomeDock, null);
			this.incapacitated.explode.Enter(new StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State.Callback(RemoteWorkerSM.States.Explode));
		}

		public static bool IsNewWorker(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.playNewWorker;
		}

		public static void SetNextChore(RemoteWorkerSM.StatesInstance smi)
		{
			smi.master.StartNextChore();
		}

		public static void ClearChore(RemoteWorkerSM.StatesInstance smi)
		{
			smi.master.driver.StopChore();
		}

		public static bool HasChore(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.driver.HasChore();
		}

		public static bool HasChoreQueued(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.HasChoreQueued();
		}

		public static bool CanReachDepot(RemoteWorkerSM.StatesInstance smi)
		{
			int depotCell = RemoteWorkerSM.States.GetDepotCell(smi);
			return depotCell != Grid.InvalidCell && smi.master.GetComponent<Navigator>().CanReach(depotCell);
		}

		public static int GetDepotCell(RemoteWorkerSM.StatesInstance smi)
		{
			RemoteWorkerDock homeDepot = smi.master.HomeDepot;
			if (homeDepot == null)
			{
				return Grid.InvalidCell;
			}
			return Grid.PosToCell(homeDepot);
		}

		public static bool HasRemoteOperator(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.ActivelyControlled;
		}

		public static bool RequiresMaintnence(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.RequiresMaintnence;
		}

		public static bool DockIsOperational(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.HomeDepot != null && smi.master.HomeDepot.IsOperational;
		}

		public static bool HasHomeDepot(RemoteWorkerSM.StatesInstance smi)
		{
			return RemoteWorkerSM.States.GetDepotCell(smi) != Grid.InvalidCell;
		}

		public static void StopWork(RemoteWorkerSM.StatesInstance smi)
		{
			if (smi.master.driver.HasChore())
			{
				smi.master.driver.StopChore();
			}
		}

		public static bool IsInsideDock(RemoteWorkerSM.StatesInstance smi)
		{
			return smi.master.Docked;
		}

		public static void Explode(RemoteWorkerSM.StatesInstance smi)
		{
			Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactDust, smi.master.transform.position, 0f);
			PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
			component.Element.substance.SpawnResource(Grid.CellToPosCCC(Grid.PosToCell(smi.master.gameObject), Grid.SceneLayer.Ore), 42f, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, false, false);
			Util.KDestroyGameObject(smi.master.gameObject);
		}

		public static void TickResources(RemoteWorkerSM.StatesInstance smi, float dt)
		{
			if (dt > 0f)
			{
				smi.master.TickResources(dt);
			}
		}

		public RemoteWorkerSM.States.ControlledStates controlled;

		public RemoteWorkerSM.States.UncontrolledStates uncontrolled;

		public RemoteWorkerSM.States.IncapacitatedStates incapacitated;

		public StateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.TargetParameter homedock;

		public class ControlledStates : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State
		{
			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State exit_dock;

			public RemoteWorkerSM.States.ControlledStates.WorkingStates working;

			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State no_work;

			public class WorkingStates : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State
			{
				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State find_work;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State do_work;
			}
		}

		public class UncontrolledStates : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State
		{
			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State approach_dock;

			public RemoteWorkerSM.States.UncontrolledStates.WorkingDockStates working;

			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State idle;

			public class WorkingDockStates : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State
			{
				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State new_worker;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State enter;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State recharge;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State recharge_pst;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State drain_gunk;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State drain_gunk_pst;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State fill_oil;

				public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State fill_oil_pst;
			}
		}

		public class IncapacitatedStates : GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State
		{
			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State lost;

			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State lost_recovery;

			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State die;

			public GameStateMachine<RemoteWorkerSM.States, RemoteWorkerSM.StatesInstance, RemoteWorkerSM, object>.State explode;
		}
	}
}
