using System;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

public class BionicOxygenTankMonitor : GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.fistSpawn;
		this.fistSpawn.ParamTransition<bool>(this.HasSpawnedBefore, this.safe, GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.IsTrue).Enter(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State.Callback(BionicOxygenTankMonitor.StartWithFullTank));
		this.safe.Transition(this.low, GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Not(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsSafe)), UpdateRate.SIM_200ms);
		this.low.DefaultState(this.low.idle);
		this.low.idle.Transition(this.critical, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsCritical), UpdateRate.SIM_200ms).Transition(this.safe, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsSafe), UpdateRate.SIM_200ms).ScheduleChange(this.low.schedule, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.IsAllowedToSeekOxygenBySchedule));
		this.low.schedule.ToggleUrge(Db.Get().Urges.FindOxygenRefill).DefaultState(this.low.schedule.enableSensors).Transition(this.critical, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsCriticalAndNotConsumingOxygen), UpdateRate.SIM_200ms)
			.Exit(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State.Callback(BionicOxygenTankMonitor.DisableOxygenSourceSensors));
		this.low.schedule.enableSensors.Enter(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State.Callback(BionicOxygenTankMonitor.EnableOxygenSourceSensors)).GoTo(this.low.schedule.oxygenCanisterMode);
		this.low.schedule.oxygenCanisterMode.DefaultState(this.low.schedule.oxygenCanisterMode.running);
		this.low.schedule.oxygenCanisterMode.running.ScheduleChange(this.low.idle, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.IsNotAllowedToSeekOxygenSourceItemsByScheduleAndSeekChoreHasNotBegun)).OnSignal(this.OxygenSourceItemLostSignal, this.low.schedule.environmentAbsorbMode, new Func<BionicOxygenTankMonitor.Instance, bool>(BionicOxygenTankMonitor.NoOxygenSourceAvailableButAbsorbCellAvailable)).OnSignal(this.AbsorbCellChangedSignal, this.low.schedule.environmentAbsorbMode, (BionicOxygenTankMonitor.Instance smi) => !BionicOxygenTankMonitor.FindOxygenSourceChoreIsRunning(smi) && BionicOxygenTankMonitor.NoOxygenSourceAvailableButAbsorbCellAvailable(smi))
			.Update(new Action<BionicOxygenTankMonitor.Instance, float>(BionicOxygenTankMonitor.UpdateAbsorbCellIfNoOxygenSourceAvailable), UpdateRate.SIM_200ms, false)
			.ToggleChore((BionicOxygenTankMonitor.Instance smi) => new FindAndConsumeOxygenSourceChore(smi.master, false), this.low.schedule.oxygenCanisterMode.ends, this.low.schedule.oxygenCanisterMode.ends);
		this.low.schedule.oxygenCanisterMode.ends.EnterTransition(this.safe, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsSafe)).GoTo(this.low.idle);
		this.low.schedule.environmentAbsorbMode.DefaultState(this.low.schedule.environmentAbsorbMode.running);
		this.low.schedule.environmentAbsorbMode.running.ScheduleChange(this.low.idle, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.IsNotAllowedToSeekOxygenSourceItemsByScheduleAndAbsorbChoreHasNotBegun)).OnSignal(this.ClosestOxygenSourceChanged, this.low.schedule.oxygenCanisterMode, new Func<BionicOxygenTankMonitor.Instance, bool>(BionicOxygenTankMonitor.OxygenSourceItemAvailableAndAbsorbChoreNotStarted)).ToggleChore((BionicOxygenTankMonitor.Instance smi) => new BionicMassOxygenAbsorbChore(smi.master, false), this.low.schedule.environmentAbsorbMode.ends, this.low.schedule.environmentAbsorbMode.ends);
		this.low.schedule.environmentAbsorbMode.ends.EnterTransition(this.safe, new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsSafe)).GoTo(this.low.idle);
		this.critical.ToggleUrge(Db.Get().Urges.FindOxygenRefill).Exit(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State.Callback(BionicOxygenTankMonitor.DisableOxygenSourceSensors)).DefaultState(this.critical.enableSensors)
			.ToggleExpression(Db.Get().Expressions.RecoverBreath, null)
			.Update(delegate(BionicOxygenTankMonitor.Instance smi, float dt)
			{
				if (smi.master.gameObject.GetAmounts().Get("Breath").value <= DUPLICANTSTATS.BIONICS.Breath.SUFFOCATE_AMOUNT)
				{
					smi.isRecoveringFromSuffocation = true;
				}
			}, UpdateRate.SIM_200ms, false)
			.Exit(delegate(BionicOxygenTankMonitor.Instance smi)
			{
				smi.isRecoveringFromSuffocation = false;
			});
		this.critical.enableSensors.Enter(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State.Callback(BionicOxygenTankMonitor.EnableOxygenSourceSensors)).GoTo(this.critical.oxygenCanisterMode);
		this.critical.oxygenCanisterMode.DefaultState(this.critical.oxygenCanisterMode.running);
		this.critical.oxygenCanisterMode.running.OnSignal(this.ClosestOxygenSourceChanged, this.critical.environmentAbsorbMode, (BionicOxygenTankMonitor.Instance smi) => !BionicOxygenTankMonitor.FindOxygenSourceChoreIsRunning(smi) && BionicOxygenTankMonitor.NoOxygenSourceAvailableButAbsorbCellAvailable(smi)).OnSignal(this.OxygenSourceItemLostSignal, this.critical.environmentAbsorbMode, new Func<BionicOxygenTankMonitor.Instance, bool>(BionicOxygenTankMonitor.NoOxygenSourceAvailableButAbsorbCellAvailable)).OnSignal(this.AbsorbCellChangedSignal, this.critical.environmentAbsorbMode, (BionicOxygenTankMonitor.Instance smi) => !BionicOxygenTankMonitor.FindOxygenSourceChoreIsRunning(smi) && BionicOxygenTankMonitor.NoOxygenSourceAvailableButAbsorbCellAvailable(smi))
			.Update(new Action<BionicOxygenTankMonitor.Instance, float>(BionicOxygenTankMonitor.UpdateAbsorbCellIfNoOxygenSourceAvailable), UpdateRate.SIM_200ms, false)
			.ToggleChore((BionicOxygenTankMonitor.Instance smi) => new FindAndConsumeOxygenSourceChore(smi.master, true), this.critical.oxygenCanisterMode.ends, this.critical.oxygenCanisterMode.ends);
		this.critical.oxygenCanisterMode.ends.EnterTransition(this.low, GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Not(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsCritical))).GoTo(this.critical.oxygenCanisterMode.running);
		this.critical.environmentAbsorbMode.DefaultState(this.critical.environmentAbsorbMode.running);
		this.critical.environmentAbsorbMode.running.OnSignal(this.ClosestOxygenSourceChanged, this.critical.oxygenCanisterMode, new Func<BionicOxygenTankMonitor.Instance, bool>(BionicOxygenTankMonitor.OxygenSourceItemAvailableAndAbsorbChoreNotStarted)).ToggleChore((BionicOxygenTankMonitor.Instance smi) => new BionicMassOxygenAbsorbChore(smi.master, true), this.critical.environmentAbsorbMode.ends, this.critical.environmentAbsorbMode.ends);
		this.critical.environmentAbsorbMode.ends.EnterTransition(this.low, GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Not(new StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Transition.ConditionCallback(BionicOxygenTankMonitor.AreOxygenLevelsCritical))).GoTo(this.critical.oxygenCanisterMode);
	}

	public static bool IsAllowedToSeekOxygenBySchedule(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.IsAllowedToSeekOxygenBySchedule;
	}

	public static bool IsNotAllowedToSeekOxygenSourceItemsByScheduleAndSeekChoreHasNotBegun(BionicOxygenTankMonitor.Instance smi)
	{
		return !BionicOxygenTankMonitor.IsAllowedToSeekOxygenBySchedule(smi) && !BionicOxygenTankMonitor.FindOxygenSourceChoreIsRunning(smi);
	}

	public static bool IsNotAllowedToSeekOxygenSourceItemsByScheduleAndAbsorbChoreHasNotBegun(BionicOxygenTankMonitor.Instance smi)
	{
		return !BionicOxygenTankMonitor.IsAllowedToSeekOxygenBySchedule(smi) && !BionicOxygenTankMonitor.AbsorbChoreIsRunning(smi);
	}

	public static bool AreOxygenLevelsSafe(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.OxygenPercentage >= 0.85f;
	}

	public static bool AreOxygenLevelsCritical(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.OxygenPercentage <= 0f;
	}

	public static bool AreOxygenLevelsCriticalAndNotConsumingOxygen(BionicOxygenTankMonitor.Instance smi)
	{
		return BionicOxygenTankMonitor.AreOxygenLevelsCritical(smi) && !BionicOxygenTankMonitor.IsConsumingOxygen(smi);
	}

	public static bool IsThereAnOxygenSourceItemAvailable(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.GetClosestOxygenSource() != null;
	}

	public static bool AbsorbCellUnavailable(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.AbsorbOxygenCell == Grid.InvalidCell;
	}

	public static bool AbsorbCellAvailable(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.AbsorbOxygenCell != Grid.InvalidCell;
	}

	public static bool NoOxygenSourceAvailable(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.GetClosestOxygenSource() == null;
	}

	public static bool NoOxygenSourceAvailableButAbsorbCellAvailable(BionicOxygenTankMonitor.Instance smi)
	{
		return BionicOxygenTankMonitor.NoOxygenSourceAvailable(smi) && BionicOxygenTankMonitor.AbsorbCellAvailable(smi);
	}

	public static bool OxygenSourceItemAvailableAndAbsorbChoreNotStarted(BionicOxygenTankMonitor.Instance smi)
	{
		return BionicOxygenTankMonitor.IsThereAnOxygenSourceItemAvailable(smi) && !BionicOxygenTankMonitor.AbsorbChoreIsRunning(smi);
	}

	public static bool AbsorbChoreIsRunning(BionicOxygenTankMonitor.Instance smi)
	{
		return BionicOxygenTankMonitor.ChoreIsRunning(smi, Db.Get().ChoreTypes.BionicAbsorbOxygen) || BionicOxygenTankMonitor.ChoreIsRunning(smi, Db.Get().ChoreTypes.BionicAbsorbOxygen_Critical);
	}

	public static bool FindOxygenSourceChoreIsRunning(BionicOxygenTankMonitor.Instance smi)
	{
		return BionicOxygenTankMonitor.ChoreIsRunning(smi, Db.Get().ChoreTypes.FindOxygenSourceItem) || BionicOxygenTankMonitor.ChoreIsRunning(smi, Db.Get().ChoreTypes.FindOxygenSourceItem_Critical);
	}

	public static bool ChoreIsRunning(BionicOxygenTankMonitor.Instance smi, ChoreType type)
	{
		return smi.ChoreIsRunning(type);
	}

	public static bool IsConsumingOxygen(BionicOxygenTankMonitor.Instance smi)
	{
		return smi.IsConsumingOxygen();
	}

	public static void StartWithFullTank(BionicOxygenTankMonitor.Instance smi)
	{
		smi.AddFirstTimeSpawnedOxygen();
	}

	public static void EnableOxygenSourceSensors(BionicOxygenTankMonitor.Instance smi)
	{
		smi.SetOxygenSourceSensorsActiveState(true);
	}

	public static void DisableOxygenSourceSensors(BionicOxygenTankMonitor.Instance smi)
	{
		smi.SetOxygenSourceSensorsActiveState(false);
	}

	public static void UpdateAbsorbCellIfNoOxygenSourceAvailable(BionicOxygenTankMonitor.Instance smi, float dt)
	{
		if (BionicOxygenTankMonitor.NoOxygenSourceAvailable(smi))
		{
			smi.UpdatePotentialCellToAbsorbOxygen(Grid.InvalidCell);
		}
	}

	public const SimHashes INITIAL_TANK_ELEMENT = SimHashes.Oxygen;

	public static readonly Tag INITIAL_TANK_ELEMENT_TAG = SimHashes.Oxygen.CreateTag();

	public const float SAFE_TRESHOLD = 0.85f;

	public const float CRITICAL_TRESHOLD = 0f;

	public const float OXYGEN_TANK_CAPACITY_IN_SECONDS = 2400f;

	public static readonly float OXYGEN_TANK_CAPACITY_KG = 2400f * DUPLICANTSTATS.BIONICS.BaseStats.OXYGEN_USED_PER_SECOND;

	public static float INITIAL_OXYGEN_TEMP = DUPLICANTSTATS.BIONICS.Temperature.Internal.IDEAL;

	public static float SECONDS_PER_PATH_COST_UNIT = 0.3f;

	public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State fistSpawn;

	public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State safe;

	public BionicOxygenTankMonitor.LowOxygenStates low;

	public BionicOxygenTankMonitor.SeekOxygenStates critical;

	private StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.BoolParameter HasSpawnedBefore;

	public StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Signal AbsorbCellChangedSignal;

	public StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Signal OxygenSourceItemLostSignal;

	public StateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.Signal ClosestOxygenSourceChanged;

	public interface IChore
	{
		bool IsConsumingOxygen();
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public class ChoreState : GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State
	{
		public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State running;

		public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State ends;
	}

	public class SeekOxygenStates : GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State
	{
		public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State enableSensors;

		public BionicOxygenTankMonitor.ChoreState oxygenCanisterMode;

		public BionicOxygenTankMonitor.ChoreState environmentAbsorbMode;
	}

	public class LowOxygenStates : GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State
	{
		public GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.State idle;

		public BionicOxygenTankMonitor.SeekOxygenStates schedule;
	}

	public new class Instance : GameStateMachine<BionicOxygenTankMonitor, BionicOxygenTankMonitor.Instance, IStateMachineTarget, BionicOxygenTankMonitor.Def>.GameInstance, OxygenBreather.IGasProvider
	{
		public bool IsAllowedToSeekOxygenBySchedule
		{
			get
			{
				return ScheduleManager.Instance.IsAllowed(this.schedulable, Db.Get().ScheduleBlockTypes.Eat);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.AvailableOxygen == 0f;
			}
		}

		public float OxygenPercentage
		{
			get
			{
				return this.AvailableOxygen / this.storage.capacityKg;
			}
		}

		public float AvailableOxygen
		{
			get
			{
				return this.storage.GetMassAvailable(GameTags.Breathable);
			}
		}

		public float SpaceAvailableInTank
		{
			get
			{
				return this.storage.capacityKg - this.AvailableOxygen;
			}
		}

		public int AbsorbOxygenCell { get; private set; } = Grid.InvalidCell;

		public Storage storage { get; private set; }

		public Instance(IStateMachineTarget master, BionicOxygenTankMonitor.Def def)
			: base(master, def)
		{
			this.query = new AbsorbCellQuery();
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
			Sensors component = base.GetComponent<Sensors>();
			this.schedulable = base.GetComponent<Schedulable>();
			this.navigator = base.GetComponent<Navigator>();
			float movementSpeedMultiplier = BipedTransitionLayer.GetMovementSpeedMultiplier(Db.Get().AttributeConverters.MovementSpeed.Lookup(this.navigator.gameObject));
			this.movementRate = movementSpeedMultiplier / BionicOxygenTankMonitor.SECONDS_PER_PATH_COST_UNIT;
			this.oxygenBreather = base.GetComponent<OxygenBreather>();
			this.brain = base.GetComponent<MinionBrain>();
			this.dataHolder = base.GetComponent<MinionStorageDataHolder>();
			MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
			minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Combine(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			this.oxygenSourceSensors = new ClosestPickupableSensor<Pickupable>[] { component.GetSensor<ClosestOxygenCanisterSensor>() };
			for (int i = 0; i < this.oxygenSourceSensors.Length; i++)
			{
				ClosestPickupableSensor<Pickupable> closestPickupableSensor = this.oxygenSourceSensors[i];
				closestPickupableSensor.OnItemChanged = (Action<Pickupable>)Delegate.Combine(closestPickupableSensor.OnItemChanged, new Action<Pickupable>(this.OnOxygenSourceSensorItemChanged));
			}
			this.storage = base.gameObject.GetComponents<Storage>().FindFirst<Storage>((Storage s) => s.storageID == GameTags.StoragesIds.BionicOxygenTankStorage);
			this.oxygenTankAmountInstance = Db.Get().Amounts.BionicOxygenTank.Lookup(base.gameObject);
			this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(base.gameObject);
			Storage storage = this.storage;
			storage.OnStorageChange = (Action<GameObject>)Delegate.Combine(storage.OnStorageChange, new Action<GameObject>(this.OnOxygenTankStorageChanged));
			this.choreDriver = base.gameObject.GetComponent<ChoreDriver>();
		}

		public bool ChoreIsRunning(ChoreType type)
		{
			if (this.choreDriver == null)
			{
				return false;
			}
			Chore currentChore = this.choreDriver.GetCurrentChore();
			return currentChore != null && currentChore.choreType == type;
		}

		public bool IsConsumingOxygen()
		{
			this.choreDriver = base.smi.GetComponent<ChoreDriver>();
			if (this.choreDriver == null)
			{
				return false;
			}
			BionicOxygenTankMonitor.IChore chore = this.choreDriver.GetCurrentChore() as BionicOxygenTankMonitor.IChore;
			return chore != null && chore.IsConsumingOxygen();
		}

		public Pickupable GetClosestOxygenSource()
		{
			return this.closestOxygenSource;
		}

		private void OnOxygenSourceSensorItemChanged(object o)
		{
			this.CompareOxygenSources();
		}

		private void OnOxygenTankStorageChanged(object o)
		{
			this.RefreshAmountInstance();
		}

		public void RefreshAmountInstance()
		{
			this.oxygenTankAmountInstance.SetValue(this.AvailableOxygen);
		}

		public void AddFirstTimeSpawnedOxygen()
		{
			this.storage.AddElement(SimHashes.Oxygen, this.storage.capacityKg - this.AvailableOxygen, BionicOxygenTankMonitor.INITIAL_OXYGEN_TEMP, byte.MaxValue, 0, false, true);
			base.sm.HasSpawnedBefore.Set(true, this, false);
		}

		private void OnCopyMinionBegins(StoredMinionIdentity destination)
		{
			MinionStorageDataHolder.DataPackData dataPackData = new MinionStorageDataHolder.DataPackData
			{
				Bools = new bool[] { base.sm.HasSpawnedBefore.Get(this) }
			};
			this.dataHolder.UpdateData<BionicOxygenTankMonitor.Instance>(dataPackData);
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RefreshAmountInstance();
		}

		public override void PostParamsInitialized()
		{
			MinionStorageDataHolder.DataPack dataPack = this.dataHolder.GetDataPack<BionicOxygenTankMonitor.Instance>();
			if (dataPack != null && dataPack.IsStoringNewData)
			{
				MinionStorageDataHolder.DataPackData dataPackData = dataPack.ReadData();
				if (dataPackData != null)
				{
					bool flag = dataPackData.Bools[0];
					base.sm.HasSpawnedBefore.Set(flag, this, false);
				}
			}
			base.PostParamsInitialized();
		}

		private void CompareOxygenSources()
		{
			Pickupable pickupable = null;
			float num = 2.1474836E+09f;
			for (int i = 0; i < this.oxygenSourceSensors.Length; i++)
			{
				ClosestPickupableSensor<Pickupable> closestPickupableSensor = this.oxygenSourceSensors[i];
				int itemNavCost = closestPickupableSensor.GetItemNavCost();
				if ((float)itemNavCost < num)
				{
					num = (float)itemNavCost;
					pickupable = closestPickupableSensor.GetItem();
				}
			}
			if (pickupable != null && base.IsInsideState(base.sm.critical))
			{
				float num2 = num / this.movementRate * this.oxygenBreather.ConsumptionRate;
				if (this.oxygenBreather.GetAmounts().Get(Db.Get().Amounts.Breath).value < num2)
				{
					pickupable = null;
				}
			}
			if (this.closestOxygenSource != pickupable)
			{
				this.closestOxygenSource = pickupable;
				base.sm.ClosestOxygenSourceChanged.Trigger(this);
			}
		}

		public void UpdatePotentialCellToAbsorbOxygen(int previouslyReservedCell)
		{
			float num = this.brain.GetAmounts().Get(Db.Get().Amounts.Breath).value / this.brain.GetAmounts().Get(Db.Get().Amounts.Breath).GetMax();
			this.query.Reset(this.brain, BionicOxygenTankMonitor.AreOxygenLevelsCritical(this), this.AvailableOxygen, num, previouslyReservedCell, this.isRecoveringFromSuffocation);
			this.navigator.RunQuery(base.smi.query);
			int num2 = base.smi.query.GetResultCell();
			if (num2 == Grid.PosToCell(base.gameObject) && !GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(num2, GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS, this.oxygenBreather).IsBreathable)
			{
				num2 = PathFinder.InvalidCell;
			}
			bool flag = this.AbsorbOxygenCell != num2;
			this.AbsorbOxygenCell = num2;
			if (flag)
			{
				base.sm.AbsorbCellChangedSignal.Trigger(this);
			}
		}

		public float AddGas(Sim.MassConsumedCallback mass_cb_info)
		{
			return this.AddGas(ElementLoader.elements[(int)mass_cb_info.elemIdx].id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
		}

		public float AddGas(SimHashes element, float mass, float temperature, byte disseaseIDX = 255, int _disseaseCount = 0)
		{
			float num = Mathf.Min(mass, this.SpaceAvailableInTank);
			float num2 = mass - num;
			float num3 = num / mass;
			int num4 = Mathf.CeilToInt((float)_disseaseCount * num3);
			this.storage.AddElement(element, num, temperature, disseaseIDX, num4, false, true);
			return num2;
		}

		public void SetOxygenSourceSensorsActiveState(bool shouldItBeActive)
		{
			for (int i = 0; i < this.oxygenSourceSensors.Length; i++)
			{
				ClosestPickupableSensor<Pickupable> closestPickupableSensor = this.oxygenSourceSensors[i];
				closestPickupableSensor.SetActive(shouldItBeActive);
				if (shouldItBeActive)
				{
					closestPickupableSensor.Update();
				}
			}
		}

		public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
		{
			if (this.IsEmpty)
			{
				return false;
			}
			SimHashes simHashes = SimHashes.Vacuum;
			float num = 0f;
			float num2;
			SimUtil.DiseaseInfo diseaseInfo;
			this.storage.ConsumeAndGetDisease(GameTags.Breathable, amount, out num2, out diseaseInfo, out num, out simHashes);
			OxygenBreather.BreathableGasConsumed(oxygen_breather, simHashes, amount, num, diseaseInfo.idx, diseaseInfo.count);
			return true;
		}

		public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public bool IsLowOxygen()
		{
			return this.OxygenPercentage <= 0f;
		}

		public bool HasOxygen()
		{
			return !this.IsEmpty;
		}

		public bool IsBlocked()
		{
			return false;
		}

		public bool ShouldEmitCO2()
		{
			return false;
		}

		public bool ShouldStoreCO2()
		{
			return false;
		}

		protected override void OnCleanUp()
		{
			if (this.dataHolder != null)
			{
				MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
				minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Remove(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			}
			if (this.storage != null)
			{
				Storage storage = this.storage;
				storage.OnStorageChange = (Action<GameObject>)Delegate.Remove(storage.OnStorageChange, new Action<GameObject>(this.OnOxygenTankStorageChanged));
			}
			base.OnCleanUp();
		}

		public AttributeInstance airConsumptionRate;

		private Schedulable schedulable;

		private AmountInstance oxygenTankAmountInstance;

		private ClosestPickupableSensor<Pickupable>[] oxygenSourceSensors;

		private Pickupable closestOxygenSource;

		private Navigator navigator;

		private float movementRate;

		private AbsorbCellQuery query;

		private OxygenBreather oxygenBreather;

		private MinionBrain brain;

		private MinionStorageDataHolder dataHolder;

		private ChoreDriver choreDriver;

		public bool isRecoveringFromSuffocation;
	}
}
