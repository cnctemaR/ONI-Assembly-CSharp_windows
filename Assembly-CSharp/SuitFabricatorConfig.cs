using System;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SuitFabricator";
		int num = 4;
		int num2 = 3;
		string text2 = "suit_maker_kanim";
		int num3 = 100;
		float num4 = 240f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		Fabricator fabricator = go.AddOrGet<Fabricator>();
		fabricator.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_suit_fabricator_kanim") };
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		};
	}

	public const string ID = "SuitFabricator";
}
