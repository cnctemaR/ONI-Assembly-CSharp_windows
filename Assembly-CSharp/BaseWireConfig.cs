using System;
using TUNING;
using UnityEngine;

public abstract class BaseWireConfig : IBuildingConfig
{
	public abstract override BuildingDef CreateBuildingDef();

	public BuildingDef CreateBuildingDef(string id, string anim, float mass, float construction_time, float[] construction_mass, float insulation, DecorValues decor, AttributeInfo[] attribute_infos = null)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, anim, 800f, construction_time, construction_mass, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, decor, attribute_infos);
		buildingDef.Insulation = insulation;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.ObjectLayer = ObjectLayer.Wire;
		buildingDef.TileLayer = ObjectLayer.WireTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementWire;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DisableWhenInactive = true;
		buildingDef.SceneLayer = Grid.SceneLayer.Wires;
		buildingDef.isKAnimTile = true;
		buildingDef.isGraphTile = true;
		buildingDef.isUtility = true;
		buildingDef.OverlayAnim = Assets.GetAnim(anim);
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<Wire>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		BuildingTemplates.DoPostConfigure(go);
	}
}
