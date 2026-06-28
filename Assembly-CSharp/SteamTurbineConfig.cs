using System;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[] { "Plastic", "Metal" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SteamTurbine", 3, 1, "steamturbine_kanim", 25f, 30, 60f, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0]
		}, array, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, none);
		buildingDef.GeneratorWattageRating = 8000f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;
		buildingDef.Deprecated = true;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.OverheatTemperature = 2273.15f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Turbine turbine = go.AddOrGet<Turbine>();
		turbine.srcElem = SimHashes.Steam;
		turbine.srcMinTemp = 473.15f;
		turbine.destTempDelta = -50f;
		turbine.pumpKGRate = 5f;
		turbine.srcMinMass = turbine.pumpKGRate;
		turbine.destMaxMass = turbine.pumpKGRate * 3f;
		turbine.minEmitMass = 15f;
		turbine.maxRPM = 5000f;
		turbine.rpmAcceleration = turbine.maxRPM / 30f;
		turbine.rpmDeceleration = turbine.maxRPM / 20f;
		turbine.minGenerationRPM = 4000f;
		go.AddOrGet<Generator>();
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.setLiquidImpermeable = false;
		simCellOccupier.setGasImpermeable = false;
		simCellOccupier.strengthMultiplier = 1f;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "SteamTurbine";
}
