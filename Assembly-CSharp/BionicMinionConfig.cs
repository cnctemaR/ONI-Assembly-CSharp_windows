using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicMinionConfig : IEntityConfig
{
	public static string[] GetAttributes()
	{
		return BaseMinionConfig.BaseMinionAttributes().Append<string>(new string[]
		{
			Db.Get().Attributes.BionicBoosterSlots.Id,
			Db.Get().Attributes.BionicBatteryCountCapacity.Id
		});
	}

	public static string[] GetAmounts()
	{
		return BaseMinionConfig.BaseMinionAmounts().Append<string>(new string[]
		{
			Db.Get().Amounts.BionicOil.Id,
			Db.Get().Amounts.BionicGunk.Id,
			Db.Get().Amounts.BionicInternalBattery.Id,
			Db.Get().Amounts.BionicOxygenTank.Id
		});
	}

	public static AttributeModifier[] GetTraits()
	{
		return BaseMinionConfig.BaseMinionTraits(BionicMinionConfig.MODEL);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = BaseMinionConfig.BaseMinion(BionicMinionConfig.MODEL, BionicMinionConfig.GetAttributes(), BionicMinionConfig.GetAmounts(), BionicMinionConfig.GetTraits());
		gameObject.AddOrGet<AttributeLevels>().maxAttributeLevel = 0;
		Storage storage = gameObject.AddComponent<Storage>();
		storage.storageID = GameTags.StoragesIds.BionicBatteryStorage;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		storage.storageFilters = new List<Tag>(GameTags.BionicCompatibleBatteries);
		storage.allowItemRemoval = false;
		storage.showInUI = false;
		Storage storage2 = gameObject.AddComponent<Storage>();
		storage2.storageID = GameTags.StoragesIds.BionicUpgradeStorage;
		storage2.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		storage2.storageFilters = new List<Tag> { GameTags.BionicUpgrade };
		storage2.allowItemRemoval = false;
		storage2.showInUI = false;
		Storage storage3 = gameObject.AddComponent<Storage>();
		storage3.capacityKg = BionicOxygenTankMonitor.OXYGEN_TANK_CAPACITY_KG;
		storage3.storageID = GameTags.StoragesIds.BionicOxygenTankStorage;
		storage3.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		storage3.allowItemRemoval = false;
		storage3.showInUI = false;
		ManualDeliveryKG manualDeliveryKG = gameObject.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		manualDeliveryKG.capacity = 0f;
		manualDeliveryKG.refillMass = 0f;
		manualDeliveryKG.handlePrioritizable = false;
		gameObject.AddOrGet<ReanimateBionicWorkable>();
		gameObject.AddOrGet<WarmBlooded>().complexity = WarmBlooded.ComplexityType.HomeostasisWithoutCaloriesImpact;
		gameObject.AddOrGet<BionicMinionStorageExtension>();
		gameObject.AddOrGet<MinionStorageDataHolder>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
		BaseMinionConfig.BasePrefabInit(go, BionicMinionConfig.MODEL);
		AmountInstance amountInstance = Db.Get().Amounts.BionicOil.Lookup(go);
		amountInstance.value = amountInstance.GetMax();
		AmountInstance amountInstance2 = Db.Get().Amounts.BionicGunk.Lookup(go);
		amountInstance2.value = amountInstance2.GetMin();
	}

	public void OnSpawn(GameObject go)
	{
		Sensors component = go.GetComponent<Sensors>();
		component.Add(new ClosestElectrobankSensor(component, true));
		component.Add(new ClosestOxygenCanisterSensor(component, false));
		component.Add(new ClosestLubricantSensor(component, false));
		BaseMinionConfig.BaseOnSpawn(go, BionicMinionConfig.MODEL, this.RATIONAL_AI_STATE_MACHINES);
		component.GetSensor<SafeCellSensor>().AddIgnoredFlagsSet(BionicMinionConfig.ID, SafeCellQuery.SafeFlags.IsBreathable);
		BionicOxygenTankMonitor.Instance smi = go.GetSMI<BionicOxygenTankMonitor.Instance>();
		if (smi != null)
		{
			go.GetComponent<OxygenBreather>().AddGasProvider(smi);
		}
		this.BionicFreeDiscoveries(go);
		go.Trigger(1589886948, go);
	}

	private void BionicFreeDiscoveries(GameObject instance)
	{
		GameScheduler.Instance.Schedule("BionicUnlockCraftingTable", 8f, delegate(object data)
		{
			TechItem techItem = Db.Get().TechItems.Get("CraftingTable");
			if (!techItem.IsComplete())
			{
				Notifier component = Game.Instance.GetComponent<Notifier>();
				Notification notification = new Notification(MISC.NOTIFICATIONS.BIONICRESEARCHUNLOCK.NAME, NotificationType.MessageImportant, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.BIONICRESEARCHUNLOCK.MESSAGEBODY.Replace("{0}", Assets.GetPrefab("CraftingTable").GetProperName()), Assets.GetPrefab("CraftingTable").GetProperName(), true, 0f, null, null, null, true, true, false);
				component.Add(notification, "");
				techItem.POIUnlocked();
			}
			DiscoveredResources.Instance.Discover(PowerControlStationConfig.TINKER_TOOLS);
		}, null, null);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.DLC3;
	}

	public BionicMinionConfig()
	{
		Func<RationalAi.Instance, StateMachine.Instance>[] array = BaseMinionConfig.BaseRationalAiStateMachines();
		Func<RationalAi.Instance, StateMachine.Instance>[] array2 = new Func<RationalAi.Instance, StateMachine.Instance>[10];
		array2[0] = (RationalAi.Instance smi) => new BreathMonitor.Instance(smi.master)
		{
			canRecoverBreath = false
		};
		array2[1] = (RationalAi.Instance smi) => new SteppedInMonitor.Instance(smi.master, new string[] { "CarpetFeet" });
		array2[2] = (RationalAi.Instance smi) => new BionicBatteryMonitor.Instance(smi.master, new BionicBatteryMonitor.Def());
		array2[3] = (RationalAi.Instance smi) => new BionicBedTimeMonitor.Instance(smi.master, new BionicBedTimeMonitor.Def());
		array2[4] = (RationalAi.Instance smi) => new BionicMicrochipMonitor.Instance(smi.master, new BionicMicrochipMonitor.Def());
		array2[5] = (RationalAi.Instance smi) => new BionicOilMonitor.Instance(smi.master, new BionicOilMonitor.Def());
		array2[6] = (RationalAi.Instance smi) => new GunkMonitor.Instance(smi.master, new GunkMonitor.Def());
		array2[7] = (RationalAi.Instance smi) => new BionicWaterDamageMonitor.Instance(smi.master, new BionicWaterDamageMonitor.Def());
		array2[8] = (RationalAi.Instance smi) => new BionicUpgradesMonitor.Instance(smi.master, new BionicUpgradesMonitor.Def());
		array2[9] = (RationalAi.Instance smi) => new BionicOxygenTankMonitor.Instance(smi.master, new BionicOxygenTankMonitor.Def());
		this.RATIONAL_AI_STATE_MACHINES = array.Append<Func<RationalAi.Instance, StateMachine.Instance>>(array2);
		base..ctor();
	}

	public static Tag MODEL = GameTags.Minions.Models.Bionic;

	public static string NAME = DUPLICANTS.MODEL.BIONIC.NAME;

	public static string ID = BionicMinionConfig.MODEL.ToString();

	public static string[] DEFAULT_BIONIC_TRAITS = new string[] { "BionicBaseline" };

	public Func<RationalAi.Instance, StateMachine.Instance>[] RATIONAL_AI_STATE_MACHINES;
}
