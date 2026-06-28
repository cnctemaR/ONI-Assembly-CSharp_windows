using System;
using TUNING;
using UnityEngine;

public class CompostConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Compost", 2, 2, "compost_kanim", 400f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER3, null);
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2000f;
		storage.disableOnStore = true;
		Compost compost = go.AddOrGet<Compost>();
		compost.emitHash = SimHashes.Fertilizer;
		compost.emitMassThreshold = 10f;
		CompostWorkable compostWorkable = go.AddOrGet<CompostWorkable>();
		compostWorkable.workTime = 20f;
		compostWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_compost_kanim") };
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("ToxicSand"), 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.1f, SimHashes.Fertilizer, 0f, true, 0.5f, 1f, false)
		};
		elementConverter.conversionInterval = 1f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("ToxicSand");
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.minimumMass = 1f;
		go.AddOrGet<Prioritizable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const float SAND_INPUT_PER_SECOND = 0.1f;

	public const float FERTILIZER_OUTPUT_PER_SECOND = 0.1f;
}
