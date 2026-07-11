using System;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ClothingFabricator";
		int num = 4;
		int num2 = 3;
		string text2 = "clothingfactory_kanim";
		int num3 = 100;
		float num4 = 240f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(2, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		Fabricator fabricator = go.AddOrGet<Fabricator>();
		fabricator.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_clothingfactory_kanim") };
		fabricator.AnimOffset = new Vector3(-1f, 0f, 0f);
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "ClothingFabricator";
}
