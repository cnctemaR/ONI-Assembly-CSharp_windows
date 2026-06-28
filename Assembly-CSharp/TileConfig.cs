using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class TileConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Tile", 1, 1, "floor_basic_kanim", 400f, 3f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Tile, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, null);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DisableWhenInactive = true;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.isKAnimTile = true;
		buildingDef.isSolidTile = true;
		buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_solid");
		buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_solid_place");
		buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_info");
		buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_place_info");
		buildingDef.ConstructionOffsetFilter = new CellOffset[]
		{
			new CellOffset(0, -1)
		};
		if (buildingDef.EffectDescription == null)
		{
			buildingDef.EffectDescription = new List<Descriptor>();
		}
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent((DUPLICANTSTATS.FOUNDATION_MOVEMENT_BOOST - 1f) * 100f, GameUtil.TimeSlice.None))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent((DUPLICANTSTATS.FOUNDATION_MOVEMENT_BOOST - 1f) * 100f, GameUtil.TimeSlice.None)));
		buildingDef.EffectDescription.Add(descriptor);
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		go.AddOrGet<Insulator>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
