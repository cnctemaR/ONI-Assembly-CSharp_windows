using System;
using TUNING;
using UnityEngine;

public class LiquidPumpingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidPumpingStation";
		int num = 2;
		int num2 = 4;
		string text2 = "waterpump_kanim";
		int num3 = 100;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.DefaultAnimState = "on";
		buildingDef.ShowInBuildMenu = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LiquidPumpingStation>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_waterpump_kanim") };
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
	}

	private static void AddGuide(GameObject go, bool occupy_tiles)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = go.transform;
		gameObject.transform.SetLocalPosition(Vector3.zero);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.Offset = go.GetComponent<Building>().Def.GetVisualizerOffset();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim(new HashedString("waterpump_kanim")) };
		kbatchedAnimController.initialAnim = "place_guide";
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		kbatchedAnimController.isMovable = true;
		PumpingStationGuide pumpingStationGuide = gameObject.AddComponent<PumpingStationGuide>();
		pumpingStationGuide.parent = go;
		pumpingStationGuide.occupyTiles = occupy_tiles;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingPreview, false);
		LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingUnderConstruction, true);
	}

	public const string ID = "LiquidPumpingStation";
}
