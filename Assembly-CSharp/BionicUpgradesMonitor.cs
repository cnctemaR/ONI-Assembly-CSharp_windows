using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

public class BionicUpgradesMonitor : GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>
{
	public static void CreateAssignableSlots(MinionAssignablesProxy minionAssignablesProxy)
	{
		AssignableSlot bionicUpgrade = Db.Get().AssignableSlots.BionicUpgrade;
		int num = Mathf.Max(0, 7);
		for (int i = 0; i < num; i++)
		{
			string text = (i + 2).ToString();
			BionicUpgradesMonitor.AddAssignableSlot(bionicUpgrade, text, minionAssignablesProxy);
		}
	}

	private static void AddAssignableSlot(AssignableSlot bionicUpgradeSlot, string IDSufix, MinionAssignablesProxy minionAssignablesProxy)
	{
		Ownables component = minionAssignablesProxy.GetComponent<Ownables>();
		if (bionicUpgradeSlot is OwnableSlot)
		{
			OwnableSlotInstance ownableSlotInstance = new OwnableSlotInstance(component, (OwnableSlot)bionicUpgradeSlot);
			OwnableSlotInstance ownableSlotInstance2 = ownableSlotInstance;
			ownableSlotInstance2.ID += IDSufix;
			component.Add(ownableSlotInstance);
			return;
		}
		if (bionicUpgradeSlot is EquipmentSlot)
		{
			Equipment component2 = component.GetComponent<Equipment>();
			EquipmentSlotInstance equipmentSlotInstance = new EquipmentSlotInstance(component2, (EquipmentSlot)bionicUpgradeSlot);
			EquipmentSlotInstance equipmentSlotInstance2 = equipmentSlotInstance;
			equipmentSlotInstance2.ID += IDSufix;
			component2.Add(equipmentSlotInstance);
		}
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.initialize;
		this.initialize.Enter(new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State.Callback(BionicUpgradesMonitor.InitializeSlots)).EnterTransition(this.firstSpawn, new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Transition.ConditionCallback(BionicUpgradesMonitor.IsFirstTimeSpawningThisBionic)).EnterGoTo(this.inactive);
		this.firstSpawn.Enter(new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State.Callback(BionicUpgradesMonitor.SpawnAndInstallInitialUpgrade));
		this.inactive.EventTransition(GameHashes.BionicOnline, this.active, new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Transition.ConditionCallback(BionicUpgradesMonitor.IsBionicOnline)).Enter(new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State.Callback(BionicUpgradesMonitor.UpdateBatteryMonitorWattageModifiers));
		this.active.DefaultState(this.active.idle).EventTransition(GameHashes.BionicOffline, this.inactive, GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Not(new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Transition.ConditionCallback(BionicUpgradesMonitor.IsBionicOnline))).EventHandler(GameHashes.BionicUpgradeWattageChanged, new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State.Callback(BionicUpgradesMonitor.UpdateBatteryMonitorWattageModifiers))
			.Enter(new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State.Callback(BionicUpgradesMonitor.UpdateBatteryMonitorWattageModifiers));
		this.active.idle.OnSignal(this.UpgradeSlotAssignationChanged, this.active.seeking, new Func<BionicUpgradesMonitor.Instance, bool>(BionicUpgradesMonitor.WantsToInstallNewUpgrades));
		this.active.seeking.OnSignal(this.UpgradeSlotAssignationChanged, this.active.idle, new Func<BionicUpgradesMonitor.Instance, bool>(BionicUpgradesMonitor.DoesNotWantsToInstallNewUpgrades)).DefaultState(this.active.seeking.inProgress);
		this.active.seeking.inProgress.ToggleChore((BionicUpgradesMonitor.Instance smi) => new SeekAndInstallBionicUpgradeChore(smi.master), this.active.idle, this.active.seeking.failed);
		this.active.seeking.failed.EnterTransition(this.active.idle, new StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Transition.ConditionCallback(BionicUpgradesMonitor.DoesNotWantsToInstallNewUpgrades)).GoTo(this.active.seeking.inProgress);
	}

