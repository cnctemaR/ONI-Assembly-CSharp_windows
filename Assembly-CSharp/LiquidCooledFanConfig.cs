using System;
using TUNING;
using UnityEngine;

public class LiquidCooledFanConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidCooledFan";
		int num = 2;
		int num2 = 2;
		string text2 = "fanliquid_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 100f;
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		float num = 2426.72f;
		float num2 = 0.01f;
		LiquidCooledFan liquidCooledFan = go.AddOrGet<LiquidCooledFan>();
		liquidCooledFan.gasStorage = storage;
		liquidCooledFan.liquidStorage = storage2;
		liquidCooledFan.waterKGConsumedPerKJ = 1f / (num * num2);
		liquidCooledFan.coolingKilowatts = 80f;
		liquidCooledFan.minCooledTemperature = 290f;
		liquidCooledFan.minEnvironmentMass = 0.25f;
		liquidCooledFan.minCoolingRange = new Vector2I(-2, 0);
		liquidCooledFan.maxCoolingRange = new Vector2I(2, 4);
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = new Tag("Water");
		manualDeliveryKG.capacity = 500f;
		manualDeliveryKG.refillMass = 50f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.storeOnConsume = true;
		elementConsumer.storage = storage;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRadius = 8;
		elementConsumer.EnableConsumption(true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.showDescriptor = false;
		LiquidCooledFanWorkable liquidCooledFanWorkable = go.AddOrGet<LiquidCooledFanWorkable>();
		liquidCooledFanWorkable.SetWorkTime(20f);
		liquidCooledFanWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_liquidfan_kanim") };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "LiquidCooledFan";
}
