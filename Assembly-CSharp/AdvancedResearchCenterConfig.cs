using System;
using TUNING;
using UnityEngine;

public class AdvancedResearchCenterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AdvancedResearchCenter", 3, 3, "research_center2_kanim", 200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 4f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.disableOnStore = true;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.Water;
		manualDeliveryKG.refillMass = 250f;
		manualDeliveryKG.capacity = 500f;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.research_point_type_id = ResearchTypes.ID.BETA;
		researchCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research2_kanim") };
		researchCenter.mass_per_point = 50f;
		go.AddOrGet<Prioritizable>();
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(GameTags.Water, 0.83f)
		};
		elementConverter.conversionInterval = 1f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
		BuildingTemplates.DoPostConfigure(go);
	}

	public const float BASE_RESEARCH_SPEED = 0.83f;

	public const float SKILL_EFFICACY = 0.27666667f;

	public const float MASS_PER_POINT = 50f;
}
