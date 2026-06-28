using System;
using TUNING;
using UnityEngine;

public class AdvancedResearchCenterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AdvancedResearchCenter";
		int num = 3;
		int num2 = 3;
		string text2 = "research_center2_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 4f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.HotKey = global::Action.BuildMenuKeyS;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.Water;
		manualDeliveryKG.refillMass = 250f;
		manualDeliveryKG.capacity = 500f;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research2_kanim") };
		researchCenter.research_point_type_id = ResearchTypes.ID.BETA;
		researchCenter.mass_per_point = 50f;
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

	public const string ID = "AdvancedResearchCenter";

	public const float BASE_RESEARCH_SPEED = 0.83f;

	public const float MASS_PER_POINT = 50f;
}
