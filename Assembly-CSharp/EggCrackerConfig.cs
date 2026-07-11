using System;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class EggCrackerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "EggCracker";
		int num = 2;
		int num2 = 2;
		string text2 = "egg_cracker_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_egg", false);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.labelByResult = false;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_egg_cracker_kanim") };
		complexFabricator.outputOffset = new Vector3(1f, 1f, 0f);
		Prioritizable.AddRef(go);
		go.AddOrGet<EggCracker>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "EggCracker";
}
