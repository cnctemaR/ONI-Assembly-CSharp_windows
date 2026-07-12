using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TravelTubeEntranceConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "TravelTubeEntrance";
		int num = 3;
		int num2 = 2;
		string text2 = "tube_launcher_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		TravelTubeEntrance travelTubeEntrance = go.AddOrGet<TravelTubeEntrance>();
		travelTubeEntrance.waxPerLaunch = 0.05f;
		travelTubeEntrance.joulesPerLaunch = 10000f;
		travelTubeEntrance.jouleCapacity = 40000f;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 10f;
		List<Storage.StoredItemModifier> list = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Preserve
		};
		storage.SetDefaultStoredItemModifiers(list);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = SimHashes.MilkFat.CreateTag();
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.refillMass = 0.05f;
		manualDeliveryKG.SetStorage(storage);
		go.AddOrGet<TravelTubeEntrance.Work>();
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<EnergyConsumerSelfSustaining>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().visualizeRequirements = RequireInputs.Requirements.NoWire;
	}

	public const string ID = "TravelTubeEntrance";

	public const float WAX_PER_LAUNCH = 0.05f;

	public const int STORAGE_WAX_LAUNCHECOUNT_CAPACITY = 200;

	private const float JOULES_PER_LAUNCH = 10000f;

	private const float LAUNCHES_FROM_FULL_CHARGE = 4f;
}
