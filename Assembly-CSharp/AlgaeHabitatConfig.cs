using System;
using TUNING;
using UnityEngine;

public class AlgaeHabitatConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AlgaeHabitat", 1, 2, "algaefarm_kanim", 100f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.FARMABLE, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.FARMABLE;
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
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 25f;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.requestedItemTag = new Tag("Water");
		manualDeliveryKG2.capacity = 200f;
		manualDeliveryKG2.refillMass = 50f;
		AlgaeHabitat algaeHabitat = go.AddOrGet<AlgaeHabitat>();
		algaeHabitat.lightBonusMultiplier = 1.1f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.030000001f),
			new ElementConverter.ConsumedElement(new Tag("Water"), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 0.040000003f, SimHashes.Oxygen, 303.15f, false, 0f, 1f)
		};
		elementConverter.conversionInterval = 1f;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.0003333333f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
		elementConsumer.isRequired = false;
		ElementConsumer elementConsumer2 = go.AddComponent<PassiveElementConsumer>();
		elementConsumer2.elementToConsume = SimHashes.Water;
		elementConsumer2.consumptionRate = 1.25f;
		elementConsumer2.consumptionRadius = 1;
		elementConsumer2.showDescriptor = false;
		elementConsumer2.storeOnConsume = true;
		elementConsumer2.capacityKG = 10f;
		go.AddOrGet<AnimTileable>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
