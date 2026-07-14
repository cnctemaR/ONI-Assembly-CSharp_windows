using System;
using UnityEngine;

public class LitterBox : GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).DefaultState(this.operational.critterReady);
		this.operational.critterReady.EventTransition(GameHashes.OnStorageChange, this.operational.requiresEmptying, new StateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.Transition.ConditionCallback(LitterBox.RequiresEmptying));
		this.operational.requiresEmptying.Enter(new StateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State.Callback(LitterBox.CreateEmptyLitterBoxChore)).WorkableCompleteTransition(new Func<LitterBox.Instance, Workable>(LitterBox.GetWorkable), this.operational.empty).WorkableStopTransition(new Func<LitterBox.Instance, Workable>(LitterBox.GetWorkable), this.noOperational)
			.Exit(new StateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State.Callback(LitterBox.CancelEmptyLitterBoxChore));
		this.operational.empty.Enter(new StateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State.Callback(LitterBox.DropStorage)).EnterGoTo(this.operational.critterReady);
	}

	private static Workable GetWorkable(LitterBox.Instance smi)
	{
		return smi.GetWorkable();
	}

	private static bool RequiresEmptying(LitterBox.Instance smi)
	{
		return smi.IsFull;
	}

	private static void DropStorage(LitterBox.Instance smi)
	{
		smi.DropStorage();
	}

	private static void CreateEmptyLitterBoxChore(LitterBox.Instance smi)
	{
		smi.CreateWorkableChore();
	}

	private static void CancelEmptyLitterBoxChore(LitterBox.Instance smi)
	{
		smi.CancelWorkChore();
	}

	private static string[] POOP_INTERACT_ANIM_NAMES = new string[] { "working_pre", "working_loop", "working_pst" };

	public GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State noOperational;

	public LitterBox.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OperationalStates : GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State
	{
		public GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State critterReady;

		public GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State requiresEmptying;

		public GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.State empty;
	}

	public new class Instance : GameStateMachine<LitterBox, LitterBox.Instance, IStateMachineTarget, LitterBox.Def>.GameInstance, IPoopStation
	{
		public bool IsFull
		{
			get
			{
				return this.storage.RemainingCapacity() <= 0f;
			}
		}

		public bool IsCritterOperational
		{
			get
			{
				return this.operationalCmp.IsOperational && base.IsInsideState(base.sm.operational.critterReady);
			}
		}

		public Instance(IStateMachineTarget master, LitterBox.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.operationalCmp = base.GetComponent<Operational>();
			this.storage = base.GetComponent<Storage>();
			this.workable = base.GetComponent<EmptyLitterboxWorkable>();
		}

		public override void StartSM()
		{
			this.RegisterPoopStation();
			base.StartSM();
		}

		protected override void OnCleanUp()
		{
			this.UnregisterPoopStation();
		}

		public void DropStorage()
		{
			this.storage.DropAll(false, false, default(Vector3), true, null);
		}

		public Workable GetWorkable()
		{
			return this.workable;
		}

		public void CreateWorkableChore()
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<EmptyLitterboxWorkable>(Db.Get().ChoreTypes.CleanLitterBox, this.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelWorkChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("LitterBox.CancelChore");
				this.chore = null;
			}
		}

		public float GetPoopCapacity()
		{
			return this.storage.capacityKg;
		}

		public float GetAvailablePoopCapacityPercentage()
		{
			return this.storage.RemainingCapacity() / this.GetPoopCapacity();
		}

		public float GetAvailablePoopCapacity()
		{
			return this.storage.RemainingCapacity();
		}

		private bool CanAcceptMorePoop()
		{
			return this.GetAvailablePoopCapacity() > 0f;
		}

		public bool IsUserCompatibleWithPoopStation(KPrefabID userPrefabID)
		{
			return userPrefabID.HasTag(GameTags.Creatures.Walker);
		}

		public GameObject GetPoopStationObject()
		{
			return base.gameObject;
		}

		public GameObject GetCurrentPoopStationUser()
		{
			return this.poopUser;
		}

		public bool IsPoopStationOperational()
		{
			return this.IsCritterOperational && this.CanAcceptMorePoop();
		}

		public string[] GetPoopingAnimNames()
		{
			return LitterBox.POOP_INTERACT_ANIM_NAMES;
		}

		public void RegisterPoopStation()
		{
			Components.PoopStations.Add(base.gameObject.GetMyWorldId(), this);
		}

		public void UnregisterPoopStation()
		{
			Components.PoopStations.Remove(base.gameObject.GetMyWorldId(), this);
		}

		public PoopData GetPoopData()
		{
			return new PoopData(false, this.storage, null, null);
		}

		public void PlayPoopStationAnim(string animName, KAnim.PlayMode playMode)
		{
			this.animController.Play(animName, playMode, 1f, 0f);
		}

		public void ClearPoopStationUser(GameObject userRequestingClearing)
		{
			if (this.poopUser == userRequestingClearing)
			{
				this.poopUser = null;
				base.Trigger(-984476291, null);
			}
		}

		public bool AttemptToReservePoopStation(GameObject userRequestingReserve)
		{
			if (this.poopUser != null && this.poopUser != userRequestingReserve)
			{
				return false;
			}
			this.poopUser = userRequestingReserve;
			return true;
		}

		private KBatchedAnimController animController;

		private EmptyLitterboxWorkable workable;

		private Operational operationalCmp;

		private Storage storage;

		private Chore chore;

		private GameObject poopUser;
	}
}
