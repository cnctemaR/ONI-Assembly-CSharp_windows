using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PixelPackConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = PixelPackConfig.ID;
		int num = 4;
		int num2 = 1;
		string text = "pixel_pack_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0]
		};
		string[] array2 = new string[] { "Glass", "RefinedMetal" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.NotInTiles;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER3, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonInputPort(PixelPack.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.PIXELPACK.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_INACTIVE, false, false) };
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ObjectLayer = ObjectLayer.Backwall;
		buildingDef.SceneLayer = Grid.SceneLayer.InteriorWall;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, PixelPackConfig.ID);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddComponent<ZoneTile>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<PixelPack>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
	}

	public static string ID = "PixelPack";
}
