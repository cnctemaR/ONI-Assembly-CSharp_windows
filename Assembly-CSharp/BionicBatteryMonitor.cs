using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using UnityEngine;

public class BionicBatteryMonitor : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.firstSpawn;
		this.firstSpawn.ParamTransition<bool>(this.InitialElectrobanksSpawned, this.online, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IsTrue).Enter(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.SpawnAndInstallInitialElectrobanks));
		this.online.TriggerOnEnter(GameHashes.BionicOnline, null).Transition(this.offline, new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.DoesNotHaveCharge), UpdateRate.SIM_200ms).Enter(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.ReorganizeElectrobankStorage))
			.Update(new Action<BionicBatteryMonitor.Instance, float>(BionicBatteryMonitor.DischargeUpdate), UpdateRate.SIM_200ms, false)
			.DefaultState(this.online.idle);
		this.online.idle.ParamTransition<int>(this.ChargedElectrobankCount, this.online.critical, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IsLTEOne_Int).OnSignal(this.OnElectrobankStorageChanged, this.online.upkeep, new Func<BionicBatteryMonitor.Instance, bool>(BionicBatteryMonitor.WantsToUpkeep)).EventTransition(GameHashes.ScheduleBlocksChanged, this.online.upkeep, new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep))
			.EventTransition(GameHashes.ScheduleChanged, this.online.upkeep, new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep))
			.EventTransition(GameHashes.ScheduleBlocksTick, this.online.upkeep, new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep));
		this.online.upkeep.ParamTransition<int>(this.ChargedElectrobankCount, this.online.critical, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IsLTEOne_Int).EventTransition(GameHashes.ScheduleBlocksChanged, this.online.idle, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Not(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep))).EventTransition(GameHashes.ScheduleChanged, this.online.idle, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Not(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep)))
			.EventTransition(GameHashes.ScheduleBlocksTick, this.online.idle, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Not(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Transition.ConditionCallback(BionicBatteryMonitor.WantsToUpkeep)))
			.OnSignal(this.OnElectrobankStorageChanged, this.online.idle, (BionicBatteryMonitor.Instance smi) => !BionicBatteryMonitor.WantsToUpkeep(smi))
			.DefaultState(this.online.upkeep.seekElectrobank);
		this.online.upkeep.seekElectrobank.ToggleUrge(Db.Get().Urges.ReloadElectrobank).ToggleChore((BionicBatteryMonitor.Instance smi) => new ReloadElectrobankChore(smi.master), this.online.idle);
		this.online.critical.DefaultState(this.online.critical.seekElectrobank).ParamTransition<int>(this.ChargedElectrobankCount, this.online.idle, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IsGTOne_Int).DoTutorial(Tutorial.TutorialMessages.TM_BionicBattery);
		this.online.critical.seekElectrobank.ToggleUrge(Db.Get().Urges.ReloadElectrobank).ToggleRecurringChore((BionicBatteryMonitor.Instance smi) => new ReloadElectrobankChore(smi.master), null);
		this.offline.DefaultState(this.offline.waitingForBatteryDelivery).ToggleTag(GameTags.Incapacitated).ToggleRecurringChore((BionicBatteryMonitor.Instance smi) => new BeOfflineChore(smi.master), null)
			.ToggleUrge(Db.Get().Urges.BeOffline)
			.Enter(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.SetOffline))
			.Enter(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.DropAllDischargedElectrobanks))
			.TriggerOnEnter(GameHashes.BionicOffline, null);
		this.offline.waitingForBatteryDelivery.ParamTransition<int>(this.ChargedElectrobankCount, this.offline.waitingForBatteryInstallation, GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IsGTZero_Int).Toggle("Enable Delivery of new Electrobanks", new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.EnableManualDelivery), new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.DisableManualDelivery)).Toggle("Enable User Prioritization", new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.EnablePrioritizationComponent), new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.DisablePrioritizationComponent));
		this.offline.waitingForBatteryInstallation.Toggle("Enable User Prioritization", new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.EnablePrioritizationComponent), new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.DisablePrioritizationComponent)).Enter(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.StartReanimateWorkChore)).Exit(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.CancelReanimateWorkChore))
			.WorkableCompleteTransition(new Func<BionicBatteryMonitor.Instance, Workable>(BionicBatteryMonitor.GetReanimateWorkable), this.offline.reboot)
			.DefaultState(this.offline.waitingForBatteryInstallation.waiting);
		this.offline.waitingForBatteryInstallation.waiting.ToggleStatusItem(Db.Get().DuplicantStatusItems.BionicWaitingForReboot, null).WorkableStartTransition(new Func<BionicBatteryMonitor.Instance, Workable>(BionicBatteryMonitor.GetReanimateWorkable), this.offline.waitingForBatteryInstallation.working);
		this.offline.waitingForBatteryInstallation.working.WorkableStopTransition(new Func<BionicBatteryMonitor.Instance, Workable>(BionicBatteryMonitor.GetReanimateWorkable), this.offline.waitingForBatteryInstallation.waiting);
		this.offline.reboot.PlayAnim("power_up").OnAnimQueueComplete(this.online).ScheduleGoTo(10f, this.online)
			.Exit(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.AutomaticallyDropAllDepletedElectrobanks))
			.Exit(new StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State.Callback(BionicBatteryMonitor.SetOnline));
	}

	public static ReanimateBionicWorkable GetReanimateWorkable(BionicBatteryMonitor.Instance smi)
	{
		return smi.reanimateWorkable;
	}

	public static bool DoesNotHaveCharge(BionicBatteryMonitor.Instance smi)
	{
		return smi.CurrentCharge <= 0f;
	}

	public static bool IsCriticallyLow(BionicBatteryMonitor.Instance smi)
	{
		return smi.ChargedElectrobankCount <= 1;
	}

	public static bool ChargeIsBelowNotificationThreshold(BionicBatteryMonitor.Instance smi)
	{
		return smi.CurrentCharge <= 30000f;
	}

	public static bool IsAnyElectrobankAvailableToBeFetched(BionicBatteryMonitor.Instance smi)
	{
		return smi.GetClosestElectrobank() != null;
	}

	public static bool WantsToInstallNewBattery(BionicBatteryMonitor.Instance smi)
	{
		return BionicBatteryMonitor.IsCriticallyLow(smi) || (smi.InUpkeepTime && smi.ChargedElectrobankCount < smi.ElectrobankCountCapacity);
	}

	public static bool WantsToUpkeep(BionicBatteryMonitor.Instance smi)
	{
		return BionicBatteryMonitor.WantsToInstallNewBattery(smi);
	}

	public static void SpawnAndInstallInitialElectrobanks(BionicBatteryMonitor.Instance smi)
	{
		smi.SpawnAndInstallInitialElectrobanks();
	}

	public static void RefreshCharge(BionicBatteryMonitor.Instance smi)
	{
		smi.RefreshCharge();
	}

	public static void EnableManualDelivery(BionicBatteryMonitor.Instance smi)
	{
		smi.SetManualDeliveryEnableState(true);
	}

	public static void DisableManualDelivery(BionicBatteryMonitor.Instance smi)
	{
		smi.SetManualDeliveryEnableState(false);
	}

	public static void StartReanimateWorkChore(BionicBatteryMonitor.Instance smi)
	{
		smi.CreateWorkableChore();
	}

	public static void CancelReanimateWorkChore(BionicBatteryMonitor.Instance smi)
	{
		smi.CancelWorkChore();
	}

	public static void SetOffline(BionicBatteryMonitor.Instance smi)
	{
		smi.SetOnlineState(false);
	}

	public static void SetOnline(BionicBatteryMonitor.Instance smi)
	{
		smi.SetOnlineState(true);
	}

	public static void AutomaticallyDropAllDepletedElectrobanks(BionicBatteryMonitor.Instance smi)
	{
		smi.AutomaticallyDropAllDepletedElectrobanks();
	}

	public static void ReorganizeElectrobankStorage(BionicBatteryMonitor.Instance smi)
	{
		smi.ReorganizeElectrobanks();
	}

	public static void DropAllDischargedElectrobanks(BionicBatteryMonitor.Instance smi)
	{
		smi.DropAllDischargedElectrobanks();
	}

	public static void EnablePrioritizationComponent(BionicBatteryMonitor.Instance smi)
	{
		Prioritizable.AddRef(smi.gameObject);
		smi.gameObject.Trigger(1980521255, null);
	}

	public static void DisablePrioritizationComponent(BionicBatteryMonitor.Instance smi)
	{
		Prioritizable.RemoveRef(smi.gameObject);
		smi.gameObject.Trigger(1980521255, null);
	}

	public static void DischargeUpdate(BionicBatteryMonitor.Instance smi, float dt)
	{
		float num = Mathf.Min(dt * smi.Wattage, smi.CurrentCharge);
		smi.ConsumePower(num);
	}

	private static BionicBatteryMonitor.WattageModifier MakeDifficultyModifier(string id, string desc, float watts)
	{
		return new BionicBatteryMonitor.WattageModifier(id, desc + ": <b>" + ((watts >= 0f) ? "+</b>" : "-</b>") + GameUtil.GetFormattedWattage(Mathf.Abs(watts), GameUtil.WattageFormatterUnit.Automatic, true), watts, watts);
	}

	public static BionicBatteryMonitor.WattageModifier GetDifficultyModifier()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.BionicWattage);
		BionicBatteryMonitor.WattageModifier wattageModifier;
		if (BionicBatteryMonitor.difficultyWattages.TryGetValue(currentQualitySetting.id, out wattageModifier))
		{
			return wattageModifier;
		}
		return BionicBatteryMonitor.difficultyWattages["Default"];
	}

	public const int DEFAULT_ELECTROBANK_COUNT = 4;

	public const int BIONIC_SKILL_EXTRA_BATTERY_COUNT = 2;

	public const int MAX_ELECTROBANK_COUNT = 6;

	public const float DEFAULT_WATTS = 200f;

	public const string INITIAL_ELECTROBANK_TYPE_ID = "DisposableElectrobank_RawMetal";

	public static readonly string ChargedBatteryIcon = "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_charged_electrobank\">";

	public static readonly string DischargedBatteryIcon = "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_discharged_electrobank\">";

	public static readonly string CriticalBatteryIcon = "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_critical_electrobank\">";

	public static readonly string SavingBatteryIcon = "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_saving_electrobank\">";

	public static readonly string EmptySlotBatteryIcon = "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_empty_slot_electrobank\">";

	private const string ANIM_NAME_REBOOT = "power_up";

	public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State firstSpawn;

	public BionicBatteryMonitor.OnlineStates online;

	public BionicBatteryMonitor.OfflineStates offline;

	public StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Signal OnClosestAvailableElectrobankChangedSignal;

	public StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IntParameter ChargedElectrobankCount;

	public StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.IntParameter DepletedElectrobankCount;

	private StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.BoolParameter InitialElectrobanksSpawned;

	private StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.BoolParameter IsOnline;

	private StateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.Signal OnElectrobankStorageChanged;

	private static readonly Dictionary<string, BionicBatteryMonitor.WattageModifier> difficultyWattages = new Dictionary<string, BionicBatteryMonitor.WattageModifier>
	{
		{
			"VeryHard",
			BionicBatteryMonitor.MakeDifficultyModifier("difficultyWattage", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.BIONICPOWERUSE.LEVELS.VERYHARD.NAME, 200f)
		},
		{
			"Hard",
			BionicBatteryMonitor.MakeDifficultyModifier("difficultyWattage", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.BIONICPOWERUSE.LEVELS.HARD.NAME, 100f)
		},
		{
			"Default",
			BionicBatteryMonitor.MakeDifficultyModifier("difficultyWattage", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.BIONICPOWERUSE.LEVELS.DEFAULT.NAME, 0f)
		},
		{
			"Easy",
			BionicBatteryMonitor.MakeDifficultyModifier("difficultyWattage", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.BIONICPOWERUSE.LEVELS.EASY.NAME, -100f)
		},
		{
			"VeryEasy",
			BionicBatteryMonitor.MakeDifficultyModifier("difficultyWattage", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.BIONICPOWERUSE.LEVELS.VERYEASY.NAME, -150f)
		}
	};

	public class Def : StateMachine.BaseDef
	{
	}

	public struct WattageModifier
	{
		public WattageModifier(string id, string name, float value, float potentialValue)
		{
			this.id = id;
			this.name = name;
			this.value = value;
			this.potentialValue = potentialValue;
		}

		public float potentialValue;

		public float value;

		public string name;

		public string id;
	}

	public class OnlineStates : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State
	{
		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State idle;

		public BionicBatteryMonitor.UpkeepStates upkeep;

		public BionicBatteryMonitor.UpkeepStates critical;
	}

	public class UpkeepStates : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State
	{
		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State seekElectrobank;
	}

	public class OfflineStates : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State
	{
		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State waitingForBatteryDelivery;

		public BionicBatteryMonitor.RebootWorkableState waitingForBatteryInstallation;

		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State reboot;
	}

	public class RebootWorkableState : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State
	{
		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State waiting;

		public GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.State working;
	}

	public new class Instance : GameStateMachine<BionicBatteryMonitor, BionicBatteryMonitor.Instance, IStateMachineTarget, BionicBatteryMonitor.Def>.GameInstance
	{
		public float Wattage
		{
			get
			{
				return this.GetBaseWattage() + this.GetModifiersWattage();
			}
		}

		public bool IsOnline
		{
			get
			{
				return base.sm.IsOnline.Get(this);
			}
		}

		public bool InUpkeepTime
		{
			get
			{
				return this.schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Eat);
			}
		}

		public bool HaveInitialElectrobanksBeenSpawned
		{
			get
			{
				return base.sm.InitialElectrobanksSpawned.Get(this);
			}
		}

		public bool HasSpaceForNewElectrobank
		{
			get
			{
				return this.ElectrobankCount < this.ElectrobankCountCapacity;
			}
		}

		public int ElectrobankCount
		{
			get
			{
				return this.ChargedElectrobankCount + this.DepletedElectrobankCount;
			}
		}

		public int ChargedElectrobankCount
		{
			get
			{
				return base.sm.ChargedElectrobankCount.Get(this);
			}
		}

		public int DepletedElectrobankCount
		{
			get
			{
				return (int)(this.storage.GetMassAvailable("EmptyElectrobank") / 20f);
			}
		}

		public int DamagedElectrobankCount
		{
			get
			{
				return (int)(this.storage.GetMassAvailable("GarbageElectrobank") / 20f);
			}
		}

		public float CurrentCharge
		{
			get
			{
				return this.BionicBattery.value;
			}
		}

		public int ElectrobankCountCapacity
		{
			get
			{
				return (int)base.gameObject.GetAttributes().Get(Db.Get().Attributes.BionicBatteryCountCapacity.Id).GetTotalValue();
			}
		}

		public ReanimateBionicWorkable reanimateWorkable { get; private set; }

		public List<BionicBatteryMonitor.WattageModifier> Modifiers { get; set; } = new List<BionicBatteryMonitor.WattageModifier>();

		public Instance(IStateMachineTarget master, BionicBatteryMonitor.Def def)
			: base(master, def)
		{
			this.storage = base.gameObject.GetComponents<Storage>().FindFirst<Storage>((Storage s) => s.storageID == GameTags.StoragesIds.BionicBatteryStorage);
			this.reanimateWorkable = base.GetComponent<ReanimateBionicWorkable>();
			this.schedulable = base.GetComponent<Schedulable>();
			this.manualDelivery = base.GetComponent<ManualDeliveryKG>();
			this.selectable = base.GetComponent<KSelectable>();
			this.prefabID = base.GetComponent<KPrefabID>();
			this.dataHolder = base.GetComponent<MinionStorageDataHolder>();
			MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
			minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Combine(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			this.BionicBattery = Db.Get().Amounts.BionicInternalBattery.Lookup(base.gameObject);
			Storage storage = this.storage;
			storage.onDestroyItemsDropped = (Action<List<GameObject>>)Delegate.Combine(storage.onDestroyItemsDropped, new Action<List<GameObject>>(this.OnBatteriesDroppedFromDeath));
			Storage storage2 = this.storage;
			storage2.OnStorageChange = (Action<GameObject>)Delegate.Combine(storage2.OnStorageChange, new Action<GameObject>(this.OnElectrobankStorageChanged));
			base.Subscribe(540773776, new Action<object>(this.OnSkillsChanged));
			this.UpdateCapacityAmount();
			this.ApplyDifficultyModifiers();
		}

		public override void StartSM()
		{
			this.closestElectrobankSensor = base.GetComponent<Sensors>().GetSensor<ClosestElectrobankSensor>();
			ClosestElectrobankSensor closestElectrobankSensor = this.closestElectrobankSensor;
			closestElectrobankSensor.OnItemChanged = (Action<Electrobank>)Delegate.Combine(closestElectrobankSensor.OnItemChanged, new Action<Electrobank>(this.OnClosestElectrobankChanged));
			base.StartSM();
		}

		private void OnCopyMinionBegins(StoredMinionIdentity destination)
		{
			MinionStorageDataHolder.DataPackData dataPackData = new MinionStorageDataHolder.DataPackData
			{
				Bools = new bool[] { this.HaveInitialElectrobanksBeenSpawned, this.IsOnline }
			};
			this.dataHolder.UpdateData<BionicBatteryMonitor.Instance>(dataPackData);
		}

		public override void OnParamsDeserialized()
		{
			MinionStorageDataHolder.DataPack dataPack = this.dataHolder.GetDataPack<BionicBatteryMonitor.Instance>();
			if (dataPack != null && dataPack.IsStoringNewData)
			{
				MinionStorageDataHolder.DataPackData dataPackData = dataPack.ReadData();
				if (dataPackData != null)
				{
					bool flag = ((dataPackData.Bools != null && dataPackData.Bools.Length != 0) ? dataPackData.Bools[0] : this.HasSpaceForNewElectrobank);
					bool flag2 = ((dataPackData.Bools != null && dataPackData.Bools.Length > 1) ? dataPackData.Bools[1] : this.IsOnline);
					base.sm.InitialElectrobanksSpawned.Set(flag, this, false);
					base.sm.IsOnline.Set(flag2, this, false);
				}
			}
			this.RefreshCharge();
			base.OnParamsDeserialized();
		}

		public void DropAllDischargedElectrobanks()
		{
			List<GameObject> list = new List<GameObject>();
			this.storage.Find(GameTags.EmptyPortableBattery, list);
			foreach (GameObject gameObject in list)
			{
				this.storage.Drop(gameObject, true);
			}
		}

		protected override void OnCleanUp()
		{
			if (this.dataHolder != null)
			{
				MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
				minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Remove(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			}
			this.UpdateNotifications();
			base.OnCleanUp();
		}

		private void OnSkillsChanged(object o)
		{
			if (this.storage.capacityKg != (float)this.ElectrobankCountCapacity)
			{
				this.OnBatteryCapacityChanged();
			}
		}

		private void ApplyDifficultyModifiers()
		{
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.BionicWattage);
			BionicBatteryMonitor.WattageModifier wattageModifier;
			if (BionicBatteryMonitor.difficultyWattages.TryGetValue(currentQualitySetting.id, out wattageModifier))
			{
				this.Modifiers.Add(wattageModifier);
			}
		}

		private void UpdateCapacityAmount()
		{
			int num = this.ElectrobankCountCapacity - 4;
			this.BionicBattery.maxAttribute.ClearModifiers();
			this.BionicBattery.maxAttribute.Add(new AttributeModifier(Db.Get().Amounts.BionicInternalBattery.maxAttribute.Id, 120000f * (float)num, null, false, false, true));
		}

		private void OnBatteryCapacityChanged()
		{
			this.UpdateCapacityAmount();
			for (int i = this.storage.Count - 1; i >= 0; i--)
			{
				if (this.storage.Count > this.ElectrobankCountCapacity)
				{
					GameObject gameObject = this.storage.items[i];
					Electrobank component = gameObject.GetComponent<Electrobank>();
					this.storage.Drop(gameObject, true);
					Vector3 position = gameObject.transform.position;
					position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
					gameObject.transform.position = position;
					if (component != null && component.HasTag(GameTags.ChargedPortableBattery) && !component.IsFullyCharged)
					{
						component.RemovePower(component.Charge, true);
					}
				}
			}
			base.smi.storage.capacityKg = (float)this.ElectrobankCountCapacity;
		}

		private void OnClosestElectrobankChanged(Electrobank newItem)
		{
			base.sm.OnClosestAvailableElectrobankChangedSignal.Trigger(this);
		}

		public float GetBaseWattage()
		{
			return 200f;
		}

		public float GetModifiersWattage()
		{
			float num = 0f;
			foreach (BionicBatteryMonitor.WattageModifier wattageModifier in this.Modifiers)
			{
				num += wattageModifier.value;
			}
			return num;
		}

		private void OnElectrobankStorageChanged(object o)
		{
			this.ReorganizeElectrobanks();
			this.RefreshCharge();
			base.smi.sm.OnElectrobankStorageChanged.Trigger(this);
		}

		public void ReorganizeElectrobanks()
		{
			this.storage.items.Sort(delegate(GameObject b1, GameObject b2)
			{
				Electrobank component = b1.GetComponent<Electrobank>();
				Electrobank component2 = b2.GetComponent<Electrobank>();
				if (component == null)
				{
					return -1;
				}
				if (component2 == null)
				{
					return 1;
				}
				return component.Charge.CompareTo(component2.Charge);
			});
		}

		public void CreateWorkableChore()
		{
			if (this.reanimateChore == null)
			{
				this.reanimateChore = new WorkChore<ReanimateBionicWorkable>(Db.Get().ChoreTypes.RescueIncapacitated, this.reanimateWorkable, null, true, null, null, null, true, null, false, false, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, true);
				this.reanimateChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
			}
		}

		public void CancelWorkChore()
		{
			if (this.reanimateChore != null)
			{
				this.reanimateChore.Cancel("BionicBatteryMonitor.CancelChore");
				this.reanimateChore = null;
			}
		}

		public void SetOnlineState(bool online)
		{
			base.sm.IsOnline.Set(online, this, false);
			this.RefreshCharge();
		}

		public void SetManualDeliveryEnableState(bool enable)
		{
			if (!enable)
			{
				this.manualDelivery.capacity = 0f;
				this.manualDelivery.refillMass = 0f;
				this.manualDelivery.RequestedItemTag = null;
				this.manualDelivery.AbortDelivery("Manual delivery disabled");
				return;
			}
			Tag[] array = new Tag[GameTags.BionicIncompatibleBatteries.Count];
			GameTags.BionicIncompatibleBatteries.CopyTo(array, 0);
			base.smi.storage.capacityKg = (float)this.ElectrobankCountCapacity;
			base.smi.manualDelivery.capacity = 1f;
			base.smi.manualDelivery.refillMass = 1f;
			base.smi.manualDelivery.MinimumMass = 1f;
			this.manualDelivery.ForbiddenTags = array;
			this.manualDelivery.RequestedItemTag = GameTags.ChargedPortableBattery;
		}

		public GameObject GetFirstDischargedElectrobankInInventory()
		{
			return this.storage.FindFirst(GameTags.EmptyPortableBattery);
		}

		public Electrobank GetClosestElectrobank()
		{
			return this.closestElectrobankSensor.GetItem();
		}

		public void RefreshCharge()
		{
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			this.storage.Find(GameTags.ChargedPortableBattery, list);
			this.storage.Find(GameTags.EmptyPortableBattery, list2);
			float num = 0f;
			if (this.IsOnline)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Electrobank component = list[i].GetComponent<Electrobank>();
					num += component.Charge;
				}
			}
			this.BionicBattery.SetValue(num);
			base.sm.ChargedElectrobankCount.Set(list.Count, this, false);
			base.sm.DepletedElectrobankCount.Set(list2.Count, this, false);
			this.UpdateNotifications();
		}

		public void ConsumePower(float joules)
		{
			List<GameObject> list = new List<GameObject>();
			this.storage.Find(GameTags.ChargedPortableBattery, list);
			float num = joules;
			for (int i = 0; i < list.Count; i++)
			{
				Electrobank component = list[i].GetComponent<Electrobank>();
				float num2 = Mathf.Min(component.Charge, num);
				float num3 = component.RemovePower(num2, false);
				num -= num3;
				WorldResourceAmountTracker<ElectrobankTracker>.Get().RegisterAmountConsumed(component.ID, num3);
			}
			this.RefreshCharge();
		}

		public void DebugAddCharge(float joules)
		{
			float num = MathF.Min(joules, (float)this.ElectrobankCountCapacity * 120000f - this.CurrentCharge);
			List<GameObject> list = new List<GameObject>();
			this.storage.Find(GameTags.ChargedPortableBattery, list);
			int num2 = 0;
			while (num > 0f && num2 < list.Count)
			{
				Electrobank component = list[num2].GetComponent<Electrobank>();
				float num3 = Mathf.Min(120000f - component.Charge, num);
				component.AddPower(num3);
				num -= num3;
				num2++;
			}
			if (num > 0f && list.Count < this.ElectrobankCountCapacity)
			{
				int num4 = this.storage.items.Count - 1;
				while (num > 0f && num4 >= 0)
				{
					GameObject gameObject = this.storage.items[num4];
					if (!(gameObject == null))
					{
						Electrobank electrobank = gameObject.GetComponent<Electrobank>();
						if (electrobank == null && gameObject.HasTag(GameTags.EmptyPortableBattery))
						{
							this.storage.Drop(gameObject, true);
							GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab("DisposableElectrobank_RawMetal"), base.transform.position);
							gameObject2.SetActive(true);
							electrobank = gameObject2.GetComponent<Electrobank>();
							float num5 = Mathf.Clamp(electrobank.Charge - num, 0f, float.MaxValue);
							electrobank.RemovePower(num5, true);
							num -= electrobank.Charge;
							this.storage.Store(gameObject2, false, false, true, false);
						}
					}
					num4--;
				}
			}
			if (num > 0f && this.storage.items.Count < this.ElectrobankCountCapacity)
			{
				do
				{
					GameObject gameObject3 = Util.KInstantiate(Assets.GetPrefab("DisposableElectrobank_RawMetal"), base.transform.position);
					gameObject3.SetActive(true);
					Electrobank component2 = gameObject3.GetComponent<Electrobank>();
					float num6 = Mathf.Clamp(component2.Charge - num, 0f, float.MaxValue);
					component2.RemovePower(num6, true);
					num -= component2.Charge;
					this.storage.Store(gameObject3, false, false, true, false);
				}
				while (num > 0f && this.storage.items.Count < this.ElectrobankCountCapacity && num > 0f);
			}
			this.RefreshCharge();
		}

		private void UpdateNotifications()
		{
			this.criticalBatteryStatusItemGuid = this.selectable.ToggleStatusItem(Db.Get().DuplicantStatusItems.BionicCriticalBattery, this.criticalBatteryStatusItemGuid, BionicBatteryMonitor.ChargeIsBelowNotificationThreshold(base.smi) && !this.prefabID.HasTag(GameTags.Incapacitated) && !this.prefabID.HasTag(GameTags.Dead), base.gameObject);
		}

		public bool AddOrUpdateModifier(BionicBatteryMonitor.WattageModifier modifier, bool triggerCallbacks = true)
		{
			int num = this.Modifiers.FindIndex((BionicBatteryMonitor.WattageModifier mod) => mod.id == modifier.id);
			bool flag;
			if (num >= 0)
			{
				flag = this.Modifiers[num].name != modifier.name || this.Modifiers[num].value != modifier.value || this.Modifiers[num].potentialValue != modifier.potentialValue;
				this.Modifiers[num] = modifier;
			}
			else
			{
				this.Modifiers.Add(modifier);
				flag = true;
			}
			if (flag)
			{
				this.Modifiers.Sort((BionicBatteryMonitor.WattageModifier a, BionicBatteryMonitor.WattageModifier b) => b.value.CompareTo(a.value));
			}
			if (triggerCallbacks)
			{
				base.Trigger(1361471071, this.Wattage);
			}
			return flag;
		}

		public bool RemoveModifier(string modifierID, bool triggerCallbacks = true)
		{
			int num = this.Modifiers.FindIndex((BionicBatteryMonitor.WattageModifier mod) => mod.id == modifierID);
			if (num >= 0)
			{
				this.Modifiers.RemoveAt(num);
				if (triggerCallbacks)
				{
					base.Trigger(1361471071, this.Wattage);
				}
				this.Modifiers.Sort((BionicBatteryMonitor.WattageModifier a, BionicBatteryMonitor.WattageModifier b) => b.value.CompareTo(a.value));
				return true;
			}
			return false;
		}

		private void OnBatteriesDroppedFromDeath(List<GameObject> items)
		{
			if (items != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					Electrobank component = items[i].GetComponent<Electrobank>();
					if (component != null && component.HasTag(GameTags.ChargedPortableBattery) && !component.IsFullyCharged)
					{
						component.RemovePower(component.Charge, true);
					}
				}
			}
		}

		public void SpawnAndInstallInitialElectrobanks()
		{
			for (int i = 0; i < this.ElectrobankCountCapacity; i++)
			{
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("DisposableElectrobank_RawMetal"), base.transform.position);
				gameObject.SetActive(true);
				this.storage.Store(gameObject, false, false, true, false);
			}
			this.RefreshCharge();
			this.SetOnlineState(true);
			base.sm.InitialElectrobanksSpawned.Set(true, this, false);
		}

		public void AutomaticallyDropAllDepletedElectrobanks()
		{
			List<GameObject> list = new List<GameObject>();
			this.storage.Find(GameTags.EmptyPortableBattery, list);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				this.storage.Drop(gameObject, true);
			}
		}

		public Storage storage;

		public KPrefabID prefabID;

		private Schedulable schedulable;

		private AmountInstance BionicBattery;

		private ManualDeliveryKG manualDelivery;

		private ClosestElectrobankSensor closestElectrobankSensor;

		private KSelectable selectable;

		private MinionStorageDataHolder dataHolder;

		private Guid criticalBatteryStatusItemGuid;

		private Chore reanimateChore;
	}
}
