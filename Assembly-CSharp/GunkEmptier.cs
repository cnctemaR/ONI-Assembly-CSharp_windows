using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GunkEmptier : GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.noOperational.EventTransition(GameHashes.OperationalChanged, this.operational, new StateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Transition.ConditionCallback(GunkEmptier.IsOperational));
		this.operational.EventTransition(GameHashes.OperationalChanged, this.noOperational, GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Not(new StateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Transition.ConditionCallback(GunkEmptier.IsOperational))).DefaultState(this.operational.noStorageSpace);
		this.operational.noStorageSpace.ToggleStatusItem(Db.Get().BuildingStatusItems.GunkEmptierFull, null).EventTransition(GameHashes.OnStorageChange, this.operational.ready, new StateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Transition.ConditionCallback(GunkEmptier.HasSpaceToEmptyABionicGunkTank));
		this.operational.ready.EventTransition(GameHashes.OnStorageChange, this.operational.noStorageSpace, GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Not(new StateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.Transition.ConditionCallback(GunkEmptier.HasSpaceToEmptyABionicGunkTank))).ToggleRecurringChore(new Func<GunkEmptier.Instance, Chore>(GunkEmptier.CreateChore), null);
	}

	public static bool HasSpaceToEmptyABionicGunkTank(GunkEmptier.Instance smi)
	{
		return smi.RemainingStorageCapacity >= GunkMonitor.GUNK_CAPACITY;
	}

	public static bool IsOperational(GunkEmptier.Instance smi)
	{
		return smi.IsOperational;
	}

	private static WorkChore<GunkEmptierWorkable> CreateChore(GunkEmptier.Instance smi)
	{
		WorkChore<GunkEmptierWorkable> workChore = new WorkChore<GunkEmptierWorkable>(Db.Get().ChoreTypes.ExpellGunk, smi.master, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
		workChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, smi.master.GetComponent<Assignable>());
		return workChore;
	}

	private static string DISEASE_ID = DUPLICANTSTATS.BIONICS.Secretions.PEE_DISEASE;

	private static int DISEASE_ON_DUPE_COUNT_PER_USE = DUPLICANTSTATS.BIONICS.Secretions.DISEASE_PER_PEE / 20;

	public GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.State noOperational;

	public GunkEmptier.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OperationalStates : GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.State
	{
		public GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.State noStorageSpace;

		public GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.State ready;
	}

	public new class Instance : GameStateMachine<GunkEmptier, GunkEmptier.Instance, IStateMachineTarget, GunkEmptier.Def>.GameInstance
	{
		public float RemainingStorageCapacity
		{
			get
			{
				return this.storage.RemainingCapacity();
			}
		}

		public bool IsOperational
		{
			get
			{
				return this.operational.IsOperational;
			}
		}

		public Instance(IStateMachineTarget master, GunkEmptier.Def def)
			: base(master, def)
		{
			GunkEmptierWorkable component = base.GetComponent<GunkEmptierWorkable>();
			GunkEmptierWorkable gunkEmptierWorkable = component;
			gunkEmptierWorkable.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(gunkEmptierWorkable.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnGunkEmptierUsed));
			Components.GunkExtractors.Add(component);
			this.storage = base.GetComponent<Storage>();
			this.operational = base.GetComponent<Operational>();
			base.gameObject.AddOrGet<Ownable>().AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.AssignablePrecondition_OnlyOnBionics));
		}

		protected override void OnCleanUp()
		{
			GunkEmptierWorkable component = base.GetComponent<GunkEmptierWorkable>();
			GunkEmptierWorkable gunkEmptierWorkable = component;
			gunkEmptierWorkable.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Remove(gunkEmptierWorkable.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnGunkEmptierUsed));
			Components.GunkExtractors.Remove(component);
			base.OnCleanUp();
		}

		private bool AssignablePrecondition_OnlyOnBionics(MinionAssignablesProxy worker)
		{
			return worker.GetMinionModel() == BionicMinionConfig.MODEL;
		}

		public void OnGunkEmptierUsed(Workable workable, Workable.WorkableEvent ev)
		{
			if (ev == Workable.WorkableEvent.WorkCompleted)
			{
				this.AddDisseaseToWorker(workable.worker);
			}
		}

		public void AddDisseaseToWorker(WorkerBase worker)
		{
			if (worker != null)
			{
				byte index = Db.Get().Diseases.GetIndex(GunkEmptier.DISEASE_ID);
				worker.GetComponent<PrimaryElement>().AddDisease(index, GunkEmptier.DISEASE_ON_DUPE_COUNT_PER_USE, "GunkEmptier.Flush");
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[(int)index].Name, GunkEmptier.DISEASE_ON_DUPE_COUNT_PER_USE), base.transform, Vector3.up, 1.5f, false, false);
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms, true);
				return;
			}
			DebugUtil.LogWarningArgs(new object[] { "Tried to add disease on gunk emptier use but worker was null" });
		}

		private Operational operational;

		private Storage storage;
	}
}
