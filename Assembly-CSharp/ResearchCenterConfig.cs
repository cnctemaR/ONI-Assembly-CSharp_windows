using System;
using TUNING;
using UnityEngine;

public class ResearchCenterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ResearchCenter";
		int num = 2;
		int num2 = 2;
		string text2 = "research_center_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.HotKey = global::Action.BuildMenuKeyR;
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
		manualDeliveryKG.requestedItemTag = GameTags.Dirt;
		manualDeliveryKG.refillMass = 250f;
		manualDeliveryKG.capacity = 500f;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research_center_kanim") };
		researchCenter.research_point_type_id = ResearchTypes.ID.ALPHA;
		researchCenter.mass_per_point = 50f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(GameTags.Dirt, 1.16f)
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

	public const float BASE_RESEARCH_SPEED = 1.16f;

	public const float MIN_RESEARCH_SPEED = 0.2f;

	public const float MASS_PER_POINT = 50f;

	public const string ID = "ResearchCenter";
}
