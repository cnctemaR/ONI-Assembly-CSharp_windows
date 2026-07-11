using System;
using TUNING;
using UnityEngine;

public class IceMachineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "IceMachine";
		int num = 2;
		int num2 = 3;
		string text2 = "freezerator_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		storage.capacityKg = 50f;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage2.showInUI = true;
		storage2.capacityKg = 50f;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		IceMachine iceMachine = go.AddOrGet<IceMachine>();
		iceMachine.SetStorages(storage, storage2);
		iceMachine.targetTemperature = 253.15f;
		iceMachine.energyConsumption = 120f;
		iceMachine.energyWaste = 4.5f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.Water;
		manualDeliveryKG.capacity = 50f;
		manualDeliveryKG.refillMass = 10f;
		manualDeliveryKG.minimumMass = 10f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "IceMachine";

	private const float WATER_STORAGE = 50f;

	private const float WATER_INPUT_RATE = 0.5f;

	private const float ICE_OUTPUT_RATE = 0.5f;

	private const float ICE_PER_LOAD = 50f;

	private const float TARGET_ICE_TEMP = 253.15f;
}
