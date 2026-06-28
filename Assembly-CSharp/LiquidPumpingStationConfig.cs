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
		float num3 = 200f;
		int num4 = 100;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Tile;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
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

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		LiquidPumpingStation liquidPumpingStation = go.AddOrGet<LiquidPumpingStation>();
		liquidPumpingStation.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_waterpump_kanim") };
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
	}

	private static void AddGuide(GameObject go, bool occupy_tiles)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = go.transform;
		gameObject.transform.localPosition = Vector3.zero;
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.Offset = go.GetComponent<Building>().Def.GetVisualizerOffset();
		kbatchedAnimController.SetAnims(new KAnimFile[] { Assets.GetAnim(new HashedString("waterpump_kanim")) }, true);
		kbatchedAnimController.initialAnim = "place_guide";
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kbatchedAnimController.isMovable = true;
		PumpingStationGuide pumpingStationGuide = gameObject.AddComponent<PumpingStationGuide>();
		pumpingStationGuide.parent = go;
		pumpingStationGuide.occupyTiles = occupy_tiles;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingPreview, false);
		LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingUnderConstruction, true);
	}

	public const string ID = "LiquidPumpingStation";
}
