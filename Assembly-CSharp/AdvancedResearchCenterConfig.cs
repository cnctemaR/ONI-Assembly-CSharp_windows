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
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = AdvancedResearchCenterConfig.INPUT_MATERIAL;
		manualDeliveryKG.refillMass = 150f;
		manualDeliveryKG.capacity = 750f;
		manualDeliveryKG.choreTags = GameTags.ChoreTypes.ResearchChores;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research2_kanim") };
		researchCenter.research_point_type_id = ResearchTypes.ID.BETA;
		researchCenter.inputMaterial = AdvancedResearchCenterConfig.INPUT_MATERIAL;
		researchCenter.mass_per_point = 50f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(AdvancedResearchCenterConfig.INPUT_MATERIAL, 0.83f)
		};
		elementConverter.showDescriptors = false;
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

	public const float CAPACITY = 750f;

	public static readonly Tag INPUT_MATERIAL = GameTags.Water;
}