	public static void InitializeSlots(BionicUpgradesMonitor.Instance smi)
	{
		smi.InitializeSlots();
	}

	public static bool IsBionicOnline(BionicUpgradesMonitor.Instance smi)
	{
		return smi.IsOnline;
	}

	public static bool WantsToInstallNewUpgrades(BionicUpgradesMonitor.Instance smi)
	{
		return smi.HasAnyUpgradeAssigned;
	}

	public static bool DoesNotWantsToInstallNewUpgrades(BionicUpgradesMonitor.Instance smi)
	{
		return !BionicUpgradesMonitor.WantsToInstallNewUpgrades(smi);
	}

	public static bool HasUpgradesInstalled(BionicUpgradesMonitor.Instance smi)
	{
		return smi.HasAnyUpgradeInstalled;
	}

	public static bool IsFirstTimeSpawningThisBionic(BionicUpgradesMonitor.Instance smi)
	{
		return !smi.sm.InitialUpgradeSpawned.Get(smi);
	}

	public static void UpdateBatteryMonitorWattageModifiers(BionicUpgradesMonitor.Instance smi)
	{
		smi.UpdateBatteryMonitorWattageModifiers();
	}

	public static void SpawnAndInstallInitialUpgrade(BionicUpgradesMonitor.Instance smi)
	{
		string text = smi.GetComponent<Traits>().GetTraitIds().Find((string t) => DUPLICANTSTATS.BIONICUPGRADETRAITS.Find((DUPLICANTSTATS.TraitVal st) => st.id == t).id == t);
		if (text != null)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(BionicUpgradeComponentConfig.GetBionicUpgradePrefabIDWithTraitID(text)), smi.master.transform.position);
			gameObject.SetActive(true);
			IAssignableIdentity component = smi.GetComponent<IAssignableIdentity>();
			BionicUpgradeComponent component2 = gameObject.GetComponent<BionicUpgradeComponent>();
			component2.Assign(component);
			smi.InstallUpgrade(component2);
		}
		smi.sm.InitialUpgradeSpawned.Set(true, smi, false);
		smi.GoTo(smi.sm.inactive);
	}

	public const int MAX_POSSIBLE_SLOT_COUNT = 8;

	public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State initialize;

	public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State firstSpawn;

	public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State inactive;

	public BionicUpgradesMonitor.ActiveStates active;

	private StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.Signal UpgradeSlotAssignationChanged;

	private StateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.BoolParameter InitialUpgradeSpawned;

	public class Def : StateMachine.BaseDef
	{
	}

	public class SeekingStates : GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State
	{
		public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State inProgress;

		public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State failed;
	}

	public class ActiveStates : GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State
	{
		public GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.State idle;

		public BionicUpgradesMonitor.SeekingStates seeking;
	}

	public new class Instance : GameStateMachine<BionicUpgradesMonitor, BionicUpgradesMonitor.Instance, IStateMachineTarget, BionicUpgradesMonitor.Def>.GameInstance
	{
		public bool IsOnline
		{
			get
			{
				return this.batteryMonitor != null && this.batteryMonitor.IsOnline;
			}
		}

		public bool HasAnyUpgradeAssigned
		{
			get
			{
				return this.upgradeComponentSlots != null && this.GetAnyAssignedSlot() != null;
			}
		}

		public bool HasAnyUpgradeInstalled
		{
			get
			{
				return this.upgradeComponentSlots != null && this.GetAnyInstalledUpgradeSlot() != null;
			}
		}

		public int UnlockedSlotCount
		{
			get
			{
				return Math.Clamp((int)base.gameObject.GetAttributes().Get(Db.Get().Attributes.BionicBoosterSlots.Id).GetTotalValue(), 0, 8);
			}
		}

		public int AssignedSlotCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
				{
					if (this.upgradeComponentSlots[i].assignedUpgradeComponent != null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public Instance(IStateMachineTarget master, BionicUpgradesMonitor.Def def)
			: base(master, def)
		{
			IAssignableIdentity component = base.GetComponent<IAssignableIdentity>();
			this.dataHolder = base.GetComponent<MinionStorageDataHolder>();
			MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
			minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Combine(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			this.batteryMonitor = base.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
			this.navigator = base.GetComponent<Navigator>();
			this.minionOwnables = component.GetSoleOwner();
			this.upgradesStorage = base.gameObject.GetComponents<Storage>().FindFirst<Storage>((Storage s) => s.storageID == GameTags.StoragesIds.BionicUpgradeStorage);
			this.CreateUpgradeSlots();
			base.Subscribe(540773776, new Action<object>(this.OnSlotCountAttributeChanged));
			Game.Instance.Trigger(-1523247426, this);
		}

		private void OnCopyMinionBegins(StoredMinionIdentity destination)
		{
			Tag[] array = new Tag[this.upgradeComponentSlots.Length];
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				array[i] = this.upgradeComponentSlots[i].InstalledUpgradeID;
			}
			MinionStorageDataHolder.DataPackData dataPackData = new MinionStorageDataHolder.DataPackData
			{
				Bools = new bool[] { base.smi.sm.InitialUpgradeSpawned.Get(base.smi) },
				Tags = array
			};
			this.dataHolder.UpdateData<BionicUpgradesMonitor.Instance>(dataPackData);
		}

		public override void OnParamsDeserialized()
		{
			MinionStorageDataHolder.DataPack dataPack = this.dataHolder.GetDataPack<BionicUpgradesMonitor.Instance>();
			if (dataPack != null && dataPack.IsStoringNewData)
			{
				MinionStorageDataHolder.DataPackData dataPackData = dataPack.ReadData();
				if (dataPackData != null)
				{
					base.sm.InitialUpgradeSpawned.Set(dataPackData.Bools[0], base.smi, false);
					if (dataPackData.Tags != null)
					{
						for (int i = 0; i < Mathf.Min(dataPackData.Tags.Length, this.upgradeComponentSlots.Length); i++)
						{
							Tag tag = dataPackData.Tags[i];
							this.upgradeComponentSlots[i].DeserializeAction_OverrideInstalledUpgradePrefabID(tag);
						}
					}
				}
			}
			base.OnParamsDeserialized();
		}

		protected override void OnCleanUp()
		{
			if (this.dataHolder != null)
			{
				MinionStorageDataHolder minionStorageDataHolder = this.dataHolder;
				minionStorageDataHolder.OnCopyBegins = (Action<StoredMinionIdentity>)Delegate.Remove(minionStorageDataHolder.OnCopyBegins, new Action<StoredMinionIdentity>(this.OnCopyMinionBegins));
			}
			base.OnCleanUp();
		}

		public void LockSlot(BionicUpgradesMonitor.UpgradeComponentSlot slot)
		{
			this.UninstallUpgrade(slot);
			if (slot.HasUpgradeComponentAssigned && slot.HasSpawned)
			{
				slot.InternalUninstall();
			}
			slot.InternalLock();
		}

		public void UnlockSlot(BionicUpgradesMonitor.UpgradeComponentSlot slot)
		{
			slot.InternalUnlock();
		}

		public void InstallUpgrade(BionicUpgradeComponent upgradeComponent)
		{
			BionicUpgradesMonitor.UpgradeComponentSlot slotForAssignedUpgrade = this.GetSlotForAssignedUpgrade(upgradeComponent);
			if (slotForAssignedUpgrade == null)
			{
				return;
			}
			slotForAssignedUpgrade.InternalInstall();
			Game.Instance.Trigger(-1523247426, this);
		}

		public void UninstallUpgrade(BionicUpgradesMonitor.UpgradeComponentSlot slot)
		{
			if (slot != null && slot.HasUpgradeInstalled)
			{
				slot.InternalUninstall();
				Game.Instance.Trigger(-1523247426, this);
			}
		}

		public void UpdateBatteryMonitorWattageModifiers()
		{
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				flag &= this.upgradeComponentSlots[i].HasUpgradeInstalled;
				string text = "UPGRADE_SLOT_" + i.ToString();
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (!upgradeComponentSlot.HasUpgradeInstalled)
				{
					flag2 |= this.batteryMonitor.RemoveModifier(text, false);
				}
				else
				{
					BionicBatteryMonitor.WattageModifier wattageModifier = new BionicBatteryMonitor.WattageModifier
					{
						id = text,
						name = upgradeComponentSlot.installedUpgradeComponent.CurrentWattageName,
						value = upgradeComponentSlot.installedUpgradeComponent.CurrentWattage,
						potentialValue = upgradeComponentSlot.installedUpgradeComponent.PotentialWattage
					};
					flag2 |= this.batteryMonitor.AddOrUpdateModifier(wattageModifier, false);
				}
			}
			if (flag2)
			{
				this.batteryMonitor.Trigger(1361471071, null);
			}
			if (flag)
			{
				SaveGame.Instance.ColonyAchievementTracker.fullyBoostedBionic = true;
			}
		}

		private void OnSlotCountAttributeChanged(object data)
		{
			int unlockedSlotCount = this.UnlockedSlotCount;
			bool flag = false;
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				bool flag2 = i >= unlockedSlotCount;
				if (upgradeComponentSlot.IsLocked != flag2)
				{
					flag = true;
					if (flag2)
					{
						this.LockSlot(upgradeComponentSlot);
					}
					else
					{
						this.UnlockSlot(upgradeComponentSlot);
					}
				}
			}
			this.UpdateBatteryMonitorWattageModifiers();
			if (flag)
			{
				base.Trigger(1095596132, null);
			}
		}

		private void CreateUpgradeSlots()
		{
			AssignableSlot bionicUpgrade = Db.Get().AssignableSlots.BionicUpgrade;
			this.minionOwnables.GetSlots(bionicUpgrade);
			this.upgradeComponentSlots = new BionicUpgradesMonitor.UpgradeComponentSlot[8];
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = new BionicUpgradesMonitor.UpgradeComponentSlot();
				this.upgradeComponentSlots[i] = upgradeComponentSlot;
			}
		}

		public void InitializeSlots()
		{
			AssignableSlot bionicUpgrade = Db.Get().AssignableSlots.BionicUpgrade;
			AssignableSlotInstance[] slots = this.minionOwnables.GetSlots(bionicUpgrade);
			int unlockedSlotCount = this.UnlockedSlotCount;
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				this.InitializeUpgradeSlot(upgradeComponentSlot, slots[i]);
			}
			for (int j = 0; j < this.upgradeComponentSlots.Length; j++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot2 = this.upgradeComponentSlots[j];
				upgradeComponentSlot2.OnSpawn(this);
				bool flag = j >= unlockedSlotCount;
				if (flag != upgradeComponentSlot2.IsLocked)
				{
					if (flag)
					{
						this.LockSlot(upgradeComponentSlot2);
					}
					else
					{
						this.UnlockSlot(upgradeComponentSlot2);
					}
				}
			}
		}

		private void InitializeUpgradeSlot(BionicUpgradesMonitor.UpgradeComponentSlot slot, AssignableSlotInstance assignableSlotInstance)
		{
			slot.Initialize(assignableSlotInstance, this.upgradesStorage, this);
			slot.OnInstalledUpgradeReassigned = (Action<BionicUpgradesMonitor.UpgradeComponentSlot, IAssignableIdentity>)Delegate.Combine(slot.OnInstalledUpgradeReassigned, new Action<BionicUpgradesMonitor.UpgradeComponentSlot, IAssignableIdentity>(this.OnInstalledUpgradeComponentReassigned));
			slot.OnAssignedUpgradeChanged = (Action<BionicUpgradesMonitor.UpgradeComponentSlot>)Delegate.Combine(slot.OnAssignedUpgradeChanged, new Action<BionicUpgradesMonitor.UpgradeComponentSlot>(this.OnSlotAssignationChanged));
		}

		private void OnSlotAssignationChanged(BionicUpgradesMonitor.UpgradeComponentSlot slot)
		{
			base.sm.UpgradeSlotAssignationChanged.Trigger(this);
		}

		private void OnInstalledUpgradeComponentReassigned(BionicUpgradesMonitor.UpgradeComponentSlot slot, IAssignableIdentity new_assignee)
		{
			if (!slot.AssignedUpgradeMatchesInstalledUpgrade)
			{
				this.UninstallUpgrade(slot);
			}
		}

		private BionicUpgradesMonitor.UpgradeComponentSlot GetSlotForAssignedUpgrade(BionicUpgradeComponent upgradeComponent)
		{
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (upgradeComponentSlot != null && !upgradeComponentSlot.HasUpgradeInstalled && upgradeComponentSlot.HasUpgradeComponentAssigned && upgradeComponentSlot.assignedUpgradeComponent == upgradeComponent)
				{
					return upgradeComponentSlot;
				}
			}
			return null;
		}

		public BionicUpgradesMonitor.UpgradeComponentSlot GetAnyAssignedSlot()
		{
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (upgradeComponentSlot != null && !upgradeComponentSlot.HasUpgradeInstalled && upgradeComponentSlot.HasUpgradeComponentAssigned)
				{
					return upgradeComponentSlot;
				}
			}
			return null;
		}

		public BionicUpgradesMonitor.UpgradeComponentSlot GetAnyReachableAssignedSlot()
		{
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (upgradeComponentSlot != null && !upgradeComponentSlot.HasUpgradeInstalled && upgradeComponentSlot.HasUpgradeComponentAssigned && this.IsBionicUpgradeComponentObjectAbleToBePickedUp(upgradeComponentSlot.assignedUpgradeComponent))
				{
					return upgradeComponentSlot;
				}
			}
			return null;
		}

		public bool IsBionicUpgradeComponentObjectAbleToBePickedUp(BionicUpgradeComponent upgradecComponent)
		{
			Pickupable component = upgradecComponent.GetComponent<Pickupable>();
			return !(component == null) && !component.KPrefabID.HasTag(GameTags.StoredPrivate) && component.CouldBePickedUpByMinion(base.GetComponent<KPrefabID>().InstanceID) && this.navigator.CanReach(component);
		}

		private BionicUpgradesMonitor.UpgradeComponentSlot GetAnyInstalledUpgradeSlot()
		{
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (upgradeComponentSlot != null && upgradeComponentSlot.HasUpgradeInstalled)
				{
					return upgradeComponentSlot;
				}
			}
			return null;
		}

		public BionicUpgradesMonitor.UpgradeComponentSlot GetFirstEmptyAvailableSlot()
		{
			for (int i = 0; i < this.upgradeComponentSlots.Length; i++)
			{
				BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot = this.upgradeComponentSlots[i];
				if (!upgradeComponentSlot.IsLocked && !upgradeComponentSlot.HasUpgradeInstalled && !upgradeComponentSlot.HasUpgradeComponentAssigned)
				{
					return upgradeComponentSlot;
				}
			}
			return null;
		}

		public int CountBoosterAssignments(Tag boosterID)
		{
			int num = 0;
			foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in this.upgradeComponentSlots)
			{
				if (!(upgradeComponentSlot.assignedUpgradeComponent == null) && upgradeComponentSlot.assignedUpgradeComponent.PrefabID() == boosterID)
				{
					num++;
				}
			}
			return num;
		}

		[Serialize]
		public BionicUpgradesMonitor.UpgradeComponentSlot[] upgradeComponentSlots;

		private BionicBatteryMonitor.Instance batteryMonitor;

		private Storage upgradesStorage;

		private Ownables minionOwnables;

		private MinionStorageDataHolder dataHolder;

		private Navigator navigator;

		[SerializationConfig(MemberSerialization.OptIn)]
		private struct StorageDataHolderData
		{
			[Serialize]
			public bool initialUpgradesSpawned;

			[Serialize]
			public Tag[] upgradeComponentSlotsInstalledTags;
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class UpgradeComponentSlot
	{
		public bool HasUpgradeInstalled
		{
			get
			{
				return this.installedUpgradePrefabID != Tag.Invalid;
			}
		}

		public bool HasUpgradeComponentAssigned
		{
			get
			{
				return this.assignableSlotInstance.IsAssigned() && !this.assignableSlotInstance.IsUnassigning();
			}
		}

		public bool AssignedUpgradeMatchesInstalledUpgrade
		{
			get
			{
				return this.assignedUpgradeComponent == this.installedUpgradeComponent;
			}
		}

		public bool HasSpawned { get; private set; }

		public bool IsLocked { get; private set; }

		public float WattageCost
		{
			get
			{
				if (!this.HasUpgradeInstalled)
				{
					return 0f;
				}
				return this.installedUpgradeComponent.CurrentWattage;
			}
		}

		public Func<StateMachine.Instance, StateMachine.Instance> StateMachine
		{
			get
			{
				if (!this.HasUpgradeInstalled)
				{
					return null;
				}
				return this.installedUpgradeComponent.StateMachine;
			}
		}

		public Tag InstalledUpgradeID
		{
			get
			{
				return this.installedUpgradePrefabID;
			}
		}

		public BionicUpgradeComponent assignedUpgradeComponent
		{
			get
			{
				if (!this.assignableSlotInstance.IsUnassigning())
				{
					return this.assignableSlotInstance.assignable as BionicUpgradeComponent;
				}
				return null;
			}
		}

		public BionicUpgradeComponent installedUpgradeComponent
		{
			get
			{
				if (this.HasUpgradeInstalled)
				{
					if (this._installedUpgradeComponent == null)
					{
						global::Debug.LogWarning("Error on BionicUpgradeMonitor. storage does not contains bionic upgrade with id " + this.InstalledUpgradeID.ToString() + " this could be due to loading an old save on a new version");
						this.installedUpgradePrefabID = Tag.Invalid;
					}
					return this._installedUpgradeComponent;
				}
				this._installedUpgradeComponent = null;
				return null;
			}
		}

		public void DeserializeAction_OverrideInstalledUpgradePrefabID(Tag installedUpgradePrefabID)
		{
			this.installedUpgradePrefabID = installedUpgradePrefabID;
		}

		public void Initialize(AssignableSlotInstance assignableSlotInstance, Storage storage, BionicUpgradesMonitor.Instance master)
		{
			this.assignableSlotInstance = assignableSlotInstance;
			this.assignableSlotInstance.assignables.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().Subscribe(-1585839766, new Action<object>(this.OnAssignablesChanged));
			this.storage = storage;
			this.master = master;
			this._lastAssignedUpgradeComponent = this.assignedUpgradeComponent;
		}

		public AssignableSlotInstance GetAssignableSlotInstance()
		{
			return this.assignableSlotInstance;
		}

		public void OnSpawn(BionicUpgradesMonitor.Instance smi)
		{
			if (this.HasUpgradeInstalled && this._installedUpgradeComponent == null)
			{
				GameObject gameObject = null;
				int num = 0;
				List<GameObject> list = new List<GameObject>();
				this.storage.Find(this.InstalledUpgradeID, list);
				while (num < list.Count && this._installedUpgradeComponent == null)
				{
					GameObject gameObject2 = list[num];
					bool flag = false;
					foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in smi.upgradeComponentSlots)
					{
						if (upgradeComponentSlot != this && upgradeComponentSlot.HasSpawned && !(upgradeComponentSlot.InstalledUpgradeID != this.InstalledUpgradeID) && upgradeComponentSlot.installedUpgradeComponent.gameObject == gameObject2)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						gameObject = gameObject2;
						break;
					}
					num++;
				}
				if (gameObject != null)
				{
					this._installedUpgradeComponent = gameObject.GetComponent<BionicUpgradeComponent>();
					this.StartBoosterSM();
				}
			}
			if (this.HasUpgradeInstalled && this.installedUpgradeComponent != null)
			{
				if (!this.HasUpgradeComponentAssigned)
				{
					this.installedUpgradeComponent.Assign(this.assignableSlotInstance.assignables.GetComponent<MinionAssignablesProxy>(), this.assignableSlotInstance);
				}
				this.SubscribeToInstallledUpgradeAssignable();
			}
			this.HasSpawned = true;
		}

		public void SubscribeToInstallledUpgradeAssignable()
		{
			this.UnsubscribeFromInstalledUpgradeAssignable();
			this.installedUpgradeSubscribeCallbackIDX = this.installedUpgradeComponent.Subscribe(684616645, new Action<object>(this.OnInstalledComponentReassigned));
		}

		public void UnsubscribeFromInstalledUpgradeAssignable()
		{
			if (this.installedUpgradeSubscribeCallbackIDX != -1)
			{
				this.installedUpgradeComponent.Unsubscribe(this.installedUpgradeSubscribeCallbackIDX);
				this.installedUpgradeSubscribeCallbackIDX = -1;
			}
		}

		private void OnInstalledComponentReassigned(object obj)
		{
			IAssignableIdentity assignableIdentity = ((obj == null) ? null : ((IAssignableIdentity)obj));
			Action<BionicUpgradesMonitor.UpgradeComponentSlot, IAssignableIdentity> onInstalledUpgradeReassigned = this.OnInstalledUpgradeReassigned;
			if (onInstalledUpgradeReassigned == null)
			{
				return;
			}
			onInstalledUpgradeReassigned(this, assignableIdentity);
		}

		private void OnAssignablesChanged(object o)
		{
			if (this._lastAssignedUpgradeComponent != this.assignedUpgradeComponent)
			{
				this._lastAssignedUpgradeComponent = this.assignedUpgradeComponent;
				Action<BionicUpgradesMonitor.UpgradeComponentSlot> onAssignedUpgradeChanged = this.OnAssignedUpgradeChanged;
				if (onAssignedUpgradeChanged == null)
				{
					return;
				}
				onAssignedUpgradeChanged(this);
			}
		}

		private void StartBoosterSM()
		{
			this._upgradeSmi = this.installedUpgradeComponent.StateMachine(this.master);
			this._upgradeSmi.StartSM();
		}

		public void InternalInstall()
		{
			if (!this.HasUpgradeInstalled && this.HasUpgradeComponentAssigned)
			{
				this.storage.Store(this.assignedUpgradeComponent.gameObject, true, false, true, false);
				this.installedUpgradePrefabID = this.assignedUpgradeComponent.PrefabID();
				this._installedUpgradeComponent = this.assignedUpgradeComponent;
				this.SubscribeToInstallledUpgradeAssignable();
				this.StartBoosterSM();
				GameObject targetGameObject = this.assignableSlotInstance.assignables.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject != null)
				{
					targetGameObject.Trigger(2000325176, null);
				}
			}
		}

		public void InternalUninstall()
		{
			if (this.HasUpgradeInstalled)
			{
				this.UnsubscribeFromInstalledUpgradeAssignable();
				GameObject gameObject = this.installedUpgradeComponent.gameObject;
				this.installedUpgradeComponent.Unassign();
				this.storage.Drop(gameObject, true);
				this.installedUpgradePrefabID = Tag.Invalid;
				this._installedUpgradeComponent = null;
				if (this._upgradeSmi != null)
				{
					this._upgradeSmi.StopSM("Uninstall");
					this._upgradeSmi = null;
				}
				GameObject targetGameObject = this.assignableSlotInstance.assignables.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject != null)
				{
					targetGameObject.Trigger(2000325176, null);
				}
			}
		}

		public void InternalLock()
		{
			this.IsLocked = true;
		}

		public void InternalUnlock()
		{
			this.IsLocked = false;
		}

		private BionicUpgradeComponent _installedUpgradeComponent;

		private BionicUpgradeComponent _lastAssignedUpgradeComponent;

		[Serialize]
		private Tag installedUpgradePrefabID = Tag.Invalid;

		public Action<BionicUpgradesMonitor.UpgradeComponentSlot, IAssignableIdentity> OnInstalledUpgradeReassigned;

		public Action<BionicUpgradesMonitor.UpgradeComponentSlot> OnAssignedUpgradeChanged;

		private AssignableSlotInstance assignableSlotInstance;

		private Storage storage;

		private int installedUpgradeSubscribeCallbackIDX = -1;

		private StateMachine.Instance _upgradeSmi;

		private BionicUpgradesMonitor.Instance master;
	}
}
